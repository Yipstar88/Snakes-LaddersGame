using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace SnakesNLadders___Client
{
    public partial class LOBBY : Form
    {
        Socket m_ClientSocket;
        System.Net.IPEndPoint m_remoteEndPoint;
        private static System.Windows.Forms.Timer m_CommunicationActivity_Timer;
        System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();

        //server advertisement
        public const Int16 PORT = 8000;                     // Default port 8000 decimal
        public const Int16 SERVER_ADVERTISE_PORT = 8009;	// Server Advertisement port 8009 decimal
        public const Int32 MAX_SERVER_NAME_LEN = 30;

        Socket m_ListenForServerAdvertisementsSocket;
        IPEndPoint m_ConnectEndPoint;
        IPEndPoint m_ReceiveServerAdvertisementsEndPoint;

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
            public Int32 iPort; // Port values are actually Int16, but encoded as Int32 in this message format for compatibility with the pre-existing C++ version
        };

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

        String IP;
        

        //game
        Point ImageSize = new Point(890, 670);
        Boolean shown = false;
        Boolean onoff1 = true;
        Boolean onoff2 = true;
        Boolean onoff3 = true;
        Boolean onoff4 = true;
        Boolean PlayerID = true;
        Boolean Music = true;

        //position for each players coorinate.
        List<Point> Coordinates = new List<Point>(100);
        int P1Position = 0;
        int P2Position = 0;
        int P3Position = 0;
        int P4Position = 0;

        public LOBBY()
        {
            m_iSA_MessageSize = CalculateServerAdvertisementMessageStructureSizeAndFieldSizes(ref m_iSA_FieldSize1, ref m_iSA_FieldSize2, ref m_iSA_FieldSize3,
                        ref m_iSA_FieldSize4, ref m_iSA_FieldSize5, ref m_iSA_FieldSize6, ref m_iSA_FieldSize7, ref m_iSA_PadSize_To4ByteBoundary);

            InitializeComponent();
            m_CommunicationActivity_Timer = new System.Windows.Forms.Timer(); // Check for communication activity on Non-Blocking sockets every 200ms
            m_CommunicationActivity_Timer.Interval = 100;  // Timer interval is 1/10 second
            m_CommunicationActivity_Timer.Enabled = false;
            string szLocalIPAddress = GetLocalIPAddress_AsString();
            IP = szLocalIPAddress;             // Place local IP address in IP address field
            

            Coordinates.Add(new Point(0, 0)); // this is a calculations on how the position of the map works.
            bool LeftToRight = true; // we made a boolean as our map first goes left then it goes right.
            for (int Vertical = ImageSize.Y; Vertical >= 0; Vertical -= ImageSize.Y / 10)
            {
                if (LeftToRight) // if it goes from left to right then it will 
                {
                    for (int Horizontal = ImageSize.X / 10; Horizontal <= ImageSize.X; Horizontal += ImageSize.X / 10) // sets the size from left to right.
                    {
                        Coordinates.Add(new Point(Horizontal, Vertical));
                    }
                    LeftToRight = false; //once it went from left to right it will go right to left if this is false;

                }
                else
                {
                    for (int Horizontal = ImageSize.X; Horizontal >= ImageSize.X / 10; Horizontal -= ImageSize.X / 10) //sets the size from right to left
                    {
                        Coordinates.Add(new Point(Horizontal, Vertical)); // sets the cordinates.
                    }
                    LeftToRight = true; ; // once it went from left to right now it will go right to left.
                }
            }


        }

        Int32 CalculateServerAdvertisementMessageStructureSizeAndFieldSizes(ref Int32 F1, ref Int32 F2, ref Int32 F3,
                             ref Int32 F4, ref Int32 F5, ref Int32 F6, ref Int32 F7, ref Int32 Pad)
        {  // Helper method, relating to the use of the ServerAdvertise_Message_PDU message structure
            // simplifies message Serialize, Deserialize and Receive
            F1 = sizeof(MessageType);       // eMessageType
            F2 = MAX_SERVER_NAME_LEN + 1;   // cServerName
            F3 = 1;                         // byte iIP_Address_Byte_1;
            F4 = 1;                         // byte iIP_Address_Byte_2;
            F5 = 1;                         // byte iIP_Address_Byte_3;
            F6 = 1;                         // byte iIP_Address_Byte_4;
            F7 = sizeof(Int32);             // iPort, only needs Int16, but sent as Int32 (for compatibility with c++ versions)
            Int32 iContentMessageSize = F1 + F2 + F3 + F4 + F5 + F6 + F7;
            Pad = CalculatePadTo4ByteBoundary(iContentMessageSize);
            Int32 iPaddedMessageSize = iContentMessageSize + Pad;
            return iPaddedMessageSize;
        }
        Int32 CalculatePadTo4ByteBoundary(Int32 iContentMessageSize)
        {   // Helper method, relating to the use of the Message_PDU message structure
            // calculates the size of the padding neccesary to truncate the message on a 4-byte boundary (compatible with C++)
            // Pad will always be between 0 and 3 bytes
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

        byte[] receivedBuf = new byte[1024];

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

        private void Close_Socket_and_Exit()
        {
            try
            {
                m_ClientSocket.Shutdown(SocketShutdown.Both);
            }
            catch // Silently handle any exceptions
            {
            }
            try
            {
                m_ClientSocket.Close();
            }
            catch // Silently handle any exceptions
            {
            }
            this.Close();
        }

        private void ReceiveData(IAsyncResult ar)  //this method is called when the server has sent a byte;
        {
            Socket socket = (Socket)ar.AsyncState;
            int received = socket.EndReceive(ar);
            byte[] dataBuf = new byte[received];
            Array.Copy(receivedBuf, dataBuf, received);
            m_ClientSocket.BeginReceive(receivedBuf, 0, receivedBuf.Length, SocketFlags.None, new AsyncCallback(ReceiveData), m_ClientSocket);

            string result = System.Text.Encoding.UTF8.GetString(dataBuf); // converts the byte into a string so it is readable.

            if  (result.StartsWith("\n/Countdown")) // if the command starts with countdown then do the following. // it counts from server side. to make it sync
            {
                txtTime.Text= result.Substring(11); // countdown is used to let the players know when the game is starting.
            }

            if (result.StartsWith("\n/timerleft")) // to make it sync we have sent another timer for when its a players go.
            {
                lblTimer.Text = "Roll Dice (Time Left):"; // edit lable to let the user know what the timer is for/
                txtTime.Text = result.Substring(11); // sets the timer into a textfield.
            }

            int OnlinePlayers = listBox1.Items.Count;  // counts the listbox (/listbox = from server to see who is online.
            if (OnlinePlayers == 1) // 1 players  (if one player is online)
            {
                if (PlayerID == true) // boolean used to stop list from refreshing.
                {
                    turnoffPlayers(); // void is called to turn of all labels.
                    PlayerID = false; // boolean is now off.
                }
                picPlayer1.Visible = true;  
                lblPlayer1.Visible = true;
                P1POS.Visible = true;
                lblP1lbl.Visible = true;
                lblPlayer1.Text = listBox1.Items[0].ToString();   // this code will show when their is only one player online.
            }
            if (OnlinePlayers == 2) // 2 players // if there is 2 players online then it will show a list of 2 players 
            {
                if (PlayerID == true) // stops refreshing page.
                {
                    turnoffPlayers();
                    PlayerID = false;
                }
                
                picPlayer1.Visible = true;
                picPlayer2.Visible = true;

                lblPlayer1.Visible = true;
                lblPlayer2.Visible = true;

                P1POS.Visible = true;
                P2POS.Visible = true;

                lblP1lbl.Visible = true;
                lblP2lbl.Visible = true;
                lblPlayer1.Text = listBox1.Items[0].ToString();
                lblPlayer2.Text = listBox1.Items[1].ToString(); // gets username from listbox to show near icon
            }
            if (OnlinePlayers == 3) // 3 players  // same as above.
            {
                if (PlayerID == true)
                {
                    turnoffPlayers();
                    PlayerID = false;
                }
                picPlayer1.Visible = true;
                picPlayer2.Visible = true;
                picPlayer3.Visible = true;

                lblPlayer1.Visible = true;
                lblPlayer2.Visible = true;
                lblPlayer3.Visible = true;

                P1POS.Visible = true;
                P2POS.Visible = true;
                P3POS.Visible = true;

                lblP1lbl.Visible = true;
                lblP2lbl.Visible = true;
                lblP3lbl.Visible = true;

                lblPlayer1.Text = listBox1.Items[0].ToString();
                lblPlayer2.Text = listBox1.Items[1].ToString();
                lblPlayer3.Text = listBox1.Items[2].ToString();
            }
            if (OnlinePlayers == 4) // 4 players // same as above. 
            {
                if (PlayerID == true)
                {
                    turnoffPlayers();
                    PlayerID = false;
                }
                picPlayer1.Visible = true;
                picPlayer2.Visible = true;
                picPlayer3.Visible = true;
                picPlayer4.Visible = true;

                lblPlayer1.Visible = true;
                lblPlayer2.Visible = true;
                lblPlayer3.Visible = true;
                lblPlayer4.Visible = true;

                P1POS.Visible = true;
                P2POS.Visible = true;
                P3POS.Visible = true;
                P4POS.Visible = true;

                lblP1lbl.Visible = true;
                lblP2lbl.Visible = true;
                lblP3lbl.Visible = true;
                lblP4lbl.Visible = true;

                lblPlayer1.Text = listBox1.Items[0].ToString();
                lblPlayer2.Text = listBox1.Items[1].ToString();
                lblPlayer3.Text = listBox1.Items[2].ToString();
                lblPlayer4.Text = listBox1.Items[3].ToString();
            }

            if (result.StartsWith("\n/PlayerGo1")) // the server sends which players go (if it sends playergo1) then it will allow user to take his go.
            {
                statusLabel.Text = "Player 1: Roll Dice"; // changes lable to make sure his aware thats his go.

                if (iIndexSHOW.Text == "1" && onoff1 == true ) // iIndex = which player number he is. if it's 2 he wont have this option.
                { // bollean is used to stop player from repetitive pressing button.
                    onoff1 = false; // turns boolean off
                    btnshake.Enabled = true;   // sets button avaiable for player to shake dice.
                }
                if (txtTime.Text == "0" && statusLabel.Text == "Player 1: Roll Dice") // once the text reaches 0 the bollean turns back on
                {// but wont be able to shake as it would be next players go.
                    onoff1 = true;
                }
            }

            if (result.StartsWith("\n/PlayerGo2"))  // the server sends which players go (if it sends playergo2) then it will allow user to take his go.
            {
                statusLabel.Text = "Player 2: Roll Dice"; // changes lable to make sure his aware thats his go.

                if (iIndexSHOW.Text == "2" && onoff2 == true) // iIndex = which player number he is. if it's 1 he wont have this option.
                {// bollean is used to stop player from repetitive pressing button.
                    onoff2 = false; // turns boolean off
                    btnshake.Enabled = true; // sets button avaible.
                }
                if (txtTime.Text == "0" && statusLabel.Text == "Player 2: Roll Dice") // once the text reaches 0 the bollean turns back on
                {// but wont be able to shake as it would be next players go.
                    onoff2 = true;
                }
                
            }

            if (result.StartsWith("\n/PlayerGo3")) // the server sends which players go (if it sends playergo3) then it will allow user to take his go.
            {
                statusLabel.Text = "Player 3: Roll Dice"; // changes lable to make sure his aware thats his go.

                if (iIndexSHOW.Text == "3" && onoff3 == true) // iIndex = which player number he is. if it's 1 he wont have this option.
                {// bollean is used to stop player from repetitive pressing button.
                    onoff3 = false;  // turns boolean off
                    btnshake.Enabled = true; // sets button avaible.
                }
                if (txtTime.Text == "0" && statusLabel.Text == "Player 3: Roll Dice") // once the text reaches 0 the bollean turns back on
                {// but wont be able to shake as it would be next players go.
                    onoff3 = true;
                }

            }

            if (result.StartsWith("\n/PlayerGo4"))  // this is same as above.
            {
                statusLabel.Text = "Player 4: Roll Dice";

                if (iIndexSHOW.Text == "4" && onoff4 == true)
                {
                    onoff4 = false;
                    btnshake.Enabled = true;
                }
                if (txtTime.Text == "0" && statusLabel.Text == "Player 4: Roll Dice")
                {
                    onoff4 = true;
                }

            }


            if (shown == false && result.StartsWith("\nN")) // this gets which i index value they are
            {// we used it so onces they connect how many players they are they become that player. (example player 1 will be 1) and player 2 would  be 2)
                shown = true; // sets a boolean so this doesn't change onces a player is connects.
                iIndexSHOW.Text = result.Substring(2); // sets the value into a textfield for use.
            }

            if (result.StartsWith("\nMSG")) // when a message is sent to server it sends back to all clients.
            {  
                messagesbox.AppendText("\n"+result.Substring(4)); // this will get the message we receieved cut the first 4 of the string and release the rest into 
            } // textfield for everyone to see (chatbox)

            try
            { 
            if (result.Contains("/DIC")) // when a player roles the dice it sends to server which then sends it back to everyone to see what they have rolled.
            {
                 announcements.AppendText("\n"+result.Substring(5));
            }
            }
            catch { }


            if (result.StartsWith("\n/P1"))  // this is for syncronsation (recieves the players positions)
            {
                string resiv = (Encoding.ASCII.GetString(dataBuf).Substring(4)); // gets rid of the first 4 of the string (\m/p1) to keep only 003 for example = player poistion.
                int postion = Convert.ToInt32(resiv.Substring(0, 3)); // cuts the values so it only gets the position. // somestimes it connects with other databufs.
                P1Position = SnakesLadders(postion); // goes through a void if it hit a snakes or a ladder.
                P1POS.Text = "" + P1Position;
                P1PIC.Location = Coordinates[P1Position]; // sets the cordination of the user.
                Controls.Add(P1PIC); // adds into location
                P1PIC.BringToFront();  // keeps the image on top of the map/
                P1PIC.SizeMode = PictureBoxSizeMode.StretchImage; // to keep the image inside the picturebox.
                if (P1Position >= 100) // when they reach position 100 they win.
                {
                    TheWin(lblPlayer1.Text);
                }
            }
            if (result.StartsWith("\n/P2")) // this is for syncronsation (recieves the players positions)
            {
                string resiv = (Encoding.ASCII.GetString(dataBuf).Substring(4)); // gets rid of the first 4 of the string (\m/p1) to keep only 003 for example = player poistion.
                int postion = Convert.ToInt32(resiv.Substring(0, 3)); // cuts the values so it only gets the position. // somestimes it connects with other databufs.
                P2Position = SnakesLadders(postion); // goes through a void if it hit a snakes or a ladder.
                P2POS.Text = "" + P2Position; 
                P2PIC.Location = Coordinates[Convert.ToInt32(P2POS.Text)]; // converts the reciceved data into int32 then adds into location
                Controls.Add(P2PIC); // adds into location
                P2PIC.BringToFront(); // keeps the image on top of the map/
                P2PIC.SizeMode = PictureBoxSizeMode.StretchImage; // to keep the image inside the picturebox.
                if (P2Position >= 100)
                {
                    TheWin(lblPlayer2.Text);
                }
            }

            if (result.StartsWith("\n/P3")) // this is for syncronsation (recieves the players positions)
            {
                string resiv = (Encoding.ASCII.GetString(dataBuf).Substring(4)); // gets rid of the first 4 of the string (\m/p1) to keep only 003 for example = player poistion.
                int postion = Convert.ToInt32(resiv.Substring(0, 3)); // cuts the values so it only gets the position. // somestimes it connects with other databufs.
                P3Position = SnakesLadders(postion); // goes through a void if it hit a snakes or a ladder.
                P3POS.Text = "" + P3Position;
                P3PIC.Location = Coordinates[Convert.ToInt32(P3POS.Text)]; // converts the reciceved data into int32 then adds into location
                Controls.Add(P3PIC); // adds into location
                P3PIC.BringToFront(); // keeps the image on top of the map/
                P3PIC.SizeMode = PictureBoxSizeMode.StretchImage; // to keep the image inside the picturebox.
                if (P3Position >= 100)
                {
                    TheWin(lblPlayer3.Text);
                }
            }

            if (result.StartsWith("\n/P4")) // this is for syncronsation (recieves the players positions)
            {
                string resiv = (Encoding.ASCII.GetString(dataBuf).Substring(4)); // gets rid of the first 4 of the string (\m/p1) to keep only 003 for example = player poistion.
                int postion = Convert.ToInt32(resiv.Substring(0, 3)); // cuts the values so it only gets the position. // somestimes it connects with other databufs.
                P4Position = SnakesLadders(postion); // goes through a void if it hit a snakes or a ladder.
                P4POS.Text = "" + P4Position;
                P4PIC.Location = Coordinates[Convert.ToInt32(P4POS.Text)]; // converts the reciceved data into int32 then adds into location
                Controls.Add(P4PIC);  // adds into location
                P4PIC.BringToFront(); // keeps the image on top of the map/
                P4PIC.SizeMode = PictureBoxSizeMode.StretchImage; // to keep the image inside the picturebox.
                if (P4Position >= 100)
                {
                    TheWin(lblPlayer4.Text);
                }
            }

            else if (result.StartsWith("\nL")) // if it starts with /nL which = to list of users usernames. 
            {
                string[] listbox = null; // get an array to list all 4 players into a listbox.
                String Player1 = null;
                String Player2 = null;
                String Player3 = null;
                String Player4 = null;

                listbox = (result.Substring(2).Split('|')); // server added | between all players so it is splitable onces it reaches the server

                if (result.Contains("|")) // if it contains one split = player 1 | player 2 = then 2 playera are online.
                {
                    listBox1.Items.Clear(); // stops it from adding onto the listbox.
                    Player1 = listbox[0]; // lists players with their username on listbox.
                    Player2 = listbox[1]; // lists players with their username on listbox.
                }
                else if (Regex.Matches(result, "|").Count == 2) // if it has 2 | splits then it = Player 1 | Player 2 | Player 3
                {
                    listBox1.Items.Clear(); // stops it from adding onto the listbox.
                    Player1 = listbox[0]; // lists players with their username on listbox.
                    Player2 = listbox[1]; // lists players with their username on listbox.
                    Player3 = listbox[2]; // lists players with their username on listbox.
                }
                else if (Regex.Matches(result, "|").Count == 3) // if it has 3 | splits then it = Player 1 | Player 2 | Player 3 | Player 4
                {
                    listBox1.Items.Clear();
                    Player1 = listbox[0]; // lists players with their username on listbox.
                    Player2 = listbox[1]; // lists players with their username on listbox.
                    Player3 = listbox[2]; // lists players with their username on listbox.
                    Player4 = listbox[3]; // lists players with their username on listbox.
                }
                else
                {
                    listBox1.Items.Clear(); //if their is no splits then their is 1 player.
                    Player1 = listbox[0]; // lists players with their username on listbox.
                } 
                listBox1.Items.AddRange(listbox); // onces it has compliled into a if statement then it will add all them into one listbox.
                }
                else
                {
                 String bin = ("\n"+Encoding.ASCII.GetString(dataBuf)); // if a command we didn't set has received it will store into bin
                // this command could also be tested to see what we are resiviving. // just set into the richtextbox.
               }
        }


        //recieveing the advertisement broadcast

        void Start_ListenForServerAdvertisement_Timer()  // this timer is activated once the refresh button is clicked which will search for any bytes are sent.
        {
            m_ListenForserverAdvertisement_Timer = new System.Windows.Forms.Timer(); // creates an timer.
            m_ListenForserverAdvertisement_Timer.Interval = 100; // milliseconds // how fast its repeating. 
            m_ListenForserverAdvertisement_Timer.Tick += OnTimedEvent_ListenForserverAdvertisement_Timer; // this method is called which will receive.
            m_ListenForserverAdvertisement_Timer.Enabled = true; // timer is set enabled.
        }

        void OnTimedEvent_ListenForserverAdvertisement_Timer(Object myObject, EventArgs myEventArgs)  
        {
            try
            {
                DoReceive_ServerAdvertisement_broadcast(); //method is called
            }
            catch // Silently catch any errors
            {
                statusLabel.Text = "Error Catching for Listener"; // if methods doesn;t call show this error.
            }
        }


        void DoReceive_ServerAdvertisement_broadcast() // Invoked by timer
        {
            byte[] bArray = new byte[m_iSA_MessageSize];
            int iReceiveByteCount = 0; // 
            try
            {
                iReceiveByteCount = m_ListenForServerAdvertisementsSocket.Receive(bArray); // if you have recieved any array then send into method.
            }
            catch (SocketException)
            {
                return;
            }

            if (0 == iReceiveByteCount) //if recieved no bytes then return back and retry.
            {
                return;
            }

            ServerAdvertise_Message_PDU Message = DeSerialize_ReceivedServerAdvertisementMessage(bArray);

            if (MessageType.SERVER_ADVERTISE == Message.eMessageType)
            {
                label3.Text = "Server discovered"; // once a server is found then show this lable.
                string sServerName = "";
                int ilength = ByteArrayTo_ASCII_String(Message.cServerName, ref sServerName);
                
                txtServerName.Text = sServerName; // this will be the server name which has been sent through the udp host.

                // Set Server's Address  
                byte[] bAddressArray = new byte[4];
                bAddressArray[0] = Message.iIP_Address_Byte_1; //the first 3 numbers of the port. // this is an array 
                bAddressArray[1] = Message.iIP_Address_Byte_2; //the 2nd part of the ip // this is an array 
                bAddressArray[2] = Message.iIP_Address_Byte_3; //3rd part of ip // this is an array 
                bAddressArray[3] = Message.iIP_Address_Byte_4; //4th part of ip  // this is an array 
                Int16 iPort = (Int16)Convert.ToInt32(Message.iPort); // converts port to in16
                IPAddress ipHostAddress = new IPAddress(bAddressArray); // converts ipaddress into ipadress.
                m_ConnectEndPoint = new System.Net.IPEndPoint(ipHostAddress, iPort);

                // Display Server's Address
                string ip1 = Message.iIP_Address_Byte_1.ToString(); //the ip is diveded into 4 parts this is one of the part
                string ip2 = Message.iIP_Address_Byte_2.ToString(); //the ip is diveded into 4 parts this is one of the part
                string ip3 = Message.iIP_Address_Byte_3.ToString(); //the ip is diveded into 4 parts this is one of the part
                string ip4 = Message.iIP_Address_Byte_4.ToString(); //the ip is diveded into 4 parts this is one of the part
                string port = Message.iPort.ToString(); //port which sends 0;

                txtIP.Text = ip1 + "." + ip2 + "." + ip3 + "." + ip4; // the whole of this would create an ip which has been sent from the host.
                txtPort.Text = "8000"; // we had to set port default as 8000, as we was not able to send the port to each other.
            }
        }

        static Int32 ByteArrayTo_ASCII_String(byte[] bArray, ref string sStr) // Returns length (to null terminator)
        {   // Convert byte array into ASCII string, one char per byte
            Int32 iASCIIz_length = 0; // Length to first zero (null termination)
            //Encoding.ASCII.GetBytes(sStr);
            sStr = System.Text.Encoding.UTF8.GetString(bArray);
            Int32 iStoragelength = sStr.Length;
            for (int iIndex = 0; iIndex < iStoragelength; iIndex++)
            {
                if (0 == bArray[iIndex])
                {   // Check for single-byte Zero character '0'
                    iASCIIz_length = iIndex;
                    break;
                }
            }
            return iASCIIz_length;
        }

        public ServerAdvertise_Message_PDU DeSerialize_ReceivedServerAdvertisementMessage(byte[] bRecdByteArray)
        {   // Converts a byte array (from Receive) to a MyDetailsMessage structure (Fields have fixed length, but non-fixed data length)
            ServerAdvertise_Message_PDU result_Message = new ServerAdvertise_Message_PDU(); 

            int iFieldOffset = 0; // Create an offset value which ensures the byte array is constructed with correct field positions and separations 
            result_Message.eMessageType = (MessageType)BitConverter.ToInt32(bRecdByteArray, iFieldOffset);

            iFieldOffset += m_iSA_FieldSize1;
            result_Message.cServerName = new byte[MAX_SERVER_NAME_LEN + 1];
            for (int iIndex = 0; iIndex < m_iSA_FieldSize2; iIndex++)
            {   // Byte-by-byte copy data from the byte array into the data field  
                {
                    result_Message.cServerName[iIndex] = bRecdByteArray[iIndex + iFieldOffset];
                }
            }

            iFieldOffset += (MAX_SERVER_NAME_LEN + 1);
            result_Message.iIP_Address_Byte_1 = bRecdByteArray[iFieldOffset++];
            result_Message.iIP_Address_Byte_2 = bRecdByteArray[iFieldOffset++];
            result_Message.iIP_Address_Byte_3 = bRecdByteArray[iFieldOffset++];
            result_Message.iIP_Address_Byte_4 = bRecdByteArray[iFieldOffset++];

            iFieldOffset++; // Skip the single byte of padding

            result_Message.iPort = (256 * bRecdByteArray[iFieldOffset])/*1st of the 4 bytes*/ + bRecdByteArray[iFieldOffset + 1]/*2nd of the 4 bytes*/;
            return result_Message;
        }


        public void TheWin(String Player) // when a player wins this sets everything to default and shows another form separtae for the winner.
        {
            Application.Run(new WinnersForm(Player));
            P1Position = 0;
            P2Position = 0;
            P3Position = 0;
            P4Position = 0;
            P1POS.Text = "0";
            P2POS.Text = "0";
            P3POS.Text = "0";
            P4POS.Text = "0";
            P1PIC.Location = Coordinates[Convert.ToInt32(P1PIC.Text)];
            P2PIC.Location = Coordinates[Convert.ToInt32(P2PIC.Text)];
            P3PIC.Location = Coordinates[Convert.ToInt32(P3PIC.Text)];
            P4PIC.Location = Coordinates[Convert.ToInt32(P4POS.Text)];
            messagesbox.Clear();
            String szData = "/STOP";
            byte[] byData = System.Text.Encoding.ASCII.GetBytes(szData);
            m_ClientSocket.Send(byData, SocketFlags.None);
        }


        public void turnoffPlayers() //methods which closes all of the list and icons for all players.
        {
            picPlayer1.Visible = false;
            picPlayer2.Visible = false;
            picPlayer3.Visible = false;
            picPlayer4.Visible = false;

            lblPlayer1.Visible = false;
            lblPlayer2.Visible = false;
            lblPlayer3.Visible = false;
            lblPlayer4.Visible = false;

            P1POS.Visible = false;
            P2POS.Visible = false;
            P3POS.Visible = false;
            P4POS.Visible = false;

            lblP1lbl.Visible = false;
            lblP2lbl.Visible = false;
            lblP3lbl.Visible = false;
            lblP4lbl.Visible = false;
        } 

        

        public int SnakesLadders(int pos) // this method will get an input, and if this input = to a value which = snakes or ladder it will output their new position. 
        { //with a message
            if (pos == 3)
            {
                announcements.AppendText("\nYou have Found a ladder on position 3, which led you to position 21");
                return pos = 21;  
            }
            if (pos == 8)
            {
                announcements.AppendText("\nYou have Found a ladder on position 8, which led you to position 30");
                return pos = 30;
            }
            if (pos == 17)
            {
                announcements.AppendText("\nYou been captured by a snake, but you have managed to escape position 13");
                return pos = 13;
            }
            if (pos == 28)
            {
                announcements.AppendText("\nYou have Found a ladder on position 28, which led you to position 84");
                return pos = 84;
            }
            if (pos == 52)
            {
                announcements.AppendText("\nYou been captured by a snake, but you have managed to escape position 29");
                return pos = 29;
            }
            if (pos == 57)
            {
                announcements.AppendText("\nYou been captured by a snake, but you have managed to escape position 40");
                return pos = 40;
            }
            if (pos == 58)
            {
                announcements.AppendText("\nYou have Found a ladder on position 58, which led you to position 77");
                return pos = 77;
            }
            if (pos == 62)
            {
                announcements.AppendText("\nYou been captured by a snake, but you have managed to escape position 62");
                return pos = 22;
            }
            if (pos == 75)
            {
                announcements.AppendText("\nYou have Found a ladder on position 75, which led you to position 86");
                return pos = 86;
            }
            if (pos == 80)
            {
                announcements.AppendText("\nYou have Found a ladder Which glows in gold, which led you to victory(100) ");
                return pos = 100;
            }
            if (pos == 88)
            {
                announcements.AppendText("\nYou been captured by a snake, but you have managed to escape position 18");
                return pos = 18;
            }
            if (pos == 90)
            {
                announcements.AppendText("\nYou have Found a ladder on position 90, which led you to position 91");
                return pos = 91;
            }
            if (pos == 95)
            {
                announcements.AppendText("\nYou been captured by a snake, but you have managed to escape position 51");
                return pos = 51;
            }
            if (pos == 97)
            {
                announcements.AppendText("\nYou been captured by a snake, but you have managed to escape position 79");
                return pos = 79;
            }
            return pos; // if the pos doesn't match then return their value.
        }


        private void button5_Click(object sender, EventArgs e)
        {
            label3.Visible = true;
            txtPort.Text = "8000";
            if (string.IsNullOrWhiteSpace(usernametxt.Text))
            {
                label3.Text = "Please Enter a Username";
            }
            else
            {

                label3.Text = "Connecting";
                myTimer.Interval = 5000;

                {   // Connect the Socket with a remote endpoint
                    try
                    {
                        // Create the socket, for TCP use
                        m_ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        m_ClientSocket.Blocking = true; // Socket operates in Blocking mode initially
                        label3.Text = "Succesfully Connected";
                        connectbtn.Enabled = false;
                        leavebtn.Enabled = true;
                        sendbtn.Enabled = true;
                    }
                    catch // Handle any exceptions
                    {
                        ; label3.Text = "Failed";
                        leavebtn.Enabled = false;
                        sendbtn.Enabled = false;
                        connectbtn.Enabled = true;
                        Close_Socket_and_Exit();
                    }
                    try
                    {
                        // Get the IP address from the appropriate text box
                        String szIPAddress = txtIP.Text;
                        System.Net.IPAddress DestinationIPAddress = System.Net.IPAddress.Parse(szIPAddress);

                        //Start timer
                        myTimer.Start();

                        // Get the Port number from the appropriate text box
                        String szPort = txtPort.Text;
                        int iPort = System.Convert.ToInt16(szPort, 10);

                        // Combine Address and Port to create an Endpoint
                        m_remoteEndPoint = new System.Net.IPEndPoint(DestinationIPAddress, iPort);


                        m_ClientSocket.Connect(m_remoteEndPoint);
                        m_ClientSocket.Blocking = false;    // Socket is now switched to Non-Blocking mode for send/ receive activities
                        leavebtn.Enabled = false;
                        m_CommunicationActivity_Timer.Start();  // Start the timer to perform periodic checking for received 
                        m_ClientSocket.BeginReceive(receivedBuf, 0, receivedBuf.Length, SocketFlags.None, new AsyncCallback(ReceiveData), m_ClientSocket);
                        byte[] buffer = Encoding.ASCII.GetBytes("/User:" + usernametxt.Text);
                        m_ClientSocket.Send(buffer);
                        label3.Text = "Succesfully Connected";
                        GetPlayerID.Start();
                    }
                    catch // Catch all exceptions
                    {   // If an exception occurs, display an error message
                        label3.Visible = true;
                        label3.Text = "Connecting Failed";
                        leavebtn.Enabled = false;
                        sendbtn.Enabled = false;
                        connectbtn.Enabled = true;
                    }
                    
                }
            }
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                sendbtn.PerformClick();
                sendmsgtxt.Text = "";
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        public void getID()
        {
            try
            {
                String szData = "/iInx:";
                byte[] byData = System.Text.Encoding.ASCII.GetBytes(szData);
                m_ClientSocket.Send(byData, SocketFlags.None);
            }
            catch // Silently handle any exceptions
            {
            }
        }

        private void sendbtn_Click(object sender, EventArgs e)
        {
            if (sendmsgtxt.Text == "")
            {
                MessageBox.Show("You Can't Send an Empty Message.");
                return;
            }
                try
                {
                String szData = "/Mesg:" + sendmsgtxt.Text;
                if (szData.Equals(""))
                {
                    szData = "Default message";
                }
                byte[] byData = System.Text.Encoding.ASCII.GetBytes(szData);
                m_ClientSocket.Send(byData, SocketFlags.None);
                //richTextBox1.AppendText("\nUser: " + textBox3.Text);
                sendmsgtxt.Text = "";
            }
            catch // Silently handle any exceptions
            {
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            messagesbox.SelectionStart = messagesbox.Text.Length;
            messagesbox.ScrollToCaret();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
                try
                {
                    m_ListenForServerAdvertisementsSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    m_ListenForServerAdvertisementsSocket.Blocking = false;
                }
                catch (SocketException)  // Catch exceptions from the socket operations
                {
                    MessageBox.Show("CreateSocket() failed", "'Noughts and Crosses' Client");
                    //CloseSocketAndExit();
                }

                // Set up local address structure
                InitialiseLocalAddressStructAndPort(); // For listening for Server Advertisement broadcasts

                // Bind   *** NOTE THAT BECAUSE THE CLIENT BINDS TO A SPECIFIC PORT, THIS SOLUTION ONLY PERMITS ONE CLIENT INSTANCE TO RUN AT EACH COMPUTER ***
                try
                {
                    m_ListenForServerAdvertisementsSocket.Bind(m_ReceiveServerAdvertisementsEndPoint);
                btnRefresh.Enabled = false;
                }
                catch (SocketException)  // Catch exceptions from the socket operations
                {
                    MessageBox.Show("Bind failed !\nPort is probably in use", "'Snakes and Ladders");
                    //CloseSocketAndExit();
                }
                //once the server is setted up now its time to broadcast ip to local ip address.
                Start_ListenForServerAdvertisement_Timer(); // Enable ServerAdvertisement_Reception (see timer logic)
            
        }
        void InitialiseLocalAddressStructAndPort()
        {	// Initialise Local IP address and Port number for listening for Server Advertisement messages
            IPAddress ReceiveServerAdvertisementsAddress = GetLocalHostAddressDetails();
            m_ReceiveServerAdvertisementsEndPoint = new IPEndPoint(ReceiveServerAdvertisementsAddress, SERVER_ADVERTISE_PORT);
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
                    return ipHostAddress[iAddrIndex]; // Return the first IPv4 address found
                }
            }
            txtIP.Text = "No";
            txtIP.Text = "IP";
            txtIP.Text = "v4";
            txtIP.Text = "Addr";
            IPAddress EmptyIPAddress = new IPAddress(0);
            return EmptyIPAddress;
        }


        private int DiceRoll() //generates a random number beteween 1 -6
        {
            int result;
            Random rnd = new Random();
            result = rnd.Next(1, 7);
            return result;
        }

        private void btnshake_Click(object sender, EventArgs e)
        {
            if (iIndexSHOW.Text == "1") // iIndex will be different for all the connected if its 1 then move player 1.
            {
                dicenumber.Text = "" + DiceRoll();
                rolldice(dicenumber.Text);
                P1Position = P1Position + Convert.ToInt32(dicenumber.Text);             
                byte[] byData = System.Text.Encoding.ASCII.GetBytes("/P1"+ P1Position.ToString("000") + " ");
                m_ClientSocket.Send(byData, SocketFlags.None);
            }
            else if (iIndexSHOW.Text == "2") // iIndex will be different for all the connected if its 2 then move player 2.
            {
                dicenumber.Text = "" + DiceRoll();
                rolldice(dicenumber.Text);
                P2Position = P2Position + Convert.ToInt32(dicenumber.Text);
                byte[] byData = System.Text.Encoding.ASCII.GetBytes("/P2" + P2Position.ToString("000") + " ");
                m_ClientSocket.Send(byData, SocketFlags.None);
            }
            else if (iIndexSHOW.Text == "3") // iIndex will be different for all the connected if its 3 then move player 3.
            {
                dicenumber.Text = "" + DiceRoll();
                rolldice(dicenumber.Text);
                P3Position = P3Position + Convert.ToInt32(dicenumber.Text);
                byte[] byData = System.Text.Encoding.ASCII.GetBytes("/P3" + P3Position.ToString("000") + " ");
                m_ClientSocket.Send(byData, SocketFlags.None);
            }
             
            else if (iIndexSHOW.Text == "4") // iIndex will be different for all the connected if its 4 then move player 4.
            {
                dicenumber.Text = "" + DiceRoll();
                P4Position = P4Position + Convert.ToInt32(dicenumber.Text);
                byte[] byData = System.Text.Encoding.ASCII.GetBytes("/P4" + P4Position.ToString("000") + " ");
                m_ClientSocket.Send(byData, SocketFlags.None);
            }
            btnshake.Enabled = false;
        }

        public void rolldice(string dice) // when they roll a dice. 
        {
                String szData = "/Dice:" + dicenumber.Text + " ";
                byte[] byData = System.Text.Encoding.ASCII.GetBytes(szData);
                m_ClientSocket.Send(byData, SocketFlags.None);
        }

        private void timer1_Tick(object sender, EventArgs e) //this timer is to keep whos online on the server.
        {
            PlayerID = true;
            try
            {
                String szData = "/List:";
                byte[] byData = System.Text.Encoding.ASCII.GetBytes(szData);
                m_ClientSocket.Send(byData, SocketFlags.None);
            }
            catch { }
        }
        private void GetPlayerID_Tick(object sender, EventArgs e)
        {
            getID(); 
            GetPlayerID.Stop();
        }

        private void announcements_TextChanged(object sender, EventArgs e)
        {
            announcements.SelectionStart = announcements.Text.Length;
            announcements.ScrollToCaret();
        }

        private void leavebtn_Click(object sender, EventArgs e)
        {
            //disconnect 
        }
        public String FileName(string path) // a method to find folders inside the project.
        {
            string projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            string File = Path.Combine(projectPath, "" + path);
            System.IO.Directory.CreateDirectory(File);
            return File;
        }

        public void PlayMusic(string status) // this method will turn on and off the music depending on what input they write inside the method when its called.
        {
            SoundPlayer player = new SoundPlayer(FileName("SoundEffects") + "/GameStart.wav"); //opens the project file where the gamestart wav file is found.
            if (status == "On") // if the users presses to turn on the music then it will call "on"
            {
                player.PlayLooping(); //plays the music
            }
            else
            {
                player.Stop(); //stops the music
            }
        }

        private void button1_Click(object sender, EventArgs e) //this button click is made so the user is able to mute the music and reopen.
        {
            if (Music == true)
            { 
                PlayMusic("Off");
                Music = false;
                button1.BackgroundImage = Image.FromFile(FileName("icons") + "/Onn.png");
                button1.BackgroundImageLayout = ImageLayout.Stretch;
            }
            else
            {
                PlayMusic("On");
                button1.BackgroundImage = Image.FromFile(FileName("icons") + "/Off.png");
                button1.BackgroundImageLayout = ImageLayout.Stretch;
                Music = true;
            }

        }

        private void LOBBY_Load_1(object sender, EventArgs e) //when the lobby has loaded it will turn on background music.
        {
            PlayMusic("On");
        }
    }
    }

