using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Ecosoft
{
    public partial class Documentatie : Form
    {
        public Documentatie()
        {
            InitializeComponent();
        }

        private void Documentatie_Load(object sender, EventArgs e)
        {

            string q = File.ReadAllText(@"C:\Users\KTA\Desktop\Documentatie.txt");
            richTextBox1.Text = q;
            richTextBox1.ReadOnly = true;
            richTextBox1.SelectAll();
            richTextBox1.SelectionAlignment = HorizontalAlignment.Center;
            richTextBox1.DeselectAll();
        }
    }
}
