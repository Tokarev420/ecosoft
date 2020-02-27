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
    public partial class AddZi : Form
    {
        public int cerere;


        public AddZi()
        {
            InitializeComponent();
        }

        private void AddZi_Load(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            cerere = Decimal.ToInt32(numericUpDown1.Value);
        }
    }
}
