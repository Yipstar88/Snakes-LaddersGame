namespace SnakesNLadders___Client
{
    partial class TCP_Server_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TCP_Server_Form));
            this.btn_close_server = new System.Windows.Forms.Button();
            this.IP_Address_textBox = new System.Windows.Forms.TextBox();
            this.ReceivePort_textBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Bind_button = new System.Windows.Forms.Button();
            this.Listen_button = new System.Windows.Forms.Button();
            this.Accept_button = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.NumberOfClients_textBox = new System.Windows.Forms.TextBox();
            this.Status_textBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.Message_textBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // btn_close_server
            // 
            this.btn_close_server.Location = new System.Drawing.Point(610, 394);
            this.btn_close_server.Name = "btn_close_server";
            this.btn_close_server.Size = new System.Drawing.Size(108, 31);
            this.btn_close_server.TabIndex = 0;
            this.btn_close_server.Text = "Close Server";
            this.btn_close_server.UseVisualStyleBackColor = true;
            // 
            // IP_Address_textBox
            // 
            this.IP_Address_textBox.Location = new System.Drawing.Point(105, 23);
            this.IP_Address_textBox.Name = "IP_Address_textBox";
            this.IP_Address_textBox.Size = new System.Drawing.Size(100, 20);
            this.IP_Address_textBox.TabIndex = 1;
            // 
            // ReceivePort_textBox
            // 
            this.ReceivePort_textBox.Location = new System.Drawing.Point(304, 23);
            this.ReceivePort_textBox.Name = "ReceivePort_textBox";
            this.ReceivePort_textBox.Size = new System.Drawing.Size(100, 20);
            this.ReceivePort_textBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.Location = new System.Drawing.Point(12, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Local IP Address :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label2.Location = new System.Drawing.Point(211, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Port for receiving :";
            // 
            // Bind_button
            // 
            this.Bind_button.Location = new System.Drawing.Point(30, 69);
            this.Bind_button.Name = "Bind_button";
            this.Bind_button.Size = new System.Drawing.Size(75, 23);
            this.Bind_button.TabIndex = 5;
            this.Bind_button.Text = "Bind to Port";
            this.Bind_button.UseVisualStyleBackColor = true;
            // 
            // Listen_button
            // 
            this.Listen_button.Location = new System.Drawing.Point(122, 69);
            this.Listen_button.Name = "Listen_button";
            this.Listen_button.Size = new System.Drawing.Size(164, 23);
            this.Listen_button.TabIndex = 6;
            this.Listen_button.Text = "Listen for Connection requests";
            this.Listen_button.UseVisualStyleBackColor = true;
            // 
            // Accept_button
            // 
            this.Accept_button.Location = new System.Drawing.Point(314, 69);
            this.Accept_button.Name = "Accept_button";
            this.Accept_button.Size = new System.Drawing.Size(162, 23);
            this.Accept_button.TabIndex = 7;
            this.Accept_button.Text = "Accept Connection request";
            this.Accept_button.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label3.Location = new System.Drawing.Point(27, 117);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Player\'s Connected";
            // 
            // NumberOfClients_textBox
            // 
            this.NumberOfClients_textBox.Location = new System.Drawing.Point(131, 114);
            this.NumberOfClients_textBox.Name = "NumberOfClients_textBox";
            this.NumberOfClients_textBox.Size = new System.Drawing.Size(48, 20);
            this.NumberOfClients_textBox.TabIndex = 9;
            // 
            // Status_textBox
            // 
            this.Status_textBox.Location = new System.Drawing.Point(131, 153);
            this.Status_textBox.Name = "Status_textBox";
            this.Status_textBox.Size = new System.Drawing.Size(171, 20);
            this.Status_textBox.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label4.Location = new System.Drawing.Point(27, 156);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Server Status";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label5.Location = new System.Drawing.Point(27, 215);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Message Box :";
            // 
            // Message_textBox
            // 
            this.Message_textBox.Location = new System.Drawing.Point(122, 212);
            this.Message_textBox.Name = "Message_textBox";
            this.Message_textBox.Size = new System.Drawing.Size(354, 165);
            this.Message_textBox.TabIndex = 14;
            this.Message_textBox.Text = "";
            // 
            // TCP_Server_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(730, 437);
            this.Controls.Add(this.Message_textBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Status_textBox);
            this.Controls.Add(this.NumberOfClients_textBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Accept_button);
            this.Controls.Add(this.Listen_button);
            this.Controls.Add(this.Bind_button);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ReceivePort_textBox);
            this.Controls.Add(this.IP_Address_textBox);
            this.Controls.Add(this.btn_close_server);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TCP_Server_Form";
            this.Text = "Server Hosting - System Programming Project";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_close_server;
        private System.Windows.Forms.TextBox IP_Address_textBox;
        private System.Windows.Forms.TextBox ReceivePort_textBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Bind_button;
        private System.Windows.Forms.Button Listen_button;
        private System.Windows.Forms.Button Accept_button;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox NumberOfClients_textBox;
        private System.Windows.Forms.TextBox Status_textBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RichTextBox Message_textBox;
    }
}