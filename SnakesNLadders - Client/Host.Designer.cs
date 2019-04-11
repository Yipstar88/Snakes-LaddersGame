namespace SnakesNLadders___Client
{
    partial class Host
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Host));
            this.btn_close_server = new System.Windows.Forms.Button();
            this.Status_textBox = new System.Windows.Forms.TextBox();
            this.Bind_button = new System.Windows.Forms.Button();
            this.ReceivePort_textBox = new System.Windows.Forms.TextBox();
            this.IP_Address_textBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.NumberOfClients_textBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.Message_textBox = new System.Windows.Forms.RichTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtServerName = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.CountDown = new System.Windows.Forms.Timer(this.components);
            this.PlayerGo = new System.Windows.Forms.Timer(this.components);
            this.label10 = new System.Windows.Forms.Label();
            this.txtPlayerTurn = new System.Windows.Forms.TextBox();
            this.counter = new System.Windows.Forms.Timer(this.components);
            this.m_SendServerAdvertisement_Timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // btn_close_server
            // 
            this.btn_close_server.Location = new System.Drawing.Point(473, 335);
            this.btn_close_server.Name = "btn_close_server";
            this.btn_close_server.Size = new System.Drawing.Size(108, 31);
            this.btn_close_server.TabIndex = 0;
            this.btn_close_server.Text = "Close Server";
            this.btn_close_server.UseVisualStyleBackColor = true;
            this.btn_close_server.Click += new System.EventHandler(this.button1_Click);
            // 
            // Status_textBox
            // 
            this.Status_textBox.Location = new System.Drawing.Point(407, 71);
            this.Status_textBox.Name = "Status_textBox";
            this.Status_textBox.ReadOnly = true;
            this.Status_textBox.Size = new System.Drawing.Size(187, 20);
            this.Status_textBox.TabIndex = 12;
            // 
            // Bind_button
            // 
            this.Bind_button.Location = new System.Drawing.Point(27, 111);
            this.Bind_button.Name = "Bind_button";
            this.Bind_button.Size = new System.Drawing.Size(165, 27);
            this.Bind_button.TabIndex = 6;
            this.Bind_button.Text = "Start Server";
            this.Bind_button.UseVisualStyleBackColor = true;
            this.Bind_button.Click += new System.EventHandler(this.Bind_button_Click_1);
            // 
            // ReceivePort_textBox
            // 
            this.ReceivePort_textBox.Location = new System.Drawing.Point(123, 78);
            this.ReceivePort_textBox.Name = "ReceivePort_textBox";
            this.ReceivePort_textBox.Size = new System.Drawing.Size(69, 20);
            this.ReceivePort_textBox.TabIndex = 5;
            this.ReceivePort_textBox.Text = "8000";
            // 
            // IP_Address_textBox
            // 
            this.IP_Address_textBox.Location = new System.Drawing.Point(123, 52);
            this.IP_Address_textBox.Name = "IP_Address_textBox";
            this.IP_Address_textBox.ReadOnly = true;
            this.IP_Address_textBox.Size = new System.Drawing.Size(131, 20);
            this.IP_Address_textBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.Location = new System.Drawing.Point(24, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Local IP Address :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label2.Location = new System.Drawing.Point(24, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Port for receiving :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label3.Location = new System.Drawing.Point(297, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Player\'s Online :";
            // 
            // NumberOfClients_textBox
            // 
            this.NumberOfClients_textBox.Location = new System.Drawing.Point(407, 45);
            this.NumberOfClients_textBox.Name = "NumberOfClients_textBox";
            this.NumberOfClients_textBox.ReadOnly = true;
            this.NumberOfClients_textBox.Size = new System.Drawing.Size(36, 20);
            this.NumberOfClients_textBox.TabIndex = 16;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label4.Location = new System.Drawing.Point(297, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Recent Activity";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label5.Location = new System.Drawing.Point(164, 168);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Messages Received :";
            // 
            // Message_textBox
            // 
            this.Message_textBox.Location = new System.Drawing.Point(162, 191);
            this.Message_textBox.Margin = new System.Windows.Forms.Padding(2);
            this.Message_textBox.Name = "Message_textBox";
            this.Message_textBox.ReadOnly = true;
            this.Message_textBox.Size = new System.Drawing.Size(419, 136);
            this.Message_textBox.TabIndex = 19;
            this.Message_textBox.Text = "";
            this.Message_textBox.TextChanged += new System.EventHandler(this.Message_textBox_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label6.Location = new System.Drawing.Point(28, 168);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 13);
            this.label6.TabIndex = 21;
            this.label6.Text = "Connected Users";
            // 
            // listBox1
            // 
            this.listBox1.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 22;
            this.listBox1.Location = new System.Drawing.Point(27, 191);
            this.listBox1.Margin = new System.Windows.Forms.Padding(2);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(121, 136);
            this.listBox1.TabIndex = 20;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Red;
            this.label7.Location = new System.Drawing.Point(322, 135);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(142, 24);
            this.label7.TabIndex = 50;
            this.label7.Text = "\"Server Feed\"";
            this.label7.Visible = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label8.Location = new System.Drawing.Point(24, 30);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 13);
            this.label8.TabIndex = 53;
            this.label8.Text = "Server Name :";
            // 
            // txtServerName
            // 
            this.txtServerName.Location = new System.Drawing.Point(123, 26);
            this.txtServerName.MaxLength = 20;
            this.txtServerName.Multiline = true;
            this.txtServerName.Name = "txtServerName";
            this.txtServerName.Size = new System.Drawing.Size(147, 20);
            this.txtServerName.TabIndex = 54;
            this.txtServerName.Text = "SNL Server";
            // 
            // timer1
            // 
            this.timer1.Interval = 2000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(407, 19);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(57, 20);
            this.textBox1.TabIndex = 55;
            this.textBox1.Text = "01:00";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label9.Location = new System.Drawing.Point(297, 26);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(95, 13);
            this.label9.TabIndex = 56;
            this.label9.Text = "Game will Start In :";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(483, 17);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 22);
            this.button1.TabIndex = 57;
            this.button1.Text = "Start Game";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // CountDown
            // 
            this.CountDown.Interval = 1000;
            this.CountDown.Tick += new System.EventHandler(this.CountDown_Tick);
            // 
            // PlayerGo
            // 
            this.PlayerGo.Interval = 1000;
            this.PlayerGo.Tick += new System.EventHandler(this.PlayerGo_Tick);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label10.Location = new System.Drawing.Point(297, 102);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(106, 13);
            this.label10.TabIndex = 58;
            this.label10.Text = "Which Players Turn -";
            // 
            // txtPlayerTurn
            // 
            this.txtPlayerTurn.Location = new System.Drawing.Point(407, 99);
            this.txtPlayerTurn.Name = "txtPlayerTurn";
            this.txtPlayerTurn.ReadOnly = true;
            this.txtPlayerTurn.Size = new System.Drawing.Size(187, 20);
            this.txtPlayerTurn.TabIndex = 59;
            // 
            // counter
            // 
            this.counter.Interval = 1000;
            this.counter.Tick += new System.EventHandler(this.counter_Tick);
            // 
            // Host
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(608, 378);
            this.Controls.Add(this.txtPlayerTurn);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.txtServerName);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.Message_textBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.NumberOfClients_textBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Status_textBox);
            this.Controls.Add(this.btn_close_server);
            this.Controls.Add(this.ReceivePort_textBox);
            this.Controls.Add(this.Bind_button);
            this.Controls.Add(this.IP_Address_textBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Host";
            this.Text = "Server Hosting - System Programming Project";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_close_server;
        private System.Windows.Forms.TextBox ReceivePort_textBox;
        private System.Windows.Forms.TextBox Status_textBox;
        private System.Windows.Forms.Button Bind_button;
        private System.Windows.Forms.TextBox IP_Address_textBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox NumberOfClients_textBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RichTextBox Message_textBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtServerName;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Timer CountDown;
        private System.Windows.Forms.Timer PlayerGo;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtPlayerTurn;
        private System.Windows.Forms.Timer counter;
        private System.Windows.Forms.Timer m_SendServerAdvertisement_Timer;
    }
}