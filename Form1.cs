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
                    string send = $"[PRIVATE] {sender.Username}: {p[2]}<END>"; // dinh dang tin nhan 
                    byte[] data = Encoding.UTF8.GetBytes(send); // chuyen chuoi thanh mang  gui qua mang theo chuan UTF-8 de gui dl qa mang bang networkStream
                    receiver.Client.GetStream().Write(data, 0, data.Length);// gui cho ng nhan
                    sender.Client.GetStream().Write(data, 0, data.Length);// gui lai cho ng gui (hien thi)
                }
            }
            //kiem tra gui file 
            else if (msg.StartsWith("FILE|"))
            {
                string[] p = msg.Split('|'); 
                var sender = clients.Find(x => x.Client == tcpClient); // tim ng gui
                var receiver = clients.Find(x => x.Username == p[1]); //p[1] tim ng nhan

                if (sender != null && receiver != null) //kiem tra ng gui/nhan co ton tai k
                {
                    string send = $"FILE|{sender.Username}|{p[2]}|{p[3]}<END>"; 
                    // gui file
                    // p[2] ten file
                    // p[3] nd file 
                    byte[] data = Encoding.UTF8.GetBytes(send);
                    receiver.Client.GetStream().Write(data, 0, data.Length);
                    // gui file dang base 64
                }
            }
        }
        // ================= SERVER-BROADCAST ================
        private void BroadcastUserList()
        {
            // tao chuoi ds user
            string list = "USERLIST|" + string.Join(",", clients.Select(x => x.Username)) + "<END>";
            byte[] data = Encoding.UTF8.GetBytes(list); //chuyen str sang byte de gui qa networkStream
            // gui cho tat ca client 
            foreach (var c in clients.ToList())
            {
                try { c.Client.GetStream().Write(data, 0, data.Length); } catch { }
            }
            //cap nhat UI Server
            listBox1.Invoke(new Action(() =>
            {
                listBox1.Items.Clear();
                foreach (var c in clients)
                    listBox1.Items.Add(c.Username);
            }));
        }
        // ================= SERVER-BUTTON START ================
        private void button_StartService_Click(object sender, EventArgs e) //ham skien
        {
            // rang buoc khong nhan lai khi startservice
            StartServer((int)numericUpDown1.Value);
            button_StartService.Enabled = false;
            button_StopService.Enabled = true;
        }
        // ================= SERVER-BUTTON STOP ===============
        private void button_StopService_Click(object sender, EventArgs e)
            //bam stop ctrinh dung ngat all client ,xoa ds user va rset lai nut giao dien 
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
        // cac bien ket noi, gui/nhan dlieu và xu li message 
        TcpClient client; 
        NetworkStream clientStream; 
        Thread clientThread; // luong nhan dlieu
        bool clientRunning = false; // trang thai client 

        string myUsername = "";
        StringBuilder clientBuffer = new StringBuilder(); //StringBuilder ghep message tcp

        // ================= CLIENT-CONNECT =================
        //ket noi client toi server, gui username va nhan dlieu
        private void ConnectServer(string ip, int port)
        {
            client = new TcpClient(); 
            client.Connect(IPAddress.Parse(ip), port);

            clientStream = client.GetStream();
            clientRunning = true;

            myUsername = TextNhapUsername.Text; // lay username tu textbox
            clientStream.Write(Encoding.UTF8.GetBytes(myUsername), 0, myUsername.Length);
            //gui username len server 
            clientThread = new Thread(ClientListen);
            clientThread.IsBackground = true;
            clientThread.Start();
            // rang buoc k nhan lai neu da bat
            button_Connect.Enabled = false;
            button_Disconnect.Enabled = true;
        }
        // ================= CLIENT-LISTEN =================
        //nhan dlieu, ghep thanh message hoan chinh va gui di xu li 
        private void ClientListen()
        {
            byte[] buffer = new byte[4096];

            try
            {
                while (clientRunning)
                {
                    //doc dlieu =0 thì break 
                    int bytesRead = clientStream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;
                    //ghep dlieu 
                    clientBuffer.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead)); //ghep dlieu 

                    while (clientBuffer.ToString().Contains("<END>"))
                    {
                        string full = clientBuffer.ToString();
                        int idx = full.IndexOf("<END>");
                        string msg = full.Substring(0, idx);
                        clientBuffer.Remove(0, idx + 5);

                        HandleMessage(msg);// xu li message 
                    }
                }
            }
            catch { }
            finally
            {
                this.Invoke(new Action(() => //invoke cap nhap UI khi mat ket noi 
                {
                    button_Connect.Enabled = true;
                    button_Disconnect.Enabled = false;
                    listBox1.Items.Clear(); //  clear khi mất kết nối
                }));
            }
        }
        // ================= CLIENT-HANDLE MESSAGE  =================
        //xu li phan hien thi,nhan tu server, gui toi UI
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
                //nhan file 
            {
                string[] p = msg.Split('|');
                //p[1] ng gui
                //p[2] fileName 
                //p[3] base64 
                byte[] fileData = Convert.FromBase64String(p[3]);
                //chuyen base64 -> dlieu file 
                this.Invoke(new Action(() =>
                {
                    DialogResult r = MessageBox.Show($"{p[1]} gửi file: {p[2]}", "Nhận file", MessageBoxButtons.YesNo);
                    if (r == DialogResult.Yes)
                    {
                        SaveFileDialog sfd = new SaveFileDialog();// chon noi luu
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
                    richTextBox1.AppendText(msg + Environment.NewLine);//hie thi tin nhan 
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
        // ========================== CLIENT - SENDFILE ==========================
        // skien chon file, chuyen file sang base 64,dong goi thanh message theo format FIle roi chuyen toi ng nhan 
        // ReadAllBytes() → doc file 
        //ToBase64String() → chuyen file thanh chuoi
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