using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Restaurant_Reservation_System_FinalProject_26
{
    public partial class Form3 : Form
    {
        string conString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\restaurant_service.mdf;Integrated Security=True";
        public SqlDataAdapter adapter;
        public SqlConnection cnn;
        public DataSet ds;
        public SqlCommand cmd;
        public Form3()
        {
            InitializeComponent();
        }

        private void btnAdminLogOff_Click(object sender, EventArgs e)
        {
            Form2 frm2 = new Form2();
            frm2.Show();
            this.Close();
        }

        private void btnAdminLogOff2_Click(object sender, EventArgs e)
        {
            Form2 frm2 = new Form2();
            frm2.Show();
            this.Close();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'restaurant_serviceDataSet4.Reservations' table. You can move, or remove it, as needed.
            this.reservationsTableAdapter.Fill(this.restaurant_serviceDataSet4.Reservations);
            // TODO: This line of code loads data into the 'restaurant_serviceDataSet3.User_account' table. You can move, or remove it, as needed.
            this.user_accountTableAdapter.Fill(this.restaurant_serviceDataSet3.User_account);
            // TODO: This line of code loads data into the 'restaurant_serviceDataSet2.MenuItems' table. You can move, or remove it, as needed.
            this.menuItemsTableAdapter.Fill(this.restaurant_serviceDataSet2.MenuItems);
            // TODO: This line of code loads data into the 'restaurant_serviceDataSet4.User_account' table. You can move, or remove it, as needed.
            this.user_accountTableAdapter.Fill(this.restaurant_serviceDataSet3.User_account);
            // TODO: This line of code loads data into the 'restaurant_serviceDataSet3.User_account' table. You can move, or remove it, as needed.
            this.user_accountTableAdapter.Fill(this.restaurant_serviceDataSet3.User_account);
            // TODO: This line of code loads data into the 'restaurant_serviceDataSet2.MenuItems' table. You can move, or remove it, as needed.
            this.menuItemsTableAdapter.Fill(this.restaurant_serviceDataSet2.MenuItems);

            

            cnn = new SqlConnection(conString);

          
        }


        private void cbResFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                
                if (cnn.State != ConnectionState.Open)
                    cnn.Open();
                string sql = "";
                if (cbResFilter.SelectedItem.ToString() == "Vino Santo")
                {
                    sql = $"SELECT * FROM MenuItems WHERE restaurant_id = {2} ";
                }
                else if (cbResFilter.SelectedItem.ToString() == "Pace")
                {
                    sql = $"SELECT * FROM MenuItems WHERE restaurant_id = {4} ";
                }
                else if (cbResFilter.SelectedItem.ToString() == "Asoka")
                {
                    sql = $"SELECT * FROM MenuItems WHERE restaurant_id = {5} ";
                }
                else
                {
                    sql = $"SELECT * FROM MenuItems ";
                }
                adapter = new SqlDataAdapter();
                cmd = new SqlCommand(sql, cnn);
                adapter.SelectCommand = cmd;
                ds = new DataSet();
                adapter.Fill(ds, "MenuItems");

                dataGridView2.DataSource = ds;
                dataGridView2.DataMember = "MenuItems";
                cnn.Close();
                MessageBox.Show("Restaurant Selected!");
            }
            catch (SqlException er)
            {
                MessageBox.Show(er.Message);
            }
            finally
            {
                cnn.Close();
            }
        }

        private void cbUserFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void btnLogOff3Admin_Click(object sender, EventArgs e)
        {
            Form2 frm2 = new Form2();
            frm2.Show();
            this.Close();
        }

        private void cbRSVPfilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                if (cnn.State != ConnectionState.Open)
                    cnn.Open();
                string sql = "";

                var selectedRow = cbRSVPfilter.SelectedItem as DataRowView;

                // Access the specific column from the DataRowView
                if (selectedRow != null)
                {
                    int userId = (int)selectedRow["user_id"]; // Assuming 'user_id' is the column name

                    sql = $"SELECT * FROM Reservations WHERE user_id = {userId}";
                }

                adapter = new SqlDataAdapter();
                cmd = new SqlCommand(sql, cnn);
                adapter.SelectCommand = cmd;
                ds = new DataSet();
                adapter.Fill(ds, "Reservations");

                dataGridView2.DataSource = ds;
                dataGridView2.DataMember = "Reservations";
                cnn.Close();
                MessageBox.Show("Reservation Selected!");
            }
            catch (SqlException er)
            {
                MessageBox.Show(er.Message);
            }
            finally
            {
                cnn.Close();
            }
        }
    }
}
