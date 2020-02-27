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
    public partial class AddAngajat : Form
    {
        bool edit = false;
        int id;

        public void set(DataGridViewRow rw)
        {
            edit = true;
            id = Int32.Parse(rw.Cells["ID"].Value.ToString());
            textBox1.Text = rw.Cells["Nume"].Value.ToString();
            textBox2.Text = rw.Cells["Prenume"].Value.ToString();
            numericUpDown1.Value = decimal.Parse(rw.Cells["Ore"].Value.ToString());
            numericUpDown2.Value = decimal.Parse(rw.Cells["Salariu"].Value.ToString());
            richTextBox1.Text = Database.Value("SELECT info FROM Angajati WHERE id_angajat = '"+id+"'");
        }

        public AddAngajat()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                if (edit)
                {
                    SqlCommand cmd = new SqlCommand("UPDATE Angajati SET nume = @nm , prenume = @pr , id_functie = @fun , ore = @ore, salariu = @salar , info = @inf WHERE id_angajat = @id ");
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@nm", textBox1.Text);
                    cmd.Parameters.AddWithValue("@pr", textBox2.Text);
                    cmd.Parameters.AddWithValue("@fun", 1);
                    cmd.Parameters.AddWithValue("@ore", decimal.ToInt32(numericUpDown1.Value));
                    cmd.Parameters.AddWithValue("@salar", decimal.ToInt32(numericUpDown2.Value));
                    cmd.Parameters.AddWithValue("@inf", richTextBox1.Text);
                    Database.Execute(cmd);
                    MessageBox.Show("Angajat actualizat cu succes");
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO Angajati VALUES( @nm , @pr , @fun , @ore, @salar , @inf )");
                    cmd.Parameters.AddWithValue("@nm", textBox1.Text);
                    cmd.Parameters.AddWithValue("@pr", textBox2.Text);
                    cmd.Parameters.AddWithValue("@fun", 1);
                    cmd.Parameters.AddWithValue("@ore", decimal.ToInt32(numericUpDown1.Value));
                    cmd.Parameters.AddWithValue("@salar", decimal.ToInt32(numericUpDown2.Value));
                    cmd.Parameters.AddWithValue("@inf", richTextBox1.Text);
                    Database.Execute(cmd);
                    MessageBox.Show("Angajat adaugat cu succes");
                    this.DialogResult = DialogResult.OK;
                }
            }
            else
            {
                MessageBox.Show("Va rugam sa completati toate campurile!");
            }

        }
    }
}
