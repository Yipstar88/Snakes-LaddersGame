namespace SnakesNLadders___Client
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_exit = new System.Windows.Forms.Button();
            this.btn_host = new System.Windows.Forms.Button();
            this.btn_join = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Constantia", 36F);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(441, 59);
            this.label2.TabIndex = 11;
            this.label2.Text = "Snakes And Ladders";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(547, 422);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(309, 16);
            this.label1.TabIndex = 10;
            this.label1.Text = "Copyright MrSlosky N Yip-Star88. Do Not Distrbute!";
            // 
            // btn_exit
            // 
            this.btn_exit.BackColor = System.Drawing.Color.DarkOrange;
            this.btn_exit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_exit.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btn_exit.ImageKey = "(none)";
            this.btn_exit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_exit.Location = new System.Drawing.Point(394, 309);
            this.btn_exit.Name = "btn_exit";
            this.btn_exit.Size = new System.Drawing.Size(170, 50);
            this.btn_exit.TabIndex = 9;
            this.btn_exit.Text = "Exit Game";
            this.btn_exit.UseVisualStyleBackColor = false;
            this.btn_exit.Click += new System.EventHandler(this.btn_exit_Click);
            // 
            // btn_host
            // 
            this.btn_host.BackColor = System.Drawing.Color.DarkOrange;
            this.btn_host.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_host.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btn_host.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_host.Location = new System.Drawing.Point(394, 250);
            this.btn_host.Name = "btn_host";
            this.btn_host.Size = new System.Drawing.Size(170, 50);
            this.btn_host.TabIndex = 8;
            this.btn_host.Text = "Host Server";
            this.btn_host.UseVisualStyleBackColor = false;
            this.btn_host.Click += new System.EventHandler(this.btn_host_Click);
            // 
            // btn_join
            // 
            this.btn_join.BackColor = System.Drawing.Color.DarkOrange;
            this.btn_join.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_join.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btn_join.ImageKey = "(none)";
            this.btn_join.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_join.Location = new System.Drawing.Point(394, 191);
            this.btn_join.Name = "btn_join";
            this.btn_join.Size = new System.Drawing.Size(170, 53);
            this.btn_join.TabIndex = 7;
            this.btn_join.Text = "Join Game";
            this.btn_join.UseVisualStyleBackColor = false;
            this.btn_join.Click += new System.EventHandler(this.btn_join_Click);
            // 
            // button1
            // 
            this.button1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button1.BackgroundImage")));
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(11, 402);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(56, 36);
            this.button1.TabIndex = 98;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(860, 443);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_exit);
            this.Controls.Add(this.btn_host);
            this.Controls.Add(this.btn_join);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.Text = "Main";
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_exit;
        private System.Windows.Forms.Button btn_host;
        private System.Windows.Forms.Button btn_join;
        private System.Windows.Forms.Button button1;
    }
}