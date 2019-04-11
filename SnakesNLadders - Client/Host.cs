using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Linq;
using System.Runtime.InteropServices;

namespace SnakesNLadders___Client
{   

public partial class Host : Form
    {
        string empty = "";
        byte[] _buffer = new byte[1024];
        int ncountdown = 5;
        int nPlayergo = 0;
        int counterr = 5;
        string szReceivedMessage;

        IPEndPoint m_LocalEndPoint;
        IPEndPoint m_SendServerAdvertisementsEndPoint;
        Int16 iPort;
        Socket m_SendServerAdvertisementsSocket;

        public MessageType eMessageType;
        public const Int16 PORT = 8000;                     // Default port 8000 decimal
        public const Int16 SERVER_ADVERTISE_PORT = 8009;	// Server Advertisement port 8009 decimal
        public const Int32 MAX_ALIAS_LEN = 15;
        public const Int32 MAX_SERVER_NAME_LEN = 30;

        // Helper variables used in Server Advertisement message Serialize, Deserialize and Receive
        Int32 m_iSA_FieldSize1; // eMessageType
        Int32 m_iSA_FieldSize2; // cServerName
        Int32 m_iSA_FieldSize3; // iIP_Address_Byte_1
        Int32 m_iSA_FieldSize4; // iIP_Address_Byte_2
        Int32 m_iSA_FieldSize5; // iIP_Address_Byte_3
        Int32 m_iSA_FieldSize6; // iIP_Address_Byte_4
        Int32 m_iSA_FieldSize7; // iPort
        Int32 m_iSA_PadSize_To4ByteBoundary; // Size of pad field to be added to meet a 4-byte boundary for interoperability with C++
        Int32 m_iSA_MessageSize; // The structure itself, plus any padding

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (MAX_ALIAS_LEN + 1))]
        public byte[] cAlias;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ((MAX_ALIAS_LEN + 1) * 10))]
        public byte[] cPlayerList;      // Comma-separated list of aliases

        public enum MessageType
        {
            REGISTER_ALIAS, PLAYER_LIST, CHOOSE_OPPONENT, LOCAL_CELL_SELECTION,
            OPPONENT_CELL_SELECTION, START_OF_GAME, END_OF_GAME, CLIENT_DISCONNECT, SERVER_ADVERTISE
        };

        // Service advertisement message structure - this is received from the Server, and needs to be deserialised
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public unsafe struct ServerAdvertise_Message_PDU
        {
            public MessageType eMessageType;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (MAX_SERVER_NAME_LEN + 1))]
            public byte[] cServerName;
            public byte iIP_Address_Byte_1;	// LSB
            public byte iIP_Address_Byte_2;
            public byte iIP_Address_Byte_3;
            public byte iIP_Address_Byte_4;	// MSB
            // Note - Field alignments (compatibility with C++ version of the game) - Due to the nature of the serialisation
            // performed in the C++ version, all 4-byte fields are aligned on 4-byte-boundaries. The cServerName field is 
            // 31 bytes long, and thus ends not on a 4-byte boundary. The four single byte fields that follow it are aligned
            // on byte boundaries. This means that in the actual transmitted byte array, a single padding byte (value 0) is
            // inserted before the iPort field, this requires attention when serialising / deserialising in the C# versions 
            // of the application, see DeSerialize_ReceivedServerAdvertisementMessage()
            public Int32 iPort; // Port values are actually Int16, but encoded as Int32 in this message format for compatibility with the pre-existing C++ version
        };

        void ReadFinalPortValue()
        {
            // The Port number can be overwritten by the user since it was
            // automatically set to the default value. Hence the need to read the final value
            iPort = Convert.ToInt16(ReceivePort_textBox.Text);
        }

        public class SocketT
        {
            public Socket _Socket { get; set; } //getters and setters for socket
            public string _Name { get; set; } //getters and setters for name
            public SocketT(Socket socket)
            {
            this._Socket = socket;
            }
        }

        public List<SocketT> _ClientSocket { get; set; }
        List<string> _names = new List<string>();
        private const int m_iMaxConnections = 4;

    struct Connection_Struct    // Define a structure to hold details about a single connection
    {
        public Socket ClientSpecific_Socket;
        public bool bInUse;
    };

    Socket m_ListenSocket;
    Connection_Struct[] m_Connection_Array = new Connection_Struct[m_iMaxConnections]; // Define an array to hold a number of connections

    System.Net.IPEndPoint m_LocalIPEndPoint;
    static int m_iNumberOfConnectedClients;
    private static System.Windows.Forms.Timer m_CommunicationActivity_Timer;

    public Host()
    {
        InitializeComponent();
        Initialise_ConnectionArray();

         m_iSA_MessageSize = CalculateServerAdvertisementMessageStructureSizeAndFieldSizes(ref m_iSA_FieldSize1, ref m_iSA_FieldSize2,
          ref m_iSA_FieldSize3, ref m_iSA_FieldSize4, ref m_iSA_FieldSize5, ref m_iSA_FieldSize6, ref m_iSA_FieldSize7, ref m_iSA_PadSize_To4ByteBoundary);

        m_CommunicationActivity_Timer = new System.Windows.Forms.Timer(); // Check for communication activity on Non-Blocking sockets every 200ms
        m_CommunicationActivity_Timer.Tick += new EventHandler(OnTimedEvent_PeriodicCommunicationActivityCheck); // Set event handler method for timer
        m_CommunicationActivity_Timer.Interval = 100;  // Timer interval is 1/10 second
        m_CommunicationActivity_Timer.Enabled = false;
        string szLocalIPAddress = GetLocalIPAddress_AsString(); // Get local IP address as a default value
        IP_Address_textBox.Text = szLocalIPAddress;             // Place local IP address in IP address field
        ReceivePort_textBox.Text = "8000";  // Default port number
        m_iNumberOfConnectedClients = 0;
        NumberOfClients_textBox.Text = System.Convert.ToString(m_iNumberOfConnectedClients);
            CheckForIllegalCrossThreadCalls = false;
            _ClientSocket = new List<SocketT>();
        try
        {   // Create the Listen socket, for TCP use
            m_ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_ListenSocket.Blocking = false;
            }
        catch (SocketException se)
        {   // If an exception occurs, display an error message
            MessageBox.Show(se.Message);
        }
    }

        Int32 CalculateServerAdvertisementMessageStructureSizeAndFieldSizes(ref Int32 F1, ref Int32 F2, ref Int32 F3,
                                 ref Int32 F4, ref Int32 F5, ref Int32 F6, ref Int32 F7, ref Int32 Pad)
        { 
            F1 = sizeof(MessageType);       
            F2 = MAX_SERVER_NAME_LEN + 1;   
            F3 = 1;                         // byte iIP_Address_Byte_1;
            F4 = 1;                        
            F5 = 1;                        
            F6 = 1;                        
            F7 = sizeof(Int32);            
            Int32 iContentMessageSize = F1 + F2 + F3 + F4 + F5 + F6 + F7;
            Pad = CalculatePadTo4ByteBoundary(iContentMessageSize);
            Int32 iPaddedMessageSize = iContentMessageSize + Pad;
            return iPaddedMessageSize;
        }

        Int32 CalculatePadTo4ByteBoundary(Int32 iContentMessageSize)
        {  
            int iRemainder = iContentMessageSize % 4; // The extent of overlap beyond 4-byte boundary
            if (0 == iRemainder)
            {   // Already on a 4-byte boundary
                return 0; // No padding necessary
            }
            else
            {   // Calculate pad to next 4-byte boundary
                return 4 - iRemainder;
            }
        }

        private void Initialise_ConnectionArray()
        {
        int iIndex;
        for (iIndex = 0; iIndex < m_iMaxConnections; iIndex++)
        {
            m_Connection_Array[iIndex].bInUse = false;
        }
        }

        IPAddress GetLocalHostAddressDetails()
        {
            string sHostName = Dns.GetHostName();                   // Get the local host's name
            IPHostEntry ipHostEntry = Dns.GetHostEntry(sHostName);  // Get the host's address details
            IPAddress[] ipHostAddress = ipHostEntry.AddressList;    // Extract the IP address
            // ipHostAddress is an array of multiple addresses, we take the first IPv4 address from this list
            for (int iAddrIndex = 0; iAddrIndex < ipHostAddress.Length; iAddrIndex++)
            {   // Iterate through list of addresses, looking for the first IPv4 address
                if (ipHostAddress[iAddrIndex].AddressFamily == AddressFamily.InterNetwork)
                {
                    byte[] bAddressBytes = ipHostAddress[iAddrIndex].GetAddressBytes();
                    IP_Address_textBox.Text = ipHostAddress[iAddrIndex].ToString();
                    return ipHostAddress[iAddrIndex];
                }
            }
            // No IPv4 address found - local host will not be able to connect to server using IPv4
            IP_Address_textBox.Text = "No IPv4 Addr";
            Bind_button.Enabled = false;
            return null;
        }

        private int GetnextAvailable_ConnectionArray_Entry()
        {
        int iIndex;
        for (iIndex = 0; iIndex < m_iMaxConnections; iIndex++)
        {
            if (false == m_Connection_Array[iIndex].bInUse)
            {
                return iIndex;  // Return the index value of the first not-in-use entry found
            }
        }
        return -1;      // Signal that there were no available entries
        }
        private void Bind_button_Click_1(object sender, EventArgs e) //once the server start button is click it will go through many 
        {
        try
        {
            // Get the Port number from the appropriate text box
            String szPort = ReceivePort_textBox.Text;
            int iPort = System.Convert.ToInt16(szPort, 10);

            m_LocalIPEndPoint = new System.Net.IPEndPoint(IPAddress.Any, iPort);
            m_ListenSocket.Bind(m_LocalIPEndPoint);
            Bind_button.Enabled = false;
            label7.Visible = true;
            label7.Text = "Bind succeded";

            ReceivePort_textBox.ReadOnly = true;
        }
        catch  // if the bind fails then it will show error bind failed.
        {   
            label7.Text = "Bind failed";
        }
            try
            {
                label7.Text = "Now Listening..";
                m_ListenSocket.Listen(2); // Listen for connections, with a backlog / queue maximum of 2
            }
            catch (SocketException se)
            {
                // If an exception occurs, display an error message
                MessageBox.Show(se.Message);
            }
            catch // Silently handle any other exception
            {
                label7.Text = "Listening Error";
            }
            m_CommunicationActivity_Timer.Start();  // Start the timer to perform periodic checking for connection requests   
            label7.Text = "Server Started \n(waiting for connection)";

            IPAddress LocalIPAddress = GetLocalHostAddressDetails();
            if (null == LocalIPAddress) // if coudne't get the ipaddress then show error.
            {
                MessageBox.Show("Could not get local IP address", "Snakes And LAdders");
                return;
            }
            m_LocalEndPoint = new IPEndPoint(LocalIPAddress, iPort);

            try // start server advertising.
            {
                Initialise_ServerAdvertisement();
                label7.Text = "Server Advertising";
            }
            catch // if fails then show error.
            {
                label7.Text = "Server Advertising Failed";
            }
        }

        string TruncateString(string sStr, int iMaxLength)
        {
            if (sStr.Length > iMaxLength)
            {  
                sStr = sStr.Substring(0, iMaxLength);
            }
            return sStr;
        }

            void Send_ServerAdvertisement_Broadcast() // this will send an broadcast to all players which is divdeded.
            {
                ServerAdvertise_Message_PDU Message = new ServerAdvertise_Message_PDU();
                Message.eMessageType = MessageType.SERVER_ADVERTISE;
                string sServerName = TruncateString(txtServerName.Text, MAX_SERVER_NAME_LEN);
                Message.cServerName = Encoding.ASCII.GetBytes(sServerName); // turns servername into bytes.
                byte[] bLocalAddress = m_LocalEndPoint.Address.GetAddressBytes();
                Message.iIP_Address_Byte_1 = bLocalAddress[0]; //turns ip part1 into bytes
                Message.iIP_Address_Byte_2 = bLocalAddress[1]; //turns ip part2 into bytes
                Message.iIP_Address_Byte_3 = bLocalAddress[2]; //turns ip part3 into bytes
                Message.iIP_Address_Byte_4 = bLocalAddress[3]; //turns ip part4 into bytes
                Message.iPort = iPort; // gets the port.and turns it into the message that will be sent to everyone on the getaway.

                byte[] bBuffer = Serialize_ServerAdvertise_Message_PDU(Message); // this will serialize the message array so it will be easy and short to send.
                int iBytesSent;
                try
                {
                    iBytesSent = m_SendServerAdvertisementsSocket.SendTo(bBuffer, m_SendServerAdvertisementsEndPoint);
                    if (0 < iBytesSent)
                    {
                    }
                }
                catch (SocketException ex)
                {
                    MessageBox.Show("Server Advertise broadcast send failed", "Snakes And Ladders"); //show message box that the server advertise failed.
                }
            }

            void Initialise_ServerAdvertisement()
            {
            try
            {
                m_SendServerAdvertisementsSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                m_SendServerAdvertisementsSocket.Blocking = false; // Non-blocking mode
                m_SendServerAdvertisementsSocket.EnableBroadcast = true; // Set broadcast mode on
            }
            catch (SocketException)  // Catch exceptions from the socket operations
            {
                MessageBox.Show("CreateSocket() failed", "'Snakes And Ladders' Server");
                //CloseAll();
            }
            InitialiseServerAdvertiseAddressStructAndPort();
            Start_SendServerAdvertisement_Timer();
        }

        void OnTimedEvent_SendServerAdvertisement_Timer(Object myObject, EventArgs myEventArgs)
        {
            Send_ServerAdvertisement_Broadcast();
        }
        void Start_SendServerAdvertisement_Timer()
        {
            m_SendServerAdvertisement_Timer = new System.Windows.Forms.Timer();
            m_SendServerAdvertisement_Timer.Interval = 1000; // 1 second interval
            m_SendServerAdvertisement_Timer.Tick += OnTimedEvent_SendServerAdvertisement_Timer;
            m_SendServerAdvertisement_Timer.Enabled = true;
        }

        void InitialiseServerAdvertiseAddressStructAndPort()
        {   // Initialise the IP address and Port number used for broadcasting the Server Advertisement messages
            m_SendServerAdvertisementsEndPoint = new IPEndPoint(IPAddress.Broadcast, SERVER_ADVERTISE_PORT);
        }

        public string GetLocalIPAddress_AsString()
        {
        string szHost = Dns.GetHostName();
        string szLocalIPaddress = "127.0.0.1";  // Default is local loopback address
        IPHostEntry IPHost = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress IP in IPHost.AddressList)
        {
            if (IP.AddressFamily == AddressFamily.InterNetwork) // Match only the IPv4 address
            {
                szLocalIPaddress = IP.ToString();
                break;
            }
        }
        return szLocalIPaddress;
        }
       

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void Done_button_Click(object sender, EventArgs e)
        {
        Close_And_Quit();
        }

    private void Close_And_Quit()
    {   // Close the sockets and exit the application
        try
        {
            m_ListenSocket.Close();
        }
        catch
        {
        }
        try
        {
            int iIndex;
            for (iIndex = 0; iIndex < m_iMaxConnections; iIndex++)
            {
                m_Connection_Array[iIndex].ClientSpecific_Socket.Shutdown(SocketShutdown.Both);  //closes the sockets and alll the connections
                    m_Connection_Array[iIndex].ClientSpecific_Socket.Close(); //closes the sockets and alll the connections
            }
        }
        catch
        {
        }
        try
        {
            Close();
        }
        catch
        {
        }
    }

    private void OnTimedEvent_PeriodicCommunicationActivityCheck(Object myObject, EventArgs myEventArgs) //when something is sent.
    {  
        int iIndex;
        iIndex = GetnextAvailable_ConnectionArray_Entry(); 
        if (-1 != iIndex)
        {   
            try
            {
                m_Connection_Array[iIndex].ClientSpecific_Socket = m_ListenSocket.Accept();  // Accept a connection (if pending) and assign a new socket to it (AcceptSocket)              
                m_Connection_Array[iIndex].bInUse = true;
                m_Connection_Array[iIndex].ClientSpecific_Socket.Blocking = false;           // Make the new socket operate in non-blocking mode
                m_iNumberOfConnectedClients++;
                NumberOfClients_textBox.Text = System.Convert.ToString(m_iNumberOfConnectedClients);
                Status_textBox.Text = "A new client connected";
                SendUpdateMesageToAllConnectedclients(empty);
            }
            catch (SocketException se) // Handle socket-related exception
            {   // If an exception occurs, display an error message
                if (10053 == se.ErrorCode || 10054 == se.ErrorCode) // Remote end closed the connection
                {
                    CloseConnection(iIndex);
                }
                else if (10035 != se.ErrorCode)
                {   // Ignore error messages relating to normal behaviour of non-blocking sockets
                    MessageBox.Show(se.Message);
                }
            }
            catch // Silently handle any other exception
            {
            }
        }

        // Second, check for received messages on each connected socket
        for (iIndex = 0; iIndex < m_iMaxConnections; iIndex++)
        {
            if (true == m_Connection_Array[iIndex].bInUse)
            {
                try
                {
                    EndPoint localEndPoint = (EndPoint)m_LocalIPEndPoint; 
                    byte[] ReceiveBuffer = new byte[1024];
                    int iReceiveByteCount;
                    iReceiveByteCount = m_Connection_Array[iIndex].ClientSpecific_Socket.ReceiveFrom(ReceiveBuffer, ref localEndPoint);

                        if (0 < iReceiveByteCount)
                        {   // Copy the number of bytes received, from the message buffer to the text control
                            szReceivedMessage = Encoding.ASCII.GetString(ReceiveBuffer, 0, iReceiveByteCount);
                        
                            string diceposition = szReceivedMessage;

                            int Dice = diceposition.IndexOf(" ");
                            int positon = diceposition.IndexOf(" ", Dice + 1);

                            try { 
                            if (Dice != positon && Dice >= 0)
                            {
                                diceposition = diceposition.Remove(Dice + 1, positon - Dice);
                            }
                            }
                            catch { }

                            if (diceposition.Contains("/Dice")) // if the message includes dice then then the following commands
                            {
                                string userName = listBox1.Items[iIndex].ToString(); // gets the username of the player which has sent it.
                                Message_textBox.AppendText("\n" + "ServerLOG:" + userName + " Has rolled the dice and got : (" + diceposition.Substring(5) + ")"); //shows what dice the player has rolled keeps as log in the server.
                                userName += " Has Rolled and Achived " + diceposition.Substring(5);//creates string to send back to all users.
                                SendUpdateMesageToAllConnectedclients("/DIC" + userName);// send a small command so it can divide the and take it as dice. (username includes all lines)
                            }

                            if (szReceivedMessage.Contains("/STOP")) //if the clients reach end point then it will stop all the timers.
                            {
                               ncountdown = 5;
                               nPlayergo = 0;
                               counterr = 5;
                                counter.Stop();
                                PlayerGo.Stop();
                                CountDown.Stop();
                                return;
                            }

                            if ("/User:" == szReceivedMessage.Substring(0, 6))// when a user conencts they call user with the username they have inputed which lists inside listbox.
                            {
                                listBox1.Items.Add(szReceivedMessage.Substring(6)); //adds username into listbox. (takes /user away)
                            }

                            else if ("/Mesg:" == szReceivedMessage.Substring(0, 6)) // when a player messages it will take start with /mesg
                            {
                                string userName = listBox1.Items[iIndex].ToString(); //gets players name
                                Message_textBox.AppendText("\n " + userName + " : " + szReceivedMessage.Substring(6)); //keeps message as log into host richtextbox.
                                
                
                                userName += " : " + szReceivedMessage.Substring(6); //uses string to send backk to all clients connected.
                                SendUpdateMesageToAllConnectedclients("MSG"+userName); //uses string to send backk to all clients connected.
                            }

                            //synchronisation 

                            else if (szReceivedMessage.Contains("/P1"))  //if the player 1 has moved location then it will send this command including new position example /P1056;
                            {
                                if (diceposition.StartsWith("/P")) //somestimes commands are connected so if it starts with it then send it like this.
                                {
                                    SendUpdateMesageToAllConnectedclients(szReceivedMessage);
                                }
                                else //sometimes it will be at the end so we will remove the first commands which is normaly the dice.
                                {
                                    SendUpdateMesageToAllConnectedclients(szReceivedMessage.Substring(8));
                                }
                            }
                            else if (szReceivedMessage.Contains("/P2")) //if the player 1 has moved location then it will send this command including new position example /P1056;
                            {
                                if (diceposition.StartsWith("/P"))
                                {
                                    SendUpdateMesageToAllConnectedclients(szReceivedMessage);
                                }
                                else //sometimes it will be at the end so we will remove the first commands which is normaly the dice.
                                {  
                                SendUpdateMesageToAllConnectedclients(szReceivedMessage.Substring(8));
                                }
                            }

                            else if (szReceivedMessage.Contains("/P3")) //if the player 1 has moved location then it will send this command including new position example /P1056;
                            {
                                if (diceposition.StartsWith("/P"))
                                {
                                    SendUpdateMesageToAllConnectedclients(szReceivedMessage);
                                }
                                else //sometimes it will be at the end so we will remove the first commands which is normaly the dice.
                                {
                                    SendUpdateMesageToAllConnectedclients(szReceivedMessage.Substring(8));
                                }
                            }

                            else if (szReceivedMessage.Contains("/P4")) //if the player 1 has moved location then it will send this command including new position example /P1056;
                            {
                                if (diceposition.StartsWith("/P"))
                                {
                                    SendUpdateMesageToAllConnectedclients(szReceivedMessage);
                                }
                                else //sometimes it will be at the end so we will remove the first commands which is normaly the dice.
                                {
                                    SendUpdateMesageToAllConnectedclients(szReceivedMessage.Substring(8));
                                }
                            }

                            else if ("/iInx:" == szReceivedMessage.Substring(0, 6)) //sends the user their player number to keep synchronisation 
                            {
                                String userName = "N" + (iIndex + 1); // iIndex starts with 0 so we allways add 1;
                                SendUpdateMesageToAllConnectedclients(userName); // iIndex starts with 0 so we allways add 1;
                                timer1.Start(); //if they want they can make it send every time but this is disabled atm.
                            }
                            

                        }
                }
                catch (SocketException se) // Handle socket-related exception
                {   // If an exception occurs, display an error message
                    if (10053 == se.ErrorCode || 10054 == se.ErrorCode) // Remote end closed the connection
                    {
                        CloseConnection(iIndex);
                    }
                    else if (10035 != se.ErrorCode)
                    {   // Ignore error messages relating to normal behaviour of non-blocking sockets
                        MessageBox.Show(se.Message);
                    }
                }
                catch // Silently handle any other exception
                {
                        Message_textBox.AppendText(szReceivedMessage);
                }
            }
        }
    }

        private void SendUpdateMesageToAllConnectedclients(String msg)
        {   // Send message to each connected client informing of the total number of connected clients
        int iIndex;
        for (iIndex = 0; iIndex < m_iMaxConnections; iIndex++)
        {
            if (true == m_Connection_Array[iIndex].bInUse)
            {
                string szMessage;
                if (1 == m_iNumberOfConnectedClients)
                {
                    szMessage = "\n" + string.Format(msg, m_iNumberOfConnectedClients);
                }
                else
                {
                    szMessage = "\n" + string.Format(msg, m_iNumberOfConnectedClients);
                    }
                byte[] SendMessage = System.Text.Encoding.ASCII.GetBytes(szMessage);
                m_Connection_Array[iIndex].ClientSpecific_Socket.Send(SendMessage, SocketFlags.None);
            }
        }
    }
    private void CloseConnection(int iIndex) //closes the connection
    {
        try
        {
            m_Connection_Array[iIndex].bInUse = false;
            m_Connection_Array[iIndex].ClientSpecific_Socket.Shutdown(SocketShutdown.Both);
            m_Connection_Array[iIndex].ClientSpecific_Socket.Close();
            m_iNumberOfConnectedClients--;
            NumberOfClients_textBox.Text = System.Convert.ToString(m_iNumberOfConnectedClients);
            Status_textBox.Text = "A Connection was closed";
            SendUpdateMesageToAllConnectedclients(empty);
        }
        catch // Silently handle any exceptions
        {
        }
    }
        private void Message_textBox_TextChanged(object sender, EventArgs e)
        {
            Message_textBox.SelectionStart = Message_textBox.Text.Length;
            Message_textBox.ScrollToCaret(); //keeps the textbox allways to update(stops users from scrolling down)
        }

        private void timer1_Tick(object sender, EventArgs e) //timer which clients every second/
        {
            byte[] inStream = new byte[1500];
            string[] items = listBox1.Items.OfType<object>().Select(item => item.ToString()).ToArray();
            string result = string.Join(" | ", items); // joins everything on the listbox and uses | to add between all then which is used to split after reaches client.
            SendUpdateMesageToAllConnectedclients("L" + result); // sends the list of users to everyone connected so this keeps listbox on client updated on every tick.
        }

        private void CountDown_Tick(object sender, EventArgs e)
        {
            ncountdown--;
            textBox1.Text = ""+ncountdown;
            SendUpdateMesageToAllConnectedclients("/Countdown "+ncountdown); //sends the countdown of the server game starting to players. (syncrosation)
            if (ncountdown == 0)
            {
                CountDown.Stop();
                PlayerGo.Start();
            }
        }

        private void PlayerGo_Tick(object sender, EventArgs e)//how the timer will work depending on how many players are online.
        {
            if (NumberOfClients_textBox.Text == "1") //if one player is online then make it solo play so it will be back to player one onces its finished.
            {
                if (nPlayergo == 0)
                {
                    nPlayergo = 5;
                }
                nPlayergo--;
                if (nPlayergo <= 5)    // Solo Play
                {
                    counter.Start(); //starts a counter from 5 = 0;
                    txtPlayerTurn.Text = "Player 1's Turn |" + " Time Left " + counterr; // sends message to clients to show which player's go it is.
                    SendUpdateMesageToAllConnectedclients("/PlayerGo1"); //sends command to lobby for the client to know whos turn to shake.
                }
                return;
            }

            if (NumberOfClients_textBox.Text == "2") // 1 v 1 // if there are 2 players online then they will both takes go's.
            {
                if (nPlayergo == 0)
                {
                    nPlayergo = 10;
                }
                nPlayergo--;
                if (nPlayergo <= 10 && nPlayergo > 5)
                {
                    counter.Start(); //starts a counter from 5 = 0;
                    txtPlayerTurn.Text = "Player 1's Turn |" + " Time Left " + counterr; //shows on the host page which players turn it is.
                    SendUpdateMesageToAllConnectedclients("/PlayerGo1"); //sends command to lobby for the client to know whos turn to shake.
                }
                if (nPlayergo <= 5)
                {
                    counter.Start();
                    txtPlayerTurn.Text = "Player 2's Turn |" + " Time Left " + counterr; //shows on the host page which players turn it is.
                    SendUpdateMesageToAllConnectedclients("/PlayerGo2"); //sends command to lobby for the client to know whos turn to shake.
                }
                return;
            }

            if (NumberOfClients_textBox.Text == "3") // Three for All
            {
                if (nPlayergo == 0)
                {
                    nPlayergo = 15;
                }
                nPlayergo--;
                if (nPlayergo <= 15 && nPlayergo > 10)
                {
                    counter.Start(); //starts a counter from 5 = 0;
                    txtPlayerTurn.Text = "Player 1's Turn |" + " Time Left " + counterr; //shows on the host page which players turn it is.
                    SendUpdateMesageToAllConnectedclients("/PlayerGo1"); //sends command to lobby for the client to know whos turn to shake.
                }
                if (nPlayergo <= 10 && nPlayergo > 5)
                {
                    counter.Start(); //starts a counter from 5 = 0;
                    txtPlayerTurn.Text = "Player 2's Turn |" + " Time Left " + counterr; //shows on the host page which players turn it is.
                    SendUpdateMesageToAllConnectedclients("/PlayerGo2"); //sends command to lobby for the client to know whos turn to shake.
                }
                if (nPlayergo <= 5)
                {
                    counter.Start(); //starts a counter from 5 = 0;
                    txtPlayerTurn.Text = "Player 3's Turn |" + " Time Left " + counterr; //shows on the host page which players turn it is.
                    SendUpdateMesageToAllConnectedclients("/PlayerGo3"); //sends command to lobby for the client to know whos turn to shake.
                }
                return;
            }

            if (NumberOfClients_textBox.Text == "4") // 4 Man Escort
            {
                if (nPlayergo == 0)
                {
                    nPlayergo = 20;
                }
                nPlayergo--;
                if (nPlayergo <= 20 && nPlayergo > 15)
                {
                    counter.Start(); //starts a counter from 5 = 0;
                    txtPlayerTurn.Text = "Player 1's Turn |" + " Time Left " + counterr; //shows on the host page which players turn it is.
                    SendUpdateMesageToAllConnectedclients("/PlayerGo1"); //sends command to lobby for the client to know whos turn to shake.
                }
                if (nPlayergo <= 15 && nPlayergo > 10)
                {
                    counter.Start(); //starts a counter from 5 = 0;
                    txtPlayerTurn.Text = "Player 2's Turn |" + " Time Left " + counterr; //shows on the host page which players turn it is.
                    SendUpdateMesageToAllConnectedclients("/PlayerGo2"); //sends command to lobby for the client to know whos turn to shake.
                }
                if (nPlayergo <= 10 && nPlayergo > 5)
                {
                    counter.Start(); //starts a counter from 5 = 0;
                    txtPlayerTurn.Text = "Player 3's Turn |" + " Time Left " + counterr; //shows on the host page which players turn it is.
                    SendUpdateMesageToAllConnectedclients("/PlayerGo3"); //sends command to lobby for the client to know whos turn to shake.
                }
                if (nPlayergo <= 5)
                {
                    counter.Start(); //starts a counter from 5 = 0;
                    txtPlayerTurn.Text = "Player 4's Turn |" + " Time Left " + counterr; //shows on the host page which players turn it is.
                    SendUpdateMesageToAllConnectedclients("/PlayerGo4"); //sends command to lobby for the client to know whos turn to shake.
                }
                return;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            CountDown.Start();
        }

        private void counter_Tick(object sender, EventArgs e)
        {
            counterr--;
            SendUpdateMesageToAllConnectedclients("/timerleft" + counterr);
            if (counterr == 0)
            {
                counterr = 5;
            }
        }

        private byte[] Serialize_ServerAdvertise_Message_PDU(ServerAdvertise_Message_PDU Message)
        {   // Convert an Message_PDU structure into a byte array
            byte[] bByteArray = new byte[m_iSA_MessageSize];        // Create a byte array to hold the serialised form of the message PDU

            int iFieldOffset = 0; // Create an offset value which ensures the byte array is constructed with correct field positions and separations 
            byte[] bWorkingArray = BitConverter.GetBytes((Int32)Message.eMessageType);
            Buffer.BlockCopy(bWorkingArray/*src*/, 0 /*source offset*/, bByteArray/*dest*/, iFieldOffset/*dest offset*/, m_iSA_FieldSize1/*count*/);

            iFieldOffset += m_iSA_FieldSize1;                  // Adjust offset for second field
            int iDataLength2 = Message.cServerName.Length;  // Get the length of data actually stored in field2 (variable length data, fixed size field)
            Buffer.BlockCopy(Message.cServerName/*src*/, 0 /*source offset*/, bByteArray/*dest*/, iFieldOffset/*dest offset*/, iDataLength2/*count*/);

            iFieldOffset += m_iSA_FieldSize2;                  // Adjust offset for third field
            bByteArray[iFieldOffset] = Message.iIP_Address_Byte_1;
            iFieldOffset += m_iSA_FieldSize3;                  // Adjust offset for fourth field
            bByteArray[iFieldOffset] = Message.iIP_Address_Byte_2;
            iFieldOffset += m_iSA_FieldSize4;                  // Adjust offset for fifth field
            bByteArray[iFieldOffset] = Message.iIP_Address_Byte_3;
            iFieldOffset += m_iSA_FieldSize5;                  // Adjust offset for sixth field
            bByteArray[iFieldOffset] = Message.iIP_Address_Byte_4;

            iFieldOffset += m_iSA_FieldSize6;                  // Adjust offset for seventh field


            iFieldOffset++; // Skip one byte to start port encoding on 4-byte boundary

            bByteArray[iFieldOffset++] = (byte)(Message.iPort / 256); // Create the MSB byte of the port value
            bByteArray[iFieldOffset++] = (byte)(Message.iPort % 256); // Create the LSB byte of the port value
            bByteArray[iFieldOffset++] = 0; // Complete the encoding of the additional bytes (so that the natively Int16 port value is encoded as Int32)
            bByteArray[iFieldOffset++] = 0; // Complete the encoding of the additional bytes (so that the natively Int16 port value is encoded as Int32)

            return bByteArray;
        }
    }
}
