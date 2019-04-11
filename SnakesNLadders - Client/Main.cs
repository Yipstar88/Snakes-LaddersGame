using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakesNLadders___Client
{
    public partial class Main : Form
    {
        private Form server;
        private Form connection;

        Boolean Music = true;

        public Main()
        {
            InitializeComponent();
            PlayMusic("Onn");
        }

        private void btn_join_Click(object sender, EventArgs e)
        {
            if (connection == null || (connection != null && connection.IsDisposed))
            {
                connection = new LOBBY();
                PlayMusic("Off");
            }
            if (!connection.Visible)
            {
                connection.Show();
            }
            else
            {
                connection.BringToFront();
            }
        }

        private void btn_host_Click(object sender, EventArgs e)
        {
            if (server == null || (server != null && server.IsDisposed))
            {
                server = new Host();
                PlayMusic("Off");
            }
            if (!server.Visible)
            {
                server.Show();
            }
            else
            {
                server.BringToFront();
            }
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public String FileName(string path)
        {
            string projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            string File = Path.Combine(projectPath, "" + path);
            System.IO.Directory.CreateDirectory(File);
            return File;
        }

        public void PlayMusic(string status)
        {
            SoundPlayer player = new SoundPlayer(FileName("SoundEffects") + "/MainMenu.wav");
            if (status == "On")
            {
                player.PlayLooping();
            }
            else
            {
                player.Stop();
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            PlayMusic("On");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Music == true)
            {
                PlayMusic("Off");
                Music = false;
                button1.BackgroundImage = Image.FromFile(FileName("icons") + "/Onn.png");
                button1.BackgroundImageLayout = ImageLayout.Stretch;
            }
            else
            {
                PlayMusic("On");
                button1.BackgroundImage = Image.FromFile(FileName("icons") + "/Off.png");
                button1.BackgroundImageLayout = ImageLayout.Stretch;
                Music = true;
            }
        }
    }
}
