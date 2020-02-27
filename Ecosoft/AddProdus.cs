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
    public partial class AddProdus : Form
    {

        public bool newEntry = true;
        public int id;
        public string usedStr;

        List<Materie> materii;
        List<Materie> used;
        decimal totalPrice = 0;
        decimal commercePrice = 0;

        public AddProdus()
        {
            InitializeComponent();
            
        }

        public void set(string name, decimal hours, decimal comm)
        {
            textBox1.Text = name;
            numericUpDown1.Value = hours;
            numericUpDown2.Value = comm;
            commercePrice = comm;
        }

        private void AddProdus_Load(object sender, EventArgs e)
        {
            materii = new List<Materie>(500);
            used = new List<Materie>(500);

            if (!newEntry)
            {
                string[] qq = usedStr.Split(',');
                for (int i = 0; i < qq.Length; i++)
                {
                    if (qq[i] != "")
                    {
                        Materie matt = new Materie(qq[i]);
                        used.Add(matt);
                    }
                }
            }

            DataTable dt = Database.Select("SELECT * FROM Materii");

            if (!newEntry)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Materie matt = new Materie();
                    matt.set((int)dt.Rows[i][0], 0, Decimal.Parse(dt.Rows[i][2].ToString()), dt.Rows[i][1].ToString());
                    materii.Add(matt);
                }
            }
            else
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Materie matt = new Materie();
                    matt.set((int)dt.Rows[i][0], 0, Decimal.Parse(dt.Rows[i][2].ToString()), dt.Rows[i][1].ToString());
                    if(!used.Exists(x => x.id == matt.id))
                        materii.Add(matt);
                }
            }

            updatePrice();
            refreshTable();
        }

        void updateProfit()
        { 
            label7.Text = "Profit: " + (commercePrice - totalPrice);
        }

        void updatePrice()
        {
            totalPrice = 0;
            for (int i = 0; i < used.Count; i++)
            {
                totalPrice += used[i].getFinalPrice();
            }
            //label3.Text = "Pret total: " + totalPrice;
            numericUpDown3.Value = totalPrice;
            updateProfit();
        }

        void refreshFirst()
        {
            dataGridView1.Columns.Clear();

            DataTable dt1 = new DataTable();
            dt1.Columns.Add("Materii", typeof(string));


            for (int i = 0; i < materii.Count; i++)
            {
                dt1.Rows.Add(materii[i].name);
            }

            dataGridView1.DataSource = dt1;

            DataGridViewButtonColumn clmn = new DataGridViewButtonColumn();
            clmn.HeaderText = "Adauga";
            clmn.UseColumnTextForButtonValue = true;
            clmn.Text = "Adauga";

            dataGridView1.Columns.Add(clmn);
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        void refreshTable()
        {
            
            dataGridView2.Columns.Clear();



            refreshFirst();
            //////////////2

            DataGridViewTextBoxColumn clmn0 = new DataGridViewTextBoxColumn();
            clmn0.HeaderText = "Materie";
            clmn0.ReadOnly = true;
            clmn0.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView2.Columns.Add(clmn0);

            DataGridViewTextBoxColumn clmn1 = new DataGridViewTextBoxColumn();
            clmn1.HeaderText = "Cantitate";
            dataGridView2.Columns.Add(clmn1);

            DataGridViewTextBoxColumn clmnx1 = new DataGridViewTextBoxColumn();
            clmnx1.HeaderText = "Pret";
            clmnx1.Name = "Pret";
            clmnx1.ReadOnly = true;
            //clmnx1.ReadOnly = true;
            dataGridView2.Columns.Add(clmnx1);

            DataGridViewButtonColumn clmn2 = new DataGridViewButtonColumn();
            clmn2.HeaderText = "Elimina";
            clmn2.UseColumnTextForButtonValue = true;
            clmn2.Text = "Elimina";
            dataGridView2.Columns.Add(clmn2);

            for (int i = 0; i < used.Count; i++)
            {
                dataGridView2.Rows.Add(used[i].name, "" + used[i].quantity, "" + used[i].getFinalPrice()); 
            }

           
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1 && e.RowIndex > -1)
            {
                string str = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                Materie m = materii.Find(x => x.name == str);
                m.quantity = 0;
                used.Add(m);
                materii.Remove(m);

                dataGridView1.Rows.RemoveAt(e.RowIndex);
                dataGridView2.Rows.Add(str, "0", "0"); 
            }
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3 && e.RowIndex > -1)
            {
                string str = dataGridView2.Rows[e.RowIndex].Cells[0].Value.ToString();
                Materie m = used.Find(x => x.name == str);
                m.quantity = 0;
                materii.Add(m);
                used.Remove(m);

                refreshFirst();
                dataGridView2.Rows.RemoveAt(e.RowIndex);
                updatePrice();
            }
        }

        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string str = dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            decimal f;
            if (Decimal.TryParse(str, out f))
            {
                Materie q = used.Find(x => x.name == dataGridView2.Rows[e.RowIndex].Cells[0].Value.ToString());
                q.quantity = f;
                decimal price = q.getFinalPrice();
                updatePrice();
                dataGridView2.Rows[e.RowIndex].Cells["Pret"].Value = "" + price;
            }
            else
            {
                dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "0";
                MessageBox.Show("Va rugam sa introduceti un numar valid");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string final = "";
            for (int i = 0; i < used.Count; i++)
            {
                if (used[i].quantity != 0)
                {
                    final += used[i].getString();
                    final += ",";
                }
            }
            final.Substring(0, final.Length - 2);

            if (newEntry)
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO Produse VALUES ( @nm, @ore, @mat, @cer, @pr )");
                cmd.Parameters.AddWithValue("@nm", textBox1.Text);
                cmd.Parameters.AddWithValue("@ore", numericUpDown1.Value);
                cmd.Parameters.AddWithValue("@cer", "1");
                cmd.Parameters.AddWithValue("@mat", final);
                cmd.Parameters.AddWithValue("@pr", commercePrice);
                Database.Execute(cmd);
                MessageBox.Show("Produs adaugat cu succes");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                SqlCommand cmd = new SqlCommand("UPDATE Produse SET nume = @nm, ore = @or, materii = @mat , cerere = @cer , pret = @pr WHERE id_produs = '"+id+"'");
                cmd.Parameters.AddWithValue("@nm", textBox1.Text);
                cmd.Parameters.AddWithValue("@or", numericUpDown1.Value);
                cmd.Parameters.AddWithValue("@cer", "1"); ;
                cmd.Parameters.AddWithValue("@mat", final);
                cmd.Parameters.AddWithValue("@pr", commercePrice);
                Database.Execute(cmd);
                MessageBox.Show("Produs actualizat cu succes");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            commercePrice = numericUpDown2.Value;
            updateProfit();
        }
    }

    public class Materie
    {
        public int id;
        public string name;
        public decimal quantity;
        public decimal price;

        public Materie() { }
        public Materie(string x) 
        {
            set(x);
        }

        public string getString()
        {
            return "" + id + "|" + quantity;
        }

        public static decimal getPrice(string q)
        {
            string[] xq = q.Split(',');
            decimal total = 0;
            for (int i = 0; i < xq.Length; i++)
            {
                if(xq[i] != "")
                    total += getSinglePrice(xq[i]);
            }
            return total;
        }

        public static decimal getSinglePrice(string q)
        {
            string[] xq = q.Split('|');
            int _id = Int32.Parse(xq[0]);
            decimal _quantity = Decimal.Parse(xq[1]);
            decimal _price = Decimal.Parse(Database.Value("SELECT pret FROM Materii WHERE id_materie = '" + _id + "'"));
            return _quantity * _price;
        }

        public void set(string q)
        {
            //MessageBox.Show(q);
            string[] xq = q.Split('|');
            id = Int32.Parse(xq[0]);
            quantity = Decimal.Parse(xq[1]);

            name = Database.Value("SELECT nume FROM Materii WHERE id_materie = '" + id + "'");
            price = Decimal.Parse(Database.Value("SELECT pret FROM Materii WHERE id_materie = '" + id + "'"));
        }

        public decimal getFinalPrice()
        {
            return quantity * price;
        }

        public void set(int _id, decimal _q, decimal _price, string _nm = "")
        {
            name = _nm;
            id = _id;
            quantity = _q;
            price = _price;
        }
    }
}
