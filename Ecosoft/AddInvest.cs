using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ecosoft
{
    public partial class AddInvest : Form
    {
        bool edit = false;

        int id = 0;

        public void set(int _id, string name, decimal price, int quant)
        {
            id = _id;
            textBox1.Text = name;
            numericUpDown1.Value = price;
            numericUpDown2.Value = quant;
            edit = true;
        }

        public AddInvest()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && numericUpDown1.Value != 0 && numericUpDown2.Value != 0)
            {
                if (!edit)
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO Investitii VALUES ( @nm, @pr, @qu )");
                    cmd.Parameters.AddWithValue("@nm", textBox1.Text);
                    cmd.Parameters.AddWithValue("@pr", numericUpDown1.Value);
                    cmd.Parameters.AddWithValue("@qu", Decimal.ToInt32(numericUpDown2.Value));
                    Database.Execute(cmd);
                    MessageBox.Show("Investitie adaugata cu succes");
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    SqlCommand cmd = new SqlCommand("UPDATE Investitii SET nume = @nm, pret = @pr, cantitate = @qu WHERE id_investitie = '"+id+"'");
                    cmd.Parameters.AddWithValue("@nm", textBox1.Text);
                    cmd.Parameters.AddWithValue("@pr", numericUpDown1.Value);
                    cmd.Parameters.AddWithValue("@qu", Decimal.ToInt32(numericUpDown2.Value));
                    Database.Execute(cmd);
                    MessageBox.Show("Investitie actualizata cu succes");
                    this.DialogResult = DialogResult.OK;
                }
            }
            else
                MessageBox.Show("Va rugam sa completati toate campurile");
        }

        private void AddInvest_Load(object sender, EventArgs e)
        {

        }
    }
}
