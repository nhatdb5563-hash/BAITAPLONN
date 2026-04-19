using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SimpleChat
{
    public partial class Form1 : Form
    {
        // ===== SERVER =====
        TcpListener server;
        Thread serverThread;
        bool serverRunning = false;

        class ClientInfo
        {
            public TcpClient Client;
            public string Username;
        }

        List<ClientInfo> clients = new List<ClientInfo>();


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button_StartService.Enabled = true;
            button_StopService.Enabled = false;

            button_Connect.Enabled = true;
            button_Disconnect.Enabled = false;
        }

        // ================= SERVER =================

        private void StartServer(int port)
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();

            serverRunning = true;

            serverThread = new Thread(ServerListen);
            serverThread.IsBackground = true;
            serverThread.Start();
        }

        private void ServerListen()
        {
            while (serverRunning)
            {
                try
                {
                    TcpClient newClient = server.AcceptTcpClient();
                    Thread t = new Thread(() => HandleClient(newClient));
                    t.IsBackground = true;
                    t.Start();
                }
                catch { }
            }
        }

        private void HandleClient(TcpClient tcpClient)
        {
            NetworkStream stream = tcpClient.GetStream();
            byte[] buffer = new byte[4096];
            StringBuilder sb = new StringBuilder();
            string username = "";

            try
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                username = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                // CHECK TRÙNG USERNAME
                if (clients.Any(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                {
                    byte[] data = Encoding.UTF8.GetBytes("USERNAME_TAKEN<END>");
                    stream.Write(data, 0, data.Length);
                    tcpClient.Close();
                    return;
                }

                clients.Add(new ClientInfo { Client = tcpClient, Username = username });
                BroadcastUserList();

                while (serverRunning)
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

                    while (sb.ToString().Contains("<END>"))
                    {
                        string full = sb.ToString();
                        int idx = full.IndexOf("<END>");
                        string msg = full.Substring(0, idx);
                        sb.Remove(0, idx + 5);

                        ProcessServerMessage(msg, tcpClient);
                    }
                }
            }
            catch { }
            finally
            {
                var c = clients.Find(x => x.Client == tcpClient);
                if (c != null)
                {
                    clients.Remove(c);
                    BroadcastUserList();
                }

                tcpClient.Close();
            }
        }

        private void ProcessServerMessage(string msg, TcpClient tcpClient)
        {
            if (msg.StartsWith("TO|"))
            {
                string[] p = msg.Split('|');
                var sender = clients.Find(x => x.Client == tcpClient);
                var receiver = clients.Find(x => x.Username == p[1]);

                if (sender != null && receiver != null)
                {
                    string send = $"[PRIVATE] {sender.Username}: {p[2]}<END>";
                    byte[] data = Encoding.UTF8.GetBytes(send);

                    receiver.Client.GetStream().Write(data, 0, data.Length);
                    sender.Client.GetStream().Write(data, 0, data.Length);
                }
            }
            else if (msg.StartsWith("FILE|"))
            {
                string[] p = msg.Split('|');
                var sender = clients.Find(x => x.Client == tcpClient);
                var receiver = clients.Find(x => x.Username == p[1]);

                if (sender != null && receiver != null)
                {
                    string send = $"FILE|{sender.Username}|{p[2]}|{p[3]}<END>";
                    byte[] data = Encoding.UTF8.GetBytes(send);
                    receiver.Client.GetStream().Write(data, 0, data.Length);
                }
            }
        }

        private void BroadcastUserList()
        {
            string list = "USERLIST|" + string.Join(",", clients.Select(x => x.Username)) + "<END>";
            byte[] data = Encoding.UTF8.GetBytes(list);

            foreach (var c in clients.ToList())
            {
                try { c.Client.GetStream().Write(data, 0, data.Length); } catch { }
            }

            listBox1.Invoke(new Action(() =>
            {
                listBox1.Items.Clear();
                foreach (var c in clients)
                    listBox1.Items.Add(c.Username);
            }));
        }

        private void button_StartService_Click(object sender, EventArgs e)
        {
            StartServer((int)numericUpDown1.Value);
            button_StartService.Enabled = false;
            button_StopService.Enabled = true;
        }

        private void button_StopService_Click(object sender, EventArgs e)
        {
            serverRunning = false;
            try { server?.Stop(); } catch { }

            foreach (var c in clients.ToList())
            {
                try { c.Client.Close(); } catch { }
            }

            clients.Clear();

            button_StartService.Enabled = true;
            button_StopService.Enabled = false;
        }


    }
}