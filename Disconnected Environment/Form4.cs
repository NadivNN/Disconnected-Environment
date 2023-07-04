using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Disconnected_Environment
{
    public partial class Form4 : Form
    {
        private string stringConnection = "data source=NADIVNN\\NADIV_NUGRAHA;" + "database=UNIV;User ID=sa;Password=123";
        private SqlConnection koneksi;
        public Form4()
        {
            InitializeComponent();
            koneksi = new SqlConnection(stringConnection);
            refreshform();
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void Form4_Load(object sender, EventArgs e)
        {

        }
        private void refreshform()
        {
            comboBox1.Enabled = false;
            comboBox3.Enabled = false;
            comboBox2.Enabled = false;
            comboBox1.SelectedIndex = -1;
            comboBox3.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            label5.Visible = false;
            button3.Enabled = false;
            button2.Enabled = false;
            button1.Enabled = false;
        }
        private void dataGridView()
        {
            koneksi.Open();
            string str = "select * from dbo.status_mahasiswa";
            SqlDataAdapter da = new SqlDataAdapter(str, koneksi);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0];
            koneksi.Close();
        }
        private void cbNama()
        {
            koneksi.Open();
            string str = "select nama_mahasiswa from dbo.Mahasiswa where " +
                "not EXISTS(select id_status from dbo.status_mahasiswa where " +
                "status_mahasiswa.nim = mahasiswa.nim)";
            SqlCommand cmd = new SqlCommand(str, koneksi);
            SqlDataAdapter da = new SqlDataAdapter(str, koneksi);
            DataSet ds = new DataSet();
            da.Fill(ds);
            cmd.ExecuteReader();
            koneksi.Close();

            comboBox1.DisplayMember = "nama_mahasiswa";
            comboBox1.ValueMember = "NIM";
            comboBox1.DataSource = ds.Tables[0];
        }
        private void cbTahunMasuk()
        {
            int y = DateTime.Now.Year - 2023;
            string[] type = new string[y];
            int i = 0;
            for (i = 0; i < type.Length; i++)
            {
                if (i == 0)
                {
                    comboBox2.Items.Add("2023");
                }
                else
                {
                    int l = 2023 + i;
                    comboBox2.Items.Add(l.ToString());
                }
            }
        }
        private void cbxNama_SelectedIndexChanged(object sender, EventArgs e)
        {
            koneksi.Open();
            string nim = "";
            string strs = "select NIM from dbo.Mahasiswa where nama_mahasiswa = @nm";
            SqlCommand cm = new SqlCommand(strs, koneksi);
            cm.CommandType = CommandType.Text;
            cm.Parameters.Add(new SqlParameter("@nm", comboBox1.Text));
            SqlDataReader dr = cm.ExecuteReader();
            while (dr.Read())
            {
                nim = dr["NIM"].ToString();
            }
            dr.Close();
            koneksi.Close();

            label5.Text = nim;
        }
        private void btnOpen_Click(object sendel, EventArgs e)
        {
            dataGridView();
            button4.Enabled = false;
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            comboBox2.Enabled = true;
            comboBox1.Enabled = true;
            comboBox3.Enabled = true;
            label5.Visible = true;
            cbTahunMasuk();
            cbNama();
            button2.Enabled = true;
            button3.Enabled = true;
            button1.Enabled = false;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            string nim = label5.Text;
            string statusMahasiswa = comboBox3.Text;
            string tahunMasuk = comboBox2.Text; // Added missing '=' symbol here
            int count = 0;
            string tempKodeStatus = "";
            string kodeStatus = "";
            koneksi.Open();

            // Check if there are any records in the 'status_mahasiswa' table
            string countQuery = "SELECT COUNT(*) FROM dbo.status_mahasiswa";
            SqlCommand cm = new SqlCommand(countQuery, koneksi);
            count = (int)cm.ExecuteScalar();

            if (count == 0)
            {
                kodeStatus = "1";
            }
            else
            {
                string maxIdQuery = "SELECT MAX(id_status) FROM dbo.status_mahasiswa";
                SqlCommand cmStatusMahasiswaSum = new SqlCommand(maxIdQuery, koneksi);
                int totalStatusMahasiswa = (int)cmStatusMahasiswaSum.ExecuteScalar();
                int finalKodeStatusInt = totalStatusMahasiswa + 1;
                kodeStatus = Convert.ToString(finalKodeStatusInt);
            }


            string insertQuery = "INSERT INTO dbo.status_mahasiswa (id_status, nim, status_mahasiswa, tahun_masuk) " +
                                 "VALUES (@ids, @NIM, @sm, @tm)";
            SqlCommand cmd = new SqlCommand(insertQuery, koneksi);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new SqlParameter("@ids", kodeStatus));
            cmd.Parameters.Add(new SqlParameter("@NIM", nim));
            cmd.Parameters.Add(new SqlParameter("@sm", statusMahasiswa));
            cmd.Parameters.Add(new SqlParameter("@tm", tahunMasuk));
            cmd.ExecuteNonQuery();
            koneksi.Close();

            MessageBox.Show("Data Berhasil Disimpan", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
            refreshform();
            dataGridView();
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            refreshform();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form4 fm = new Form4();
            fm.Show();
            this.Hide();
        }
    }
}
