using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Timers;

namespace SnakesNLadders___Client
{
    public partial class TCP_Server_Form : Form
    {
   
        private const int m_iMaxConnections = 2;

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

        public TCP_Server_Form()
        {
            InitializeComponent();
            Initialise_ConnectionArray();
            m_CommunicationActivity_Timer = new System.Windows.Forms.Timer(); // Check for communication activity on Non-Blocking sockets every 200ms
            m_CommunicationActivity_Timer.Tick += new EventHandler(OnTimedEvent_PeriodicCommunicationActivityCheck); // Set event handler method for timer
            m_CommunicationActivity_Timer.Interval = 100;  // Timer interval is 1/10 second
            m_CommunicationActivity_Timer.Enabled = false;
            string szLocalIPAddress = GetLocalIPAddress_AsString(); // Get local IP address as a default value
            IP_Address_textBox.Text = szLocalIPAddress;             // Place local IP address in IP address field
            ReceivePort_textBox.Text = "8000";  // Default port number
            m_iNumberOfConnectedClients = 0;
            NumberOfClients_textBox.Text = System.Convert.ToString(m_iNumberOfConnectedClients);
            Listen_button.Enabled = false;
            Accept_button.Enabled = false;
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

        private void Initialise_ConnectionArray()
        {
            int iIndex;
            for (iIndex = 0; iIndex < m_iMaxConnections; iIndex++)
            {
                m_Connection_Array[iIndex].bInUse = false;
            }
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

        private void Bind_button_Click(object sender, EventArgs e)
        {    // Bind to the selected port and start listening / receiving
            try
            {
                // Get the Port number from the appropriate text box
                String szPort = ReceivePort_textBox.Text;
                int iPort = System.Convert.ToInt16(szPort, 10);
                // Create an Endpoint that will cause the listening activity to apply to all the local node's interfaces
                m_LocalIPEndPoint = new System.Net.IPEndPoint(IPAddress.Any, iPort);
                // Bind to the local IP Address and selected port
                m_ListenSocket.Bind(m_LocalIPEndPoint);
                Bind_button.Enabled = false;
                Bind_button.Text = "Bind succeded";
                Listen_button.Enabled = true;
                // Prevent any further changes to the port number
                ReceivePort_textBox.ReadOnly = true;
            }

            catch // Catch any errors
            {   // If an exception occurs, display an error message
                Bind_button.Text = "Bind failed";
            }
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

        private void Listen_button_Click(object sender, EventArgs e)
        {
            try
            {
                Listen_button.Text = "Listening";
                m_ListenSocket.Listen(2); // Listen for connections, with a backlog / queue maximum of 2
                Listen_button.Enabled = false;
                Accept_button.Enabled = true;
            }
            catch (SocketException se)
            {
                // If an exception occurs, display an error message
                MessageBox.Show(se.Message);
            }
            catch // Silently handle any other exception
            {
            }
        }

        private void Accept_button_Click(object sender, EventArgs e)
        {
            m_CommunicationActivity_Timer.Start();  // Start the timer to perform periodic checking for connection requests   
            Accept_button.Text = "Accepting (waiting for connection attempt)";
            Accept_button.Enabled = false;
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
                    m_Connection_Array[iIndex].ClientSpecific_Socket.Shutdown(SocketShutdown.Both);
                    m_Connection_Array[iIndex].ClientSpecific_Socket.Close();
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

        private void OnTimedEvent_PeriodicCommunicationActivityCheck(Object myObject, EventArgs myEventArgs)
        {   // Periodic check whether a connection request is pending or a message has been received on a connected socket     

            // First, check for pending connection requests
            int iIndex;
            iIndex = GetnextAvailable_ConnectionArray_Entry(); // Find an available array entry for next connection request
            if (-1 != iIndex)
            {   // Only continue with Accept if there is an array entry available to hold the details

                try
                {
                    m_Connection_Array[iIndex].ClientSpecific_Socket = m_ListenSocket.Accept();  // Accept a connection (if pending) and assign a new socket to it (AcceptSocket)
                    // Will 'catch' if NO connection was pending, so statements below only occur when a connection WAS pending
                    m_Connection_Array[iIndex].bInUse = true;
                    m_Connection_Array[iIndex].ClientSpecific_Socket.Blocking = false;           // Make the new socket operate in non-blocking mode
                    m_iNumberOfConnectedClients++;
                    NumberOfClients_textBox.Text = System.Convert.ToString(m_iNumberOfConnectedClients);
                    Status_textBox.Text = "A new client connected";

                    SendUpdateMesageToAllConnectedclients();
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

                        string szReceivedMessage;
                        if (0 < iReceiveByteCount)
                        {   // Copy the number of bytes received, from the message buffer to the text control
                            szReceivedMessage = Encoding.ASCII.GetString(ReceiveBuffer, 0, iReceiveByteCount);
                            if ("QuitConnection" == szReceivedMessage)
                            {
                                CloseConnection(iIndex);
                            }
                            else
                            {
                                Message_textBox.Text = szReceivedMessage;
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
                    }
                }
            }
        }

        private void SendUpdateMesageToAllConnectedclients()
        {   // Send message to each connected client informing of the total number of connected clients
            int iIndex;
            for (iIndex = 0; iIndex < m_iMaxConnections; iIndex++)
            {
                if (true == m_Connection_Array[iIndex].bInUse)
                {
                    string szMessage;
                    if (1 == m_iNumberOfConnectedClients)
                    {
                        szMessage = string.Format("There is now {0} client connected", m_iNumberOfConnectedClients);
                    }
                    else
                    {
                        szMessage = string.Format("There are now {0} clients connected", m_iNumberOfConnectedClients);
                    }
                    byte[] SendMessage = System.Text.Encoding.ASCII.GetBytes(szMessage);
                    m_Connection_Array[iIndex].ClientSpecific_Socket.Send(SendMessage, SocketFlags.None);
                }
            }
        }

        private void CloseConnection(int iIndex)
        {
            try
            {
                m_Connection_Array[iIndex].bInUse = false;
                m_Connection_Array[iIndex].ClientSpecific_Socket.Shutdown(SocketShutdown.Both);
                m_Connection_Array[iIndex].ClientSpecific_Socket.Close();
                m_iNumberOfConnectedClients--;
                NumberOfClients_textBox.Text = System.Convert.ToString(m_iNumberOfConnectedClients);
                Status_textBox.Text = "A Connection was closed";
                SendUpdateMesageToAllConnectedclients();
            }
            catch // Silently handle any exceptions
            {
            }
        }
    }
}