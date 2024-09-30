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
            cbDeleteItem.Visible = true;
            cbDeleteReserve.Visible = true;
            cbDeleteUser.Visible = true;
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
                using (adapter = new SqlDataAdapter(query, cnn))
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
                string query = "INSERT INTO User_account (name, surname, email, phone_number, password) VALUES (@Name, @Sname, @Email, @PhoneNo, @Password)";
                using (SqlConnection cnn = new SqlConnection(conString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, cnn))
                    {
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
                    string query = "UPDATE User_account SET name = @Name, surname = @Sname, email = @Email, phone_number = @PhoneNo, password = @Password WHERE user_id = @user_id";
                    using (SqlConnection cnn = new SqlConnection(conString))
                    {
                        cnn.Open();

                        using (SqlCommand cmd = new SqlCommand(query, cnn))
                        {
                            cmd.Parameters.AddWithValue("@Name", txtName_Admin.Text);
                            cmd.Parameters.AddWithValue("@Sname", txtSurname_Admin.Text);
                            cmd.Parameters.AddWithValue("@Email", txtEmail_Admin.Text);
                            cmd.Parameters.AddWithValue("@PhoneNo", txtPhoneNo_Admin.Text);
                            cmd.Parameters.AddWithValue("@Password", txtPassword_Admin.Text);
                            cmd.Parameters.AddWithValue("@user_id", selectedId);


                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                // The UPDATE was successful
                                // You can add code here to handle success
                                MessageBox.Show("User Details updated successfully!");
                                Reload();
                            }
                            else
                            {
                                // The UPDATE failed
                                // You can add code here to handle failure
                                MessageBox.Show("No records were updated. User Details not found.");
                            }
                            cnn.Close();
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
            try
            {
                using (SqlConnection cnn = new SqlConnection(conString))
                {
                    cnn.Open();

                    // Delete all entries from the User_account table
                    if (cbDeleteUser.Checked)
                    {
                        string user_query = "DELETE FROM User_account";
                        using (SqlCommand cmd2 = new SqlCommand(user_query, cnn))
                        {
                            cmd2.ExecuteNonQuery();
                        }
                        MessageBox.Show("All User_account entries deleted successfully!");
                    }
                    else
                    {
                        MessageBox.Show("Click on the checkbox to confirm!");
                    }

                    Reload();
                    txtUserID_Admin.Text = " ";
                }
            }
            catch (SqlException er)
            {
                MessageBox.Show($"SQL Error: {er.Message}");
            }

        }

        private void Reload_Items()
        {
            string query = "SELECT * FROM MenuItems";
            DataTable dataTable = new DataTable();
            using (cnn = new SqlConnection(conString))
            {
                using (adapter = new SqlDataAdapter(query, cnn))
                {
                    cnn.Open();
                    adapter.Fill(dataTable);
                }
            }
            dataGridView2.DataSource = dataTable;
        }

        private void btnAddNewItem_Click(object sender, EventArgs e)
        {
            try
            {
                string query = "INSERT INTO MenuItems ( restaurant_id, name, description, price) VALUES (@Restaurant_id, @Item_Name, @Description, @Item_price)";
                using (SqlConnection cnn = new SqlConnection(conString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, cnn))
                    {
                        cmd.Parameters.AddWithValue("@Restaurant_id", cBoxResID_Admin.Text);
                        cmd.Parameters.AddWithValue("@Item_Name", txtItemName_Admin.Text);
                        cmd.Parameters.AddWithValue("@Description", txtResDescr_Admin.Text);
                        cmd.Parameters.AddWithValue("@Item_price", txtItemPrice_Admin.Text);
                        cnn.Open();
                        cmd.ExecuteNonQuery();
                        cnn.Close();
                        MessageBox.Show("Menu Item Added successfully!");
                        Reload_Items();
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

        private void btnUpdateItem_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedId = int.Parse(txtItemID_Admin.Text);
                if (selectedId > 0)
                {
                    string query = "UPDATE MenuItems SET restaurant_id = @Restaurant_id, name = @Item_Name, description = @Description, price = @Item_price WHERE item_id = @Item_ID";
                    using (SqlConnection cnn = new SqlConnection(conString))
                    {
                        cnn.Open();

                        using (SqlCommand cmd = new SqlCommand(query, cnn))
                        {
                            cmd.Parameters.AddWithValue("@Restaurant_id", cBoxResID_Admin.Text);
                            cmd.Parameters.AddWithValue("@Item_Name", txtItemName_Admin.Text);
                            cmd.Parameters.AddWithValue("@Description", txtResDescr_Admin.Text);
                            cmd.Parameters.AddWithValue("@Item_price", txtItemPrice_Admin.Text);
                            cmd.Parameters.AddWithValue("@item_id", selectedId);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                // The UPDATE was successful
                                // You can add code here to handle success
                                MessageBox.Show("Menu Items updated successfully!");
                                Reload_Items();
                            }
                            else
                            {
                                // The UPDATE failed
                                // You can add code here to handle failure
                                MessageBox.Show("No records were updated.Menu Item not found.");
                            }
                            cnn.Close();

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
                MessageBox.Show("Please enter a valid number for Menu Item ID.");
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

        private void btnDeleteItem_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedId = int.Parse(txtItemID_Admin.Text);
                if (selectedId > 0)
                {
                    using (SqlConnection cnn = new SqlConnection(conString))
                    {
                        cnn.Open();

                        string item_query = "Delete From MenuItems WHERE item_id = @item_id";
                        using (SqlCommand cmd2 = new SqlCommand(item_query, cnn))
                        {
                            cmd2.Parameters.AddWithValue("@item_id", selectedId);
                            cmd2.ExecuteNonQuery();
                        }

                        MessageBox.Show("Menu Items deleted successfully!");

                        Reload_Items();
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

        private void btnDeleteAllItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection cnn = new SqlConnection(conString))
                {
                    cnn.Open();

                    // Delete all entries from the User_account table
                    if (cbDeleteItem.Checked)
                    {
                        string user_query = "DELETE FROM MenuItem";
                        using (SqlCommand cmd2 = new SqlCommand(user_query, cnn))
                        {
                            cmd2.ExecuteNonQuery();
                        }
                        MessageBox.Show("All Menu Items entries deleted successfully!");
                    }
                    else
                    {
                        MessageBox.Show("Click on the checkbox to confirm!");
                    }

                    Reload_Items();
                    txtItemID_Admin.Text = " ";
                }
            }
            catch (SqlException er)
            {
                MessageBox.Show($"SQL Error: {er.Message}");
            }
        }

        private void Reload_Reservation()
        {
            string query = "SELECT * FROM Reservations";
            DataTable dataTable = new DataTable();
            using (cnn = new SqlConnection(conString))
            {
                using (adapter = new SqlDataAdapter(query, cnn))
                {
                    cnn.Open();
                    adapter.Fill(dataTable);
                }
            }
            dataGridView3.DataSource = dataTable;
        }

        private void btnAddRSVP_Click(object sender, EventArgs e)
        {
            try
            {
                string query = "INSERT INTO Reservations (user_id, restaurant_id, reservation_date, reservation_time, number_of_people, reservation_type, special_requests, rsvp_price) VALUES (@RSVP_UserID, @RSVP_ResID, @RSVP_date, @RSVP_Time, @No_Of_Guests, @Event_Type, @Special_req, @RSVP_Price)";
                using (SqlConnection cnn = new SqlConnection(conString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, cnn))
                    {

                        DateTime reservationDate = DateTime.Parse(datePicker.Text);
                        DateTime reservationTime = DateTime.Parse(cBoxTime.Text);

                        cmd.Parameters.AddWithValue("@RSVP_UserID", cBoxRSVP_User_id.Text);
                        cmd.Parameters.AddWithValue("@RSVP_ResID", cBoxResID.Text);
                        cmd.Parameters.AddWithValue("@RSVP_date", reservationDate);
                        cmd.Parameters.AddWithValue("@RSVP_Time", reservationTime);
                        cmd.Parameters.AddWithValue("@No_Of_Guests", int.Parse(txtNoGuestsAdmin.Text));
                        cmd.Parameters.AddWithValue("@Event_Type", txtEventType.Text);
                        cmd.Parameters.AddWithValue("@Special_req", txtSpecReq.Text);
                        cmd.Parameters.AddWithValue("@RSVP_Price", decimal.Parse(txtRSVP_Price.Text));
                        cnn.Open();
                        cmd.ExecuteNonQuery();
                        cnn.Close();
                        MessageBox.Show("Reservation Added successfully!");
                        Reload_Reservation();
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

        private void btnUpdateRSVP_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedId = int.Parse(txtRsvpID.Text);
                if (selectedId > 0)
                {
                    string query = "UPDATE Reservations SET user_id = @RSVP_UserID, restaurant_id = @RSVP_ResID, reservation_date = @RSVP_date, reservation_time = @RSVP_Time, number_of_people = @No_Of_Guests, reservation_type = @Event_Type, special_requests = @Special_req, rsvp_price = @RSVP_Price WHERE reservation_id = @RSVP_ID";
                    using (SqlConnection cnn = new SqlConnection(conString))
                    {
                        cnn.Open();

                        using (SqlCommand cmd = new SqlCommand(query, cnn))
                        {
                            DateTime reservationDate = DateTime.Parse(datePicker.Text);
                            DateTime reservationTime = DateTime.Parse(cBoxTime.Text);

                            cmd.Parameters.AddWithValue("@RSVP_UserID", cBoxRSVP_User_id.Text);
                            cmd.Parameters.AddWithValue("@RSVP_ResID", cBoxResID.Text);
                            cmd.Parameters.AddWithValue("@RSVP_date", reservationDate);
                            cmd.Parameters.AddWithValue("@RSVP_Time", reservationTime);
                            cmd.Parameters.AddWithValue("@No_Of_Guests", int.Parse(txtNoGuestsAdmin.Text));
                            cmd.Parameters.AddWithValue("@Event_Type", txtEventType.Text);
                            cmd.Parameters.AddWithValue("@Special_req", txtSpecReq.Text);
                            cmd.Parameters.AddWithValue("@RSVP_Price", decimal.Parse(txtRSVP_Price.Text));
                            cmd.Parameters.AddWithValue("@RSVP_ID", selectedId);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                // The UPDATE was successful
                                // You can add code here to handle success
                                MessageBox.Show("Reservation updated successfully!");
                                Reload_Reservation();
                            }
                            else
                            {
                                // The UPDATE failed
                                // You can add code here to handle failure
                                MessageBox.Show("No records were updated.Reservation not found.");
                            }
                            cnn.Close();

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
                MessageBox.Show("Please enter a valid number for Reservation ID.");
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

        private void btnDeleteRSVP_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedId = int.Parse(txtRsvpID.Text);
                if (selectedId > 0)
                {
                    using (SqlConnection cnn = new SqlConnection(conString))
                    {
                        cnn.Open();

                        string item_query = "Delete From Reservations WHERE reservation_id = @reservation_id";
                        using (SqlCommand cmd2 = new SqlCommand(item_query, cnn))
                        {
                            cmd2.Parameters.AddWithValue("@reservation_id", selectedId);
                            cmd2.ExecuteNonQuery();
                        }

                        MessageBox.Show("Reservation deleted successfully!");

                        Reload_Reservation();
                        txtRsvpID.Text = " ";
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

        private void btnDeleteAllRSVP_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection cnn = new SqlConnection(conString))
                {
                    cnn.Open();

                    // Delete all entries from the User_account table
                    if (cbDeleteReserve.Checked)
                    {
                        string user_query = "DELETE FROM Reservations";
                        using (SqlCommand cmd2 = new SqlCommand(user_query, cnn))
                        {
                            cmd2.ExecuteNonQuery();
                        }
                        MessageBox.Show("All Reservations records have been deleted successfully!");
                    }
                    else
                    {
                        MessageBox.Show("Click on the checkbox to confirm!");
                    }

                    Reload_Reservation();
                    txtRsvpID.Text = " ";
                }
            }
            catch (SqlException er)
            {
                MessageBox.Show($"SQL Error: {er.Message}");
            }


        }

        private void tabPage1_Click(object sender, EventArgs e)
        {
            cbDeleteUser.Visible = true;
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {
            cbDeleteItem.Visible = true;
         
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {
            cbDeleteReserve.Visible = true;

        }
    }
}
