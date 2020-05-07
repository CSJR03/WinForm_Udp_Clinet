using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        bool alive = false;

        UdpClient client;

        const int LocalPort = 8001;
        const int RemotePort = 8001;
        const int TTL = 20;

        const string Host = "235.5.5.1";
        IPAddress groupAddress;

        string userName;

        public Form1()
        {
            InitializeComponent();

            loginButton.Enabled = true;
            logoutButton.Enabled = false;

            sendButton.Enabled = false;
            chatTextBox.ReadOnly = true;

            groupAddress = IPAddress.Parse(Host);

        }

        void loginButton_Click(object sender , EventArgs e)
        {
            userName = userNameTextBox.Text;
            userNameTextBox.ReadOnly = true;

            try
            {
                client = new UdpClient(LocalPort);
                //
                client.JoinMulticastGroup(groupAddress, TTL);

                //Task
                Task receiveTask = new Task(ReceiveMessage);
                receiveTask.Start();
                //

                string message = userName + " Mutq Gorcec chat";
                byte[] data = Encoding.Unicode.GetBytes(message);
                client.Send(data, data.Length, Host, RemotePort);

                loginButton.Enabled = false;
                logoutButton.Enabled = true;
                sendButton.Enabled = true;


            }
            catch(Exception mes)
            {
                MessageBox.Show(mes.Message);
            }
        }

        void ReceiveMessage()
        {
            alive = true;
            try
            {
                while (alive)
                {
                    IPEndPoint remoteIp = null;

                    byte[] data = client.Receive(ref remoteIp);
                    string message = Encoding.Unicode.GetString(data);

                    this.Invoke(
                        new MethodInvoker(
                            () =>
                            {
                                string time = System.DateTime.Now.ToShortTimeString();
                                chatTextBox.Text = time + " " + message + "\n" + chatTextBox.Text;
                            }

                            ));
                        
                }
            }
            catch(Exception mes)
            {
                MessageBox.Show(mes.Message);
            }

        }

        void sendButton_Click(object sender,EventArgs e)
        {
            try
            {
                string message = $"{userName} : {messageTextBox.Text}";
                byte[] data = Encoding.Unicode.GetBytes(message);

                client.Send(data, data.Length, Host, RemotePort);
                messageTextBox.Clear();
            }
            catch(Exception mes)
            {
                MessageBox.Show(mes.Message);
            }
        }

        void logout_Click(object sender ,EventArgs e)
        {
            ExitChat();
        }

        void ExitChat()
        {
            string message = userName + " lqume chate";

            byte[] data = Encoding.Unicode.GetBytes(message);
            client.Send(data, data.Length, Host, RemotePort);
            client.DropMulticastGroup(groupAddress);

            alive = false;

            client.Close();

            loginButton.Enabled = true;
            logoutButton.Enabled = false;
            sendButton.Enabled = false;

        }

        void Form1_FormClosing(object sender , FormClosingEventArgs e)
        {
            if (alive)
            {
                ExitChat();
            }
        }

    }
}
