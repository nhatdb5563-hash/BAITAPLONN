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
        TcpListener server; //
        Thread serverThread;
        bool serverRunning = false;

        class ClientInfo
        {
            public TcpClient Client;
            public string Username;
        }

        List<ClientInfo> clients = new List<ClientInfo>();

        // ========== KHOI TAO FORM=========

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

        // ================= SERVER-START ================

        private void StartServer(int port)
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();

            serverRunning = true;

            serverThread = new Thread(ServerListen);
            serverThread.IsBackground = true;
            serverThread.Start();
        }
        // ================= SERVER-LISTEN ================
        private void ServerListen()
        {
            while (serverRunning)
            {
                try
                {
                    TcpClient newClient = server.AcceptTcpClient();
                    //=============TAO LUONG CHO CLIENT CHAY ( 1 CLIENT=1 THREAD)===================
                    Thread t = new Thread(() => HandleClient(newClient));
                    t.IsBackground = true;
                    t.Start();
                }
                catch { }
            }
        }
        // ================= SERVER-HANDLE CLIENT (client connect -> server doc ngay) ================
        private void HandleClient(TcpClient tcpClient)
        {
            NetworkStream stream = tcpClient.GetStream(); // getstream de doc/ghi du lieu
            byte[] buffer = new byte[4096]; // buffer chua du lieu nhan duoc 
            StringBuilder sb = new StringBuilder(); // ghep message lai
            string username = "";

            try
            {
                // doc username dau tien client gui len 
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                username = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                //=================== CHECK TRÙNG USERNAME==================
                if (clients.Any(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                //Any kiem tra co phan tu nao thoa dk
                // equals so sanh username cu voi username moi
                //StringComparison.OrdinalIgnoreCase so sanh k phan biet hoa thuong 
                {
                    // trung thi bao loi ngat ket noi 
                    byte[] data = Encoding.UTF8.GetBytes("USERNAME_TAKEN<END>");
                    stream.Write(data, 0, data.Length);
                    tcpClient.Close();
                    return;
                }
                // add client vao danh sach 
                clients.Add(new ClientInfo { Client = tcpClient, Username = username });
                BroadcastUserList(); // gui danh sach user cho all client 
                
                // vong lap nhan message 
                while (serverRunning)
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                    // vong lap xu li message hoan chinh ket thuc bang END 
                    while (sb.ToString().Contains("<END>"))
                    {
                        string full = sb.ToString();
                        int idx = full.IndexOf("<END>"); //Tim vi tri ket thuc message 
                        string msg = full.Substring(0, idx); //cat ms hoan chinh
                        sb.Remove(0, idx + 5); //xoa phan da xu li khoi buffer 

                        ProcessServerMessage(msg, tcpClient); 
                    }
                }
            }
            catch { }
            finally
            {
                var c = clients.Find(x => x.Client == tcpClient);
                // tim client trong danh sach
                if (c != null)
                {
                    clients.Remove(c); // xoa client khi disconnect 
                    BroadcastUserList(); // cap nhat lai ds
                }

                tcpClient.Close();
            }
        }
        // ================= SERVER-XU LI MESSAGE ================
        private void ProcessServerMessage(string msg, TcpClient tcpClient)
        {
            if (msg.StartsWith("TO|")) //Neu la tin nhan rieng
            {
                string[] p = msg.Split('|'); // tach chuoi thanh mang p[i]
                var sender = clients.Find(x => x.Client == tcpClient); // tim ng gui
                var receiver = clients.Find(x => x.Username == p[1]); // p[1] tim ng nhan theo username 

                if (sender != null && receiver != null)
                {
                    string send = $"[PRIVATE] {sender.Username}: {p[2]}<END>"; //p[2] *
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
        // ================= SERVER-BROADCAST ================
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
        // ================= SERVER-BUTTON START ================
        private void button_StartService_Click(object sender, EventArgs e)
        {
            StartServer((int)numericUpDown1.Value);
            button_StartService.Enabled = false;
            button_StopService.Enabled = true;
        }
        // ================= SERVER-BUTTON STOP ================
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

        // ===== CLIENT-KHAI BAO =====
        TcpClient client;
        NetworkStream clientStream;
        Thread clientThread;
        bool clientRunning = false;

        string myUsername = "";
        StringBuilder clientBuffer = new StringBuilder();

        // ================= CLIENT-CONNECT =================

        private void ConnectServer(string ip, int port)
        {
            client = new TcpClient();
            client.Connect(IPAddress.Parse(ip), port);

            clientStream = client.GetStream();
            clientRunning = true;

            myUsername = TextNhapUsername.Text;
            clientStream.Write(Encoding.UTF8.GetBytes(myUsername), 0, myUsername.Length);

            clientThread = new Thread(ClientListen);
            clientThread.IsBackground = true;
            clientThread.Start();

            button_Connect.Enabled = false;
            button_Disconnect.Enabled = true;
        }
        // ================= CLIENT-LISTEN =================
        private void ClientListen()
        {
            byte[] buffer = new byte[4096];

            try
            {
                while (clientRunning)
                {
                    int bytesRead = clientStream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    clientBuffer.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

                    while (clientBuffer.ToString().Contains("<END>"))
                    {
                        string full = clientBuffer.ToString();
                        int idx = full.IndexOf("<END>");
                        string msg = full.Substring(0, idx);
                        clientBuffer.Remove(0, idx + 5);

                        HandleMessage(msg);
                    }
                }
            }
            catch { }
            finally
            {
                this.Invoke(new Action(() =>
                {
                    button_Connect.Enabled = true;
                    button_Disconnect.Enabled = false;
                    listBox1.Items.Clear(); //  clear khi mất kết nối
                }));
            }
        }
        // ================= CLIENT-HANDLE MESSAGE  =================
        private void HandleMessage(string msg)
        {
            if (msg == "USERNAME_TAKEN")
            {
                this.Invoke(new Action(() =>
                {
                    MessageBox.Show("Username đã tồn tại!");
                    button_Connect.Enabled = true;
                    button_Disconnect.Enabled = false;
                }));

                clientRunning = false;
                client?.Close();
            }
            else if (msg.StartsWith("USERLIST|"))
            {
                string[] users = msg.Replace("USERLIST|", "").Split(',');

                listBox1.Invoke(new Action(() =>
                {
                    listBox1.Items.Clear();
                    foreach (var u in users)
                        listBox1.Items.Add(u);
                }));
            }
            else if (msg.StartsWith("FILE|"))
            {
                string[] p = msg.Split('|');
                byte[] fileData = Convert.FromBase64String(p[3]);

                this.Invoke(new Action(() =>
                {
                    DialogResult r = MessageBox.Show($"{p[1]} gửi file: {p[2]}", "Nhận file", MessageBoxButtons.YesNo);

                    if (r == DialogResult.Yes)
                    {
                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.FileName = p[2];

                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            System.IO.File.WriteAllBytes(sfd.FileName, fileData);
                        }
                    }
                }));
            }
            else
            {
                richTextBox1.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText(msg + Environment.NewLine);
                }));
            }
        }
        // ========================== CLIENT - BUTTON CONNECT ==========================
        private void button_Connect_Click(object sender, EventArgs e)
        {
            ConnectServer(IPServer.Text, (int)Port.Value);
        }
        // ========================== CLIENT -BUTTON DISCONNECT ==========================
        private void button_Disconnect_Click(object sender, EventArgs e)
        {
            clientRunning = false;
            try { client?.Close(); } catch { }

            listBox1.Items.Clear(); // clear user list

            button_Connect.Enabled = true;
            button_Disconnect.Enabled = false;
        }
        // ========================== CLIENT - BUTTON SEND  ==========================
        private void btnSend_Click(object sender, EventArgs e)
        {
            string toUser = listBox1.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(toUser)) return;
            //rang buoc khong gui tin  nhan cho chinh minh
            if (toUser == myUsername)
            {
                MessageBox.Show("Không thể gửi tin nhắn cho chính mình!");
                return;
            }
            string msg = $"TO|{toUser}|{textBox1.Text}<END>";
            byte[] data = Encoding.UTF8.GetBytes(msg);
            clientStream.Write(data, 0, data.Length);

            textBox1.Clear();
        }
        // ========================== CLIENT - SENFILE ==========================
        private void btnSendFile_Click(object sender, EventArgs e)
        {
            string toUser = listBox1.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(toUser)) return;

            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                byte[] fileData = System.IO.File.ReadAllBytes(ofd.FileName);
                string base64 = Convert.ToBase64String(fileData);
                string fileName = System.IO.Path.GetFileName(ofd.FileName);

                string msg = $"FILE|{toUser}|{fileName}|{base64}<END>";
                byte[] data = Encoding.UTF8.GetBytes(msg);

                clientStream.Write(data, 0, data.Length);
            }
        }
        // ========================== CLIENT - BUTTON CLEAR  ==========================
        private void btnClear_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }
        // ========================== CLIENT - BUTTON EXIT ==========================
        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}