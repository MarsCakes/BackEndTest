using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BackEndTest
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadEmployees();
                LoadAttendanceGrid();
                LoadAttendanceX();
                LoadAttendanceByMonth();
            }
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedNIK = DropDownList1.SelectedValue;
            string selectedNama = DropDownList1.SelectedItem.Text;
        }

        private void LoadEmployees()
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=BackendTestDB;Trusted_Connection=True;";
            string query = "SELECT NIK, (Nama + ' - ' + NIK) AS DisplayText FROM Employee";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                DropDownList1.DataSource = reader;
                DropDownList1.DataTextField = "DisplayText";
                DropDownList1.DataValueField = "NIK";
                DropDownList1.DataBind();
            }
        }

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = Calendar1.SelectedDate;
            TextBoxDate.Text = selectedDate.ToString("yyyy-MM-dd");
        }

        protected void ShowCalendar(object sender, EventArgs e)
        {
           
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string nik = DropDownList1.SelectedValue;        
            string tanggal = TextBoxDate.Text;              

            if (string.IsNullOrEmpty(nik) || string.IsNullOrEmpty(tanggal))
            {
                Response.Write("Please select employee and date.");
                return;
            }

            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=BackendTestDB;Trusted_Connection=True;";
            string query = "INSERT INTO absen (NIK, TanggalAbsen) VALUES (@NIK, @TanggalAbsen)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@NIK", nik);
                cmd.Parameters.AddWithValue("@TanggalAbsen", DateTime.Parse(tanggal));

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            Response.Write("Attendance saved!");
        }

        private void LoadAttendanceGrid()
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=BackendTestDB;Trusted_Connection=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT e.NIK, e.Nama, a.TanggalAbsen
                    FROM Absen a 
                    JOIN Employee e ON a.NIK = e.NIK
                    ORDER BY a.TanggalAbsen DESC";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
        }

        protected void LoadAttendanceX()
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=BackendTestDB;Trusted_Connection=True;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT e.NIK, e.Nama, a.TanggalAbsen
                    FROM Absen a
                    JOIN Employee e ON e.NIK = a.NIK
                    ORDER BY e.NIK, a.TanggalAbsen";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                DataTable pivot = new DataTable();
                pivot.Columns.Add("NIK");
                pivot.Columns.Add("Nama");
                foreach (var date in dt.AsEnumerable()
                     .Select(r => r.Field<DateTime>("TanggalAbsen"))
                     .Distinct()
                     .OrderBy(d => d))
                {
                    pivot.Columns.Add(date.ToString("yyyy-MM-dd"));
                }

                pivot.Columns.Add("Total", typeof(int));

                foreach (var emp in dt.AsEnumerable()
                         .GroupBy(r => new { NIK = r["NIK"], Nama = r["Nama"] }))
                {
                    DataRow row = pivot.NewRow();
                    row["NIK"] = emp.Key.NIK;
                    row["Nama"] = emp.Key.Nama;

                    int total = 0;

                    foreach (var date in pivot.Columns.Cast<DataColumn>()
                             .Where(c => c.ColumnName.Contains("-")))
                    {
                        bool hadir = emp.Any(r =>
                               ((DateTime)r["TanggalAbsen"]).ToString("yyyy-MM-dd")
                               == date.ColumnName);

                        if (hadir)
                        {
                            row[date.ColumnName] = "X";
                            total++;
                        }
                    }

                    row["Total"] = total;
                    pivot.Rows.Add(row);
                }

                GridView2.DataSource = pivot;
                GridView2.DataBind();
            }
        }

        protected void LoadAttendanceByMonth()
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=BackendTestDB;Trusted_Connection=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT 
                        e.NIK,
                        e.Nama,
                        FORMAT(a.TanggalAbsen, 'yyyyMM') AS TahunBulan
                    FROM Absen a
                    JOIN Employee e ON e.NIK = a.NIK
                    ORDER BY e.NIK, TahunBulan";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                DataTable pivot = new DataTable();
                pivot.Columns.Add("NIK");
                pivot.Columns.Add("Nama");

                var months = dt.AsEnumerable()
                    .Select(r => r.Field<string>("TahunBulan"))
                    .Distinct()
                    .OrderBy(s => s)
                    .ToList();

                foreach (var month in months)
                {
                    pivot.Columns.Add(month, typeof(int));
                }

                pivot.Columns.Add("Total", typeof(int));

                var groups = dt.AsEnumerable().GroupBy(
                    r => new
                    {
                        NIK = r.Field<string>("NIK"),
                        Nama = r.Field<string>("Nama")
                    });

                foreach (var g in groups)
                {
                    DataRow row = pivot.NewRow();
                    row["NIK"] = g.Key.NIK;
                    row["Nama"] = g.Key.Nama;

                    int total = 0;

                    foreach (var month in months)
                    {
                        int count = g.Count(r => r.Field<string>("TahunBulan") == month);
                        row[month] = count;
                        total += count;
                    }

                    row["Total"] = total;
                    pivot.Rows.Add(row);
                }

                GridView3.DataSource = pivot;
                GridView3.DataBind();
            }
        }

    }
}