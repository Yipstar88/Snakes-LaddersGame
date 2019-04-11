using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakesNLadders___Client
{
    public partial class WinnersForm : Form
    {
        
        public WinnersForm(String Name)
        {
            InitializeComponent();
            lblName.Text = Name;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
