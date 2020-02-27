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

    public partial class Ecosoft : Form
    {
        List<Produs> produse;
        Cerere cerere;
        decimal investitieTotala = 0;
        decimal profitLunar = 0;
        decimal oreLunar = 0;
        int oreLucrateLunar = 0;
        decimal salarii = 0;

        public Ecosoft()
        {
            InitializeComponent();
        }

        private void Ecosoft_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        void refreshAll()
        {
            refreshMaterii();
            refreshProduse();
            refreshAngajati();
            refreshProfit();
            refreshInvest();
            refreshData();
        }

        void refreshInvest()
        {
            investitiiView.Columns.Clear();

            DataTable inv = Database.Select("SELECT id_investitie AS ID, nume AS Nume, pret as Pret, cantitate AS Cantitate FROM Investitii");

            int cid = inv.Columns.Count;
            int cid1 = cid - 1;
            int cid2 = cid - 2;
            DataColumn dc = new DataColumn("Pret total", typeof(decimal));
            inv.Columns.Add(dc);

            investitieTotala = 0;

            for (int i = 0; i < inv.Rows.Count; i++)
            {
                decimal price = decimal.Parse(inv.Rows[i][cid1].ToString()) * decimal.Parse(inv.Rows[i][cid2].ToString());
                investitieTotala += price;
                inv.Rows[i][cid] = price;
            }

            investitiiView.DataSource = inv;

            label2.Text = "Investitie totala: " + investitieTotala;
        }

        void refreshProfit()
        {
            chart2.Series[0].Points.Clear();
            chart2.Series[1].Points.Clear();

            for(int i=0;i<produseView.Rows.Count;i++)
            {
                string name = produseView.Rows[i].Cells[1].Value.ToString();
                string id = produseView.Rows[i].Cells[0].Value.ToString();
                decimal ppu = Decimal.Parse(produseView.Rows[i].Cells[5].Value.ToString());
                decimal hrs = Decimal.Parse(Database.Value("SELECT ore FROM Produse WHERE id_produs = '"+id+"'"));
                decimal ppo = ppu / hrs;

                chart2.Series[0].Points.AddXY(name, ppu);
                chart2.Series[1].Points.AddXY(name, ppo);
            }
        }

        void refreshData()
        {
            cerereView.Columns.Clear();

            DataTable dt = Database.Select("SELECT id_produs AS ID, nume AS Nume, cerere AS 'Cerere pe luna', ore AS 'Ore in total' FROM Produse");

            int cid = dt.Columns.Count;
            int cid2 = cid - 1;
            int cid3 = cid - 2;

            DataColumn clmn = new DataColumn("Profit total", typeof(decimal));
            dt.Columns.Add(clmn);

            profitLunar = 0;
            oreLunar = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                decimal bucati = decimal.Parse(dt.Rows[i][cid3].ToString());
                decimal ore = decimal.Parse(dt.Rows[i][cid2].ToString()) * bucati;

                decimal newProfit = bucati * Database.getProfit(Int32.Parse(dt.Rows[i][0].ToString()));

                dt.Rows[i][cid2] = ore;
                dt.Rows[i][cid] = newProfit;

                profitLunar += newProfit;
                oreLunar += ore;
            }


            cerereView.DataSource = dt;

            cerereView.Columns[0].ReadOnly = true;
            cerereView.Columns[1].ReadOnly = true;
            cerereView.Columns[3].ReadOnly = true;
            cerereView.Columns[4].ReadOnly = true;



            label1.Text = "Profit lunar: " + (profitLunar - salarii);
            label6.Text = "Salarii: " + salarii;
            label8.Text = "Profit lunar net: " + profitLunar;
            label3.Text = "Ore necesare lunar: " + oreLunar;

            if (profitLunar > salarii)
            {
                decimal luni = decimal.Floor(investitieTotala / (profitLunar - salarii)) + 1;
                label7.Text = "Amortizarea investitiei \n initiale: " + luni + " luni";
            }
            else
            {
                label7.Text = "Investitia initiala \n nu se poate amortiza";
            }


            if (oreLucrateLunar < oreLunar)
            {
                label5.ForeColor = Color.Red;
                decimal newAng = decimal.Ceiling((oreLunar - oreLucrateLunar) / 160);
                label5.Text = "Aveti nevoie de inca " + newAng + " angajati";
            }
            else
            {
                if (oreLucrateLunar - oreLunar > 160)
                {
                    label5.ForeColor = Color.Green;
                    decimal newAng = decimal.Floor((oreLucrateLunar - oreLunar) / 160);
                    label5.Text = "Aveti in plus " + newAng + " angajati";
                }
                else
                {
                    label5.Text = "Numarul de angajati \n este in echilibru";
                }
            }

        }


        void refreshAngajati()
        {
            DataTable angajati = Database.Select("SELECT id_angajat AS ID, nume AS Nume, prenume AS Prenume, ore AS Ore, salariu AS Salariu FROM Angajati");

            oreLucrateLunar = 0;
            salarii = 0;

            for (int i = 0; i < angajati.Rows.Count; i++)
            {
                oreLucrateLunar += Int32.Parse(angajati.Rows[i]["Ore"].ToString());
                salarii += Decimal.Parse(angajati.Rows[i]["Salariu"].ToString());
            }

            oreLucrateLunar *= 4;

            

            label4.Text = "Ore lucrate lunar: " + oreLucrateLunar;

          
            //angajati.Columns.RemoveAt(2);

           

            angajatiView.Columns.Clear();
            angajatiView.DataSource = angajati;



            DataGridViewButtonColumn clmn = new DataGridViewButtonColumn();
            clmn.HeaderText = "Edit";
            clmn.Name = "Edit";
            clmn.Text = "Edit";
            clmn.UseColumnTextForButtonValue = true;
            angajatiView.Columns.Add(clmn);

            DataGridViewButtonColumn clmn2 = new DataGridViewButtonColumn();
            clmn2.HeaderText = "Concediaza";
            clmn2.Text = "Concediaza";
            clmn2.Name = "Concediaza";
            clmn2.UseColumnTextForButtonValue = true;
            angajatiView.Columns.Add(clmn2);


            angajatiView.Columns["Ore"].HeaderText = "Ore saptamanal";
            angajatiView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }


        void refreshProduse()
        {
            DataTable produse = Database.Select("SELECT id_produs AS ID, nume AS Nume, ore AS Ore, pret AS 'Pret' FROM Produse");
            produseView.Columns.Clear();

            string cost = "Cost de productie";
            string profit = "Profit per unitate";
            int cidM = produse.Columns.Count-1;
            int cid = produse.Columns.Count;
            int cid2 = produse.Columns.Count + 1;

            DataColumn dclmn = new DataColumn(cost, typeof(decimal));
            produse.Columns.Add(dclmn);
            DataColumn dclmn2 = new DataColumn(profit, typeof(decimal));
            produse.Columns.Add(dclmn2);

            for (int i = 0; i < produse.Rows.Count; i++)
            {
                decimal prodCost = Database.getPrice(Int32.Parse(produse.Rows[i][0].ToString()));
                produse.Rows[i][cid] = prodCost;
                produse.Rows[i][cid2] = Decimal.Parse(produse.Rows[i][cidM].ToString()) - prodCost;
            }
            
            produseView.DataSource = produse;

            
            //profit 

            DataGridViewButtonColumn clmn = new DataGridViewButtonColumn();
            clmn.HeaderText = "Modifica";
            clmn.Text = "Modifica";
            clmn.UseColumnTextForButtonValue = true;
            produseView.Columns.Add(clmn);

            DataGridViewButtonColumn clmn2 = new DataGridViewButtonColumn();
            clmn2.HeaderText = "Elimina";
            clmn2.Text = "Elimina";
            clmn2.UseColumnTextForButtonValue = true;
            produseView.Columns.Add(clmn2);

            produseView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        void refreshMaterii()
        {
            DataTable materii = Database.Select("SELECT id_materie AS ID, nume AS Nume, pret AS Pret FROM Materii");
            materiiView.Columns.Clear();
            materiiView.DataSource = materii;

            DataGridViewButtonColumn clmn2 = new DataGridViewButtonColumn();
            clmn2.HeaderText = "Modifica";
            clmn2.Text = "Modifica";
            clmn2.UseColumnTextForButtonValue = true;
            materiiView.Columns.Add(clmn2);

            materiiView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void materiiAdd_Click(object sender, EventArgs e)
        {
            AddMaterie adm = new AddMaterie();
            if (adm.ShowDialog() == DialogResult.OK)
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO Materii VALUES( @nume , @pret)");
                cmd.Parameters.AddWithValue("@nume", adm.name);
                cmd.Parameters.AddWithValue("@pret", adm.price);
                Database.Execute(cmd);
                refreshMaterii();
                refreshProduse();
                refreshProfit();
                MessageBox.Show("Materie prima adaugata cu succes");
            }
        }

        private void Ecosoft_Load(object sender, EventArgs e)
        {
            //Database.Execute("DELETE FROM Produse");
            produse = new List<Produs>(100);
            cerere = new Cerere();
            refreshAll();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddProdus adp = new AddProdus();
            if (adp.ShowDialog() == DialogResult.OK)
            {
                refreshProduse();
                refreshData();
                refreshProfit();
            }
        }

        private void produseView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void produseView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex == 6)
            {
                AddProdus adp = new AddProdus();
                adp.newEntry = false;
                adp.id = Int32.Parse(produseView.Rows[e.RowIndex].Cells[0].Value.ToString());
                adp.usedStr = Database.Value("SELECT materii FROM Produse WHERE id_produs = '" + adp.id + "'");
                adp.set(produseView.Rows[e.RowIndex].Cells[1].Value.ToString(), Decimal.Parse(produseView.Rows[e.RowIndex].Cells[2].Value.ToString()), Decimal.Parse(produseView.Rows[e.RowIndex].Cells[3].Value.ToString()));
                if (adp.ShowDialog() == DialogResult.OK)
                {
                    refreshProduse();
                    refreshData();
                }
            }

            if (e.ColumnIndex == 7)
            {
                int id = Int32.Parse(produseView.Rows[e.RowIndex].Cells[0].Value.ToString());
                string name = Database.Value("SELECT nume FROM Produse WHERE id_produs = '" + id + "'");
                DialogResult dialogResult = MessageBox.Show("Eliminati "+name+" din lista de produse?", "Eliminare - "+name , MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Database.Execute("DELETE FROM Produse WHERE id_produs = '"+id+"'");
                    produseView.Rows.RemoveAt(e.RowIndex);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AddAngajat ada = new AddAngajat();
            if (ada.ShowDialog() == DialogResult.OK)
            {
                refreshAngajati();
                refreshData();
            }
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void documentatieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Documentatie doc = new Documentatie();
            doc.Show();
        }

        private void materiiView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex > -1)
            {
                string id = materiiView.Rows[e.RowIndex].Cells[1].Value.ToString();
                string nume = materiiView.Rows[e.RowIndex].Cells[2].Value.ToString();

                
                string q = materiiView.Rows[e.RowIndex].Cells[3].Value.ToString();

                decimal pret = Decimal.Parse(q);
               
                AddMaterie adm = new AddMaterie();
                adm.set(nume, pret);

                if (adm.ShowDialog() == DialogResult.OK)
                {
                    SqlCommand cmd = new SqlCommand("UPDATE Materii SET nume = @nm , pret = @pr WHERE id_materie = '"+id+"'");
                    cmd.Parameters.AddWithValue("@nm", adm.name);
                    cmd.Parameters.AddWithValue("@pr", adm.price);
                    Database.Execute(cmd);
                    refreshMaterii();
                    refreshProduse();
                    refreshProfit();
                    MessageBox.Show("Materie prima actualizata cu succes");
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AddInvest inv = new AddInvest();
            if (inv.ShowDialog() == DialogResult.OK)
            {
                refreshInvest();
            }
           
        }

        private void label2_Click(object sender, EventArgs e)
        {
            label2.Text = "Investitie totala: " + investitieTotala;
        }

        private void cerereView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int cereree = Int32.Parse(cerereView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
            int id = Int32.Parse(cerereView.Rows[e.RowIndex].Cells[0].Value.ToString());
            Database.Execute("UPDATE Produse SET cerere = '"+cereree+"' WHERE id_produs = '"+id+"'");
            refreshData();
        }

        private void angajatiView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (angajatiView.Columns[e.ColumnIndex].Name == "Edit" && e.RowIndex > -1)//edit
            {
                AddAngajat ada = new AddAngajat();
                ada.set(angajatiView.Rows[e.RowIndex]);
                if (ada.ShowDialog() == DialogResult.OK)
                {
                    refreshAngajati();
                    refreshData();
                }
            }

            if (angajatiView.Columns[e.ColumnIndex].Name == "Concediaza" && e.RowIndex > -1)
            {
                int id = Int32.Parse(angajatiView.Rows[e.RowIndex].Cells["ID"].Value.ToString());
                string name = angajatiView.Rows[e.RowIndex].Cells["Nume"].Value.ToString() + " " + angajatiView.Rows[e.RowIndex].Cells["Prenume"].Value.ToString();
                DialogResult dialogResult = MessageBox.Show("Il concediati pe  " + name + "?", "Concediere - " + name, MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Database.Execute("DELETE FROM Angajati WHERE id_angajat = '" + id + "'");
                    angajatiView.Rows.RemoveAt(e.RowIndex);
                    //MessageBox.Show("L-ati concediat pe " + name);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "";
            sfd.Filter = "Excel | .xlsx";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Database.exportToExcel(produseView, "Produse", 6, sfd.FileName);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "";
            sfd.Filter = "Excel | .xlsx";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Database.exportToExcel(materiiView, "Materii Prime", 2, sfd.FileName);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "";
            sfd.Filter = "Excel | .xlsx";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Database.exportToExcel(angajatiView, "Angajati", 5, sfd.FileName);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "";
            sfd.Filter = "Excel | .xlsx";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Database.exportToExcel(investitiiView, "Investitii", 5, sfd.FileName);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "";
            sfd.Filter = "Excel | .xlsx";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Database.exportToExcel(cerereView, "Cerere", 5, sfd.FileName);
            }
        }
    }

    public class Cerere
    {
        public List<int> zile;
        public List<int> cereri;
        public string nume;
        bool changed = false;

        public Cerere()
        {
            zile = new List<int>(100);
            cereri = new List<int>(100);
            nume = "";
            changed = false;
        }

        public void set(string nm, string q, System.Windows.Forms.DataVisualization.Charting.DataPointCollection points)
        {
            nume = nm;
            points.Clear();
            if (q != "")
            {
                string[] xq = q.Split('|');
                foreach (string x in xq)
                {
                    string[] xxq = x.Split(',');
                    zile.Add(Int32.Parse(xxq[0]));
                    cereri.Add(Int32.Parse(xxq[1]));
                    points.AddXY("Zi " + xxq[0], Int32.Parse(xxq[1]));
                }
            }
            changed = false;
        }

        public void add(int cer, System.Windows.Forms.DataVisualization.Charting.DataPointCollection points)
        {
            int id = zile.Count;
            zile.Add(id);
            cereri.Add(cer);
            points.AddXY("Zi " + zile[id], cer);
            changed = true;
        }

        public void save()
        {
            if (nume != "" && zile.Count > 0 && changed)
            {
                string q = "";
                for (int i = 0; i < zile.Count; i++)
                {
                    q += "" + zile[i] + "," + cereri[i] + "|";
                }
                q = q.Substring(0, q.Length - 1);

                SqlCommand cmd = new SqlCommand("UPDATE Produse SET cerere = @param WHERE nume = '" + nume + "'");
                cmd.Parameters.AddWithValue("@param", q);
                Database.Execute(cmd);
            }
        }

    }

    public class Produs
    {
        public int id;
        public string name;
        public Produs() { }
        public Produs(int i, string n)
        {
            id = i;
            name = n;
        }
    }
}
