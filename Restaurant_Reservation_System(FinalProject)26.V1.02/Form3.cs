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
using System.CodeDom;
using System.Diagnostics.Eventing.Reader;

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
            // TODO: This line of code loads data into the 'restaurant_serviceDataSet6.Reservations' table. You can move, or remove it, as needed.
            this.reservationsTableAdapter1.Fill(this.restaurant_serviceDataSet6.Reservations);
            // TODO: This line of code loads data into the 'restaurant_serviceDataSet4.Reservations' table. You can move, or remove it, as needed.
            //this.reservationsTableAdapter.Fill(this.restaurant_serviceDataSet4.Reservations);
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
                var selectedRow = cbRSVPfilter.SelectedItem as DataRowView;
                if (selectedRow != null)
                {
                    int userId = (int)selectedRow["user_id"]; // Assuming 'user_id' is the column name

                    string sql = "SELECT * FROM Reservations WHERE user_id = @user_id";

                    adapter = new SqlDataAdapter();
                    cmd = new SqlCommand(sql, cnn);
                    cmd.Parameters.AddWithValue("@user_id", userId);
                    adapter.SelectCommand = cmd;
                    ds = new DataSet();
                    adapter.Fill(ds, "Reservations");

                    dataGridView2.DataSource = ds;
                    dataGridView2.DataMember = "Reservations";
                }
            }
            catch (SqlException er)
            {
                MessageBox.Show(er.Message);
            }
            finally
            {
                if (cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                }
            }
        }
        //Admin Update, Delete and Add buttons for user details
        private void Reload()
        {
            string query = "SELECT * FROM User_account";
            DataTable dataTable = new DataTable();
            using (cnn = new SqlConnection(conString))
            {
                using (adapter = new SqlDataAdapter(query,cnn))
                {
                    cnn.Open();
                    adapter.Fill(dataTable);
                }
            }
            dataGridView1.DataSource = dataTable;
        }
        private void btnDeleteUserDetails_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedId = int.Parse(txtUserID_Admin.Text);
                if (selectedId > 0)
                {
                    using (SqlConnection cnn = new SqlConnection(conString))
                    {
                        cnn.Open();
                        string rsvp_query = "Delete From Reservations Where user_id = @user_id";
                        using (SqlCommand cmd1 = new SqlCommand(rsvp_query, cnn))
                        {
                            cmd1.Parameters.AddWithValue("@user_id", selectedId);
                            cmd1.ExecuteNonQuery();
                        }
                        string user_query = "Delete From User_account Where user_id = @user_id";
                        using (SqlCommand cmd2 = new SqlCommand(user_query, cnn))
                        {
                            cmd2.Parameters.AddWithValue("@user_id", selectedId);
                            cmd2.ExecuteNonQuery();
                        }

                        MessageBox.Show("User details deleted successfully!");

                        Reload();
                        txtItemID_Admin.Text = " ";
                    }
                }
                else
                {
                    MessageBox.Show("ID is required");
                }
            }
            catch (SqlException er)
            {
                MessageBox.Show($"SQL Error: {er.Message}");
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter a valid ID number.");
            }

        }
        private void btnAddUserDetails_Click(object sender, EventArgs e)
        {
            try
            {
                string query = "INSERT INTO User_account (user_id, name, surname, email, phone_number, password) VALUES (@user_id, @Name, @Sname, @Email, @PhoneNo, @Password)";
                using (SqlConnection cnn = new SqlConnection(conString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, cnn))
                    {
                        cmd.Parameters.AddWithValue("@user_id", txtUserID_Admin.Text);
                        cmd.Parameters.AddWithValue("@Name", txtName_Admin.Text);
                        cmd.Parameters.AddWithValue("@Sname", txtSurname_Admin.Text);
                        cmd.Parameters.AddWithValue("@Email", txtEmail_Admin.Text);
                        cmd.Parameters.AddWithValue("@PhoneNo", txtPhoneNo_Admin.Text);
                        cmd.Parameters.AddWithValue("@Password", txtPassword_Admin.Text);
                        cnn.Open();
                        cmd.ExecuteNonQuery();
                        cnn.Close();
                        MessageBox.Show("User Details Added successfully!");
                        Reload();
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"SQL Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
    
        private void btnUpdateUserDetails_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedId = int.Parse(txtUserID_Admin.Text);
                if (selectedId > 0)
                {
                    string query = "UPDATE User_account SET Name = @Name, surname = @Sname, email = @Email, phone_number = @PhoneNo, password = @Password WHERE user_id = @user_id";
                    using (SqlConnection cnn = new SqlConnection(conString))
                    {
                        using (SqlCommand cmd = new SqlCommand(query, cnn))
                        {
                            cmd.Parameters.AddWithValue("@Name", txtName_Admin.Text);
                            cmd.Parameters.AddWithValue("@Sname", txtSurname_Admin.Text);
                            cmd.Parameters.AddWithValue("@Email", txtEmail_Admin.Text);
                            cmd.Parameters.AddWithValue("@PhoneNo", txtPhoneNo_Admin.Text);
                            cmd.Parameters.AddWithValue("@Password", txtPassword_Admin.Text);
                            cmd.Parameters.AddWithValue("@user_id", selectedId);
                            cnn.Open();
                            cmd.ExecuteNonQuery();
                            cnn.Close();
                            MessageBox.Show("User Details updated successfully!");
                            Reload();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Enter valid ID to update!");
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter a valid number for User ID.");
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"SQL Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
        private void btnDeleteAllUsers_Click(object sender, EventArgs e)
        {

        }
    }
}
