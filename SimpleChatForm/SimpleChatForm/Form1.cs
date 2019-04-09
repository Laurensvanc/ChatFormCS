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

namespace SimpleChatForm
{
    public partial class Form1 : Form
    {

        Socket sck;
        EndPoint epLocal, epRemote;
        byte[] buffer;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // prepare socket
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            // optional pre displayed IP
            textLocalIP.Text = GetLocalIP();
            textRemoteIP.Text = GetLocalIP();



        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            // Set socket
            epLocal = new IPEndPoint(IPAddress.Parse(textLocalIP.Text), Convert.ToInt32(textLocalPort.Text));
            sck.Bind(epLocal);


            // Connection with given ip
            epRemote = new IPEndPoint(IPAddress.Parse(textRemoteIP.Text), Convert.ToInt32(textRemotePort.Text));
            sck.Connect(epRemote);

            // Listener
            buffer = new byte[1500];
            sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(messageCallBack), buffer);


        }

        private string GetLocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            }

            return "127.0.0.1";
        }

        private void buttonSend_Click_1(object sender, EventArgs e)
        {
            // Convert string to byte
            ASCIIEncoding aEncoding = new ASCIIEncoding();
            byte[] sendingMessage = new byte[1500];
            sendingMessage = aEncoding.GetBytes(textMessage.Text);

            // Sending the message
            sck.Send(sendingMessage);

            // Visual message
            listMessage.Items.Add("Me: " + textMessage.Text);
            textMessage.Text = "";
        }

        private void messageCallBack(IAsyncResult aResult)
        {
            try
            {
                byte[] recieveData = new byte[1500];
                recieveData = (byte[])aResult.AsyncState;

                // Convert byte to string
                ASCIIEncoding aEncoding = new ASCIIEncoding();
                string recievedMessage = aEncoding.GetString(recieveData);

                // Add string to Chat
                listMessage.Items.Add("Other: " + recievedMessage);

                buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(messageCallBack), buffer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
