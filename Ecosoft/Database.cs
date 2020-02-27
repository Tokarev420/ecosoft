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
    public class Database
    {
        public static void exportToExcel(DataGridView view, string sheetName, int columns, string path)
        {
            Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
            app.Visible = false;
            //worksheet = workbook.Sheets["Sheet1"];
            worksheet = workbook.ActiveSheet;
            worksheet.Name = sheetName;
            for (int i = 1; i < columns + 1; i++)
            {
                worksheet.Cells[1, i] = view.Columns[i - 1].HeaderText;
            }
            for (int i = 0; i < view.Rows.Count - 1; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (view.Rows[i].Cells[j].Value != null)
                    {
                        worksheet.Cells[i + 2, j + 1] = view.Rows[i].Cells[j].Value.ToString();
                    }
                    else
                    {
                        worksheet.Cells[i + 2, j + 1] = "";
                    }
                }
            }

            workbook.SaveAs(path);

            app.Quit();
        }


        public static string getConnStr()
        {
            string path = Application.StartupPath;
            //path = path.Substring(0, path.Length - 10);
            return @"Data Source=.\SQLEXPRESS;AttachDbFilename="+path+@"\EcosoftDb.mdf;Integrated Security=True;User Instance=True";
        }

        public static decimal getProfit(int id)
        {
            decimal price = decimal.Parse(Database.Value("SELECT pret FROM Produse WHERE id_produs = '"+id+"'"));
            return price - getPrice(id);
        }

        public static decimal getPrice(int id)
        {
            string materii = Value("SELECT materii FROM Produse WHERE id_produs = '"+id+"'");
            return Materie.getPrice(materii);
        }

        public static void Execute(string cmd)
        {
            using (SqlConnection conn = new SqlConnection(getConnStr()))
            {
                conn.Open();
                SqlCommand xcmd = new SqlCommand(cmd, conn);
                xcmd.ExecuteNonQuery();
            }
        }

        public static void Execute(SqlCommand cmd)
        {
            using (SqlConnection conn = new SqlConnection(getConnStr()))
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
        }

        public static DataTable Select(string select)
        {
            using (SqlConnection conn = new SqlConnection(getConnStr()))
            {
                conn.Open();
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(select, conn);
                da.Fill(dt);
                return dt;
            }
        }

        public static DataTable Select(SqlCommand select)
        {
            using (SqlConnection conn = new SqlConnection(getConnStr()))
            {
                conn.Open();
                DataTable dt = new DataTable();
                select.Connection = conn;
                SqlDataAdapter da = new SqlDataAdapter(select);
                da.Fill(dt);
                return dt;
            }
        }

        public static string Value(string select)
        {
            return Select(select).Rows[0][0].ToString();
        }

        public static bool Exists(string select)
        {
            return Select(select).Rows.Count > 0;
        }
    }
}
