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
using System.Threading;

namespace ChattingClient
{
    public partial class ChattingClient : Form
    {
        Socket client_socket;
        bool isConnected;
        byte[] bytes = new byte[1024];
        string data;

        public ChattingClient()
        {
            InitializeComponent();
            isConnected = false;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (isConnected == false)
                return;
            byte[] msg = Encoding.UTF8.GetBytes(tbText.Text + "<eof>");
            int bytesSent = client_socket.Send(msg);
            tbText.Clear();
            tbText.Text = "";
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (isConnected == true)
                return;
            client_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client_socket.Connect(new IPEndPoint(IPAddress.Parse(tbIP.Text), 7777));
            lbChattingHistory.Items.Add(String.Format("Socket is Connected."));
            isConnected = true;
            Thread listen_thread = new Thread(do_received);
            listen_thread.Start();
        }
        void do_received() {
            while (isConnected) {
                while (true) {
                    byte[] bytes = new byte[1024];
                    int bytesRec = client_socket.Receive(bytes);
                    data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                    if (data.IndexOf("<eof>") > -1)
                        break;
                }
                data = data.Substring(0, data.Length - 5);
                Invoke((MethodInvoker)delegate {
                    lbChattingHistory.Items.Add(data);
                });
                data = "";
            }
        }
    }
}
