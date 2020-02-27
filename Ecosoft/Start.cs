using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ecosoft
{
    public partial class Start : Form
    {
        public Start()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Ecosoft eco = new Ecosoft();
            eco.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Database.Execute("DELETE FROM Angajati");
            Database.Execute("DELETE FROM Produse");
            Database.Execute("DELETE FROM Investitii");
            Database.Execute("DELETE FROM Materii");
        }
    }
}
