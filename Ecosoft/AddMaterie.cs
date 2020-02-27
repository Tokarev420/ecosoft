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
    public partial class AddMaterie : Form
    {

        public string name;
        public decimal price;

        public AddMaterie()
        {
            InitializeComponent();
        }

        public void set(string _name, decimal _price)
        {
            name = _name;
            price = _price;

            textBox1.Text = name;
            numericUpDown1.Value = price;
        }

        private void AddMaterie_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && numericUpDown1.Value != 0)
            {
                name = textBox1.Text;
                price = numericUpDown1.Value;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Va rugam sa completati toate campurile");
            }
        }


    }
}
