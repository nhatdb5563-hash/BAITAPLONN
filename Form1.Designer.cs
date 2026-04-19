using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SimpleChat
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            contextMenuStrip1 = new ContextMenuStrip(components);
            richTextBox1 = new RichTextBox();
            button7_Clear = new Button();
            button8_Exit = new Button();
            label5 = new Label();
            textBox1 = new TextBox();
            button_Send = new Button();
            panel1 = new Panel();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            TabClient = new TabPage();
            label1 = new Label();
            TextNhapUsername = new TextBox();
            button_Disconnect = new Button();
            button_Connect = new Button();
            Port = new NumericUpDown();
            label4 = new Label();
            IPServer = new ComboBox();
            label3 = new Label();
            TabServer = new TabPage();
            numericUpDown1 = new NumericUpDown();
            button_StopService = new Button();
            button_StartService = new Button();
            label2 = new Label();
            tabControl1 = new TabControl();
            listBox1 = new ListBox();
            label6 = new Label();
            button_SendFile = new Button();
            panel1.SuspendLayout();
            statusStrip1.SuspendLayout();
            TabClient.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Port).BeginInit();
            TabServer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            tabControl1.SuspendLayout();
            SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new Size(20, 20);
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(61, 4);
            // 
            // richTextBox1
            // 
            richTextBox1.BackColor = SystemColors.ScrollBar;
            richTextBox1.Location = new Point(36, 215);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(547, 220);
            richTextBox1.TabIndex = 5;
            richTextBox1.Text = "";
            // 
            // button7_Clear
            // 
            button7_Clear.BackColor = SystemColors.AppWorkspace;
            button7_Clear.Location = new Point(610, 304);
            button7_Clear.Name = "button7_Clear";
            button7_Clear.Size = new Size(135, 54);
            button7_Clear.TabIndex = 8;
            button7_Clear.Text = "Clear";
            button7_Clear.UseVisualStyleBackColor = false;
            // 
            // button8_Exit
            // 
            button8_Exit.BackColor = SystemColors.AppWorkspace;
            button8_Exit.Location = new Point(610, 381);
            button8_Exit.Name = "button8_Exit";
            button8_Exit.Size = new Size(135, 54);
            button8_Exit.TabIndex = 9;
            button8_Exit.Text = "Exit";
            button8_Exit.UseVisualStyleBackColor = false;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(24, 10);
            label5.Name = "label5";
            label5.Size = new Size(89, 20);
            label5.TabIndex = 10;
            label5.Text = "Text to send";
            // 
            // textBox1
            // 
            textBox1.BackColor = SystemColors.ScrollBar;
            textBox1.Location = new Point(119, 7);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(394, 27);
            textBox1.TabIndex = 11;
            // 
            // button_Send
            // 
            button_Send.BackColor = SystemColors.AppWorkspace;
            button_Send.Location = new Point(535, 6);
            button_Send.Name = "button_Send";
            button_Send.Size = new Size(136, 29);
            button_Send.TabIndex = 12;
            button_Send.Text = "Send";
            button_Send.UseVisualStyleBackColor = false;
            // 
            // panel1
            // 
            panel1.Controls.Add(label5);
            panel1.Controls.Add(textBox1);
            panel1.Controls.Add(button_Send);
            panel1.Location = new Point(37, 470);
            panel1.Name = "panel1";
            panel1.Size = new Size(708, 40);
            panel1.TabIndex = 13;
            // 
            // statusStrip1
            // 
            statusStrip1.BackColor = SystemColors.AppWorkspace;
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 537);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1153, 26);
            statusStrip1.TabIndex = 14;
            statusStrip1.Text = "Status: Connected/Waiting for Connections";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(292, 20);
            toolStripStatusLabel1.Text = "Status: Connected/Waiting for Connections";
            // 
            // TabClient
            // 
            TabClient.BackColor = Color.DarkGray;
            TabClient.Controls.Add(label1);
            TabClient.Controls.Add(TextNhapUsername);
            TabClient.Controls.Add(button_Disconnect);
            TabClient.Controls.Add(button_Connect);
            TabClient.Controls.Add(Port);
            TabClient.Controls.Add(label4);
            TabClient.Controls.Add(IPServer);
            TabClient.Controls.Add(label3);
            TabClient.Location = new Point(4, 29);
            TabClient.Name = "TabClient";
            TabClient.Padding = new Padding(3);
            TabClient.Size = new Size(989, 46);
            TabClient.TabIndex = 1;
            TabClient.Text = "Client";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(425, 13);
            label1.Name = "label1";
            label1.Size = new Size(115, 20);
            label1.TabIndex = 7;
            label1.Text = "Nhap Username";
            // 
            // TextNhapUsername
            // 
            TextNhapUsername.Location = new Point(546, 10);
            TextNhapUsername.Name = "TextNhapUsername";
            TextNhapUsername.Size = new Size(125, 27);
            TextNhapUsername.TabIndex = 6;
            // 
            // button_Disconnect
            // 
            button_Disconnect.BackColor = Color.DarkGray;
            button_Disconnect.Location = new Point(836, 8);
            button_Disconnect.Name = "button_Disconnect";
            button_Disconnect.Size = new Size(105, 29);
            button_Disconnect.TabIndex = 5;
            button_Disconnect.Text = "Disconnect";
            button_Disconnect.UseVisualStyleBackColor = false;
            // 
            // button_Connect
            // 
            button_Connect.BackColor = Color.DarkGray;
            button_Connect.Location = new Point(712, 9);
            button_Connect.Name = "button_Connect";
            button_Connect.Size = new Size(86, 29);
            button_Connect.TabIndex = 4;
            button_Connect.Text = "Connect";
            button_Connect.UseVisualStyleBackColor = false;
            // 
            // Port
            // 
            Port.Location = new Point(318, 7);
            Port.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            Port.Name = "Port";
            Port.Size = new Size(82, 27);
            Port.TabIndex = 3;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(277, 10);
            label4.Name = "label4";
            label4.Size = new Size(35, 20);
            label4.TabIndex = 2;
            label4.Text = "Port";
            // 
            // IPServer
            // 
            IPServer.FormattingEnabled = true;
            IPServer.Location = new Point(156, 6);
            IPServer.Name = "IPServer";
            IPServer.Size = new Size(96, 28);
            IPServer.TabIndex = 1;
            IPServer.Text = "127.0.0.1";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(24, 12);
            label3.Name = "label3";
            label3.Size = new Size(126, 20);
            label3.TabIndex = 0;
            label3.Text = "Connect to Server";
            // 
            // TabServer
            // 
            TabServer.BackColor = Color.DarkGray;
            TabServer.Controls.Add(numericUpDown1);
            TabServer.Controls.Add(button_StopService);
            TabServer.Controls.Add(button_StartService);
            TabServer.Controls.Add(label2);
            TabServer.Location = new Point(4, 29);
            TabServer.Name = "TabServer";
            TabServer.Padding = new Padding(3);
            TabServer.Size = new Size(989, 46);
            TabServer.TabIndex = 0;
            TabServer.Text = "Server ";
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(167, 10);
            numericUpDown1.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(112, 27);
            numericUpDown1.TabIndex = 4;
            // 
            // button_StopService
            // 
            button_StopService.BackColor = SystemColors.AppWorkspace;
            button_StopService.Location = new Point(718, 8);
            button_StopService.Name = "button_StopService";
            button_StopService.Size = new Size(154, 29);
            button_StopService.TabIndex = 3;
            button_StopService.Text = "Stop Service";
            button_StopService.UseVisualStyleBackColor = false;
            button_StopService.Click += button_StopService_Click;
            // 
            // button_StartService
            // 
            button_StartService.BackColor = SystemColors.AppWorkspace;
            button_StartService.Location = new Point(420, 8);
            button_StartService.Name = "button_StartService";
            button_StartService.Size = new Size(149, 29);
            button_StartService.TabIndex = 2;
            button_StartService.Text = "Start Service";
            button_StartService.UseVisualStyleBackColor = false;
            button_StartService.Click += button_StartService_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(63, 12);
            label2.Name = "label2";
            label2.Size = new Size(98, 20);
            label2.TabIndex = 0;
            label2.Text = "Listen on Port";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(TabServer);
            tabControl1.Controls.Add(TabClient);
            tabControl1.Location = new Point(36, 130);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(997, 79);
            tabControl1.TabIndex = 4;
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.Location = new Point(779, 250);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(247, 284);
            listBox1.TabIndex = 15;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.BackColor = SystemColors.AppWorkspace;
            label6.Location = new Point(779, 218);
            label6.Name = "label6";
            label6.Size = new Size(179, 20);
            label6.TabIndex = 16;
            label6.Text = "Online Users                      ";
            // 
            // button_SendFile
            // 
            button_SendFile.BackColor = SystemColors.AppWorkspace;
            button_SendFile.Location = new Point(610, 229);
            button_SendFile.Name = "button_SendFile";
            button_SendFile.Size = new Size(135, 54);
            button_SendFile.TabIndex = 17;
            button_SendFile.Text = "Send File";
            button_SendFile.UseVisualStyleBackColor = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.AppWorkspace;
            ClientSize = new Size(1153, 563);
            Controls.Add(button_SendFile);
            Controls.Add(label6);
            Controls.Add(listBox1);
            Controls.Add(statusStrip1);
            Controls.Add(panel1);
            Controls.Add(button8_Exit);
            Controls.Add(button7_Clear);
            Controls.Add(richTextBox1);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "Baitaplon";
            Load += Form1_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            TabClient.ResumeLayout(false);
            TabClient.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)Port).EndInit();
            TabServer.ResumeLayout(false);
            TabServer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            tabControl1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private ContextMenuStrip contextMenuStrip1;
        private RichTextBox richTextBox1;
        private Button button7_Clear;
        private Button button8_Exit;
        private Label label5;
        private TextBox textBox1;
        private Button button_Send;
        private Panel panel1;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private TabPage TabClient;
        private Button button_Disconnect;
        private Button button_Connect;
        private NumericUpDown Port;
        private Label label4;
        private ComboBox IPServer;
        private Label label3;
        private TabPage TabServer;
        private NumericUpDown numericUpDown1;
        private Button button_StopService;
        private Button button_StartService;
        private Label label2;
        private TabControl tabControl1;
        private TextBox TextNhapUsername;
        private Label label1;
        private ListBox listBox1;
        private Label label6;
        private Button button_SendFile;
    }
}
