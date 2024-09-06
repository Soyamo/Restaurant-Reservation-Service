﻿using System;
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
    public partial class Form2 : Form
    {

        string conString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\restaurant_service.mdf;Integrated Security=True";
        public SqlDataAdapter adapter;
        public SqlConnection cnn;
        public DataSet ds;
        public SqlCommand cmd;

        public Form2()
        {
            InitializeComponent();
            tabControl1.SelectedIndex = 1;
        }

        private void lblLogin_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage2;
            lblLogin.ForeColor = Color.Red;
        }

        private void lblSignUp_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage1;
            lblSignUp.ForeColor = Color.Red;
        }

        private void btnUserLogin_Click(object sender, EventArgs e)
        {
            errorProvider1.SetError(txtUserPhoneEmailLogin, "");
            errorProvider2.SetError(txtUserLoginPass, "");

            string enteredEmail = txtUserPhoneEmailLogin.Text;
            string enteredPassword = txtUserLoginPass.Text;

            if (txtUserPhoneEmailLogin.Text != string.Empty)
            {
                if (txtUserLoginPass.Text != string.Empty)
                {
                    var con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\restaurant_service.mdf;Integrated Security=True");
                    try
                    {
                        con.Open();
                        // Use a parameterized query to prevent SQL injection
                        string sqlQuery = $"SELECT * FROM User_account WHERE email = '{enteredEmail}' AND password = '{enteredPassword}'";
                        SqlCommand sqlCommand = new SqlCommand(sqlQuery, con);
                        con.Close();

                        con.Open();
                        string userInfoQuery = $"SELECT name, surname, email, phone_number FROM User_account WHERE email = '{enteredEmail}' AND password = '{enteredPassword}'";

                        using (SqlCommand cmd = new SqlCommand(userInfoQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@email", enteredEmail);
                            cmd.Parameters.AddWithValue("@password", enteredPassword);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    // Get user information
                                    string name = reader["name"].ToString();
                                    string surname = reader["surname"].ToString();
                                    string email = reader["email"].ToString();
                                    string phone_number = reader["phone_number"].ToString();

                                    // Valid login, do something here (e.g., open a new form)
                                    MessageBox.Show("Login successful!");

                                    // Pass the user details to the other forms
                                    Form4 frm4 = new Form4(name, surname);
                                    Form5 frm5 = new Form5(name, surname, email, phone_number);
                                    Form6 frm6 = new Form6(name, surname, email, phone_number);
                                    Form7 frm7 = new Form7(name, surname, email, phone_number);

                                    frm4.Show();
                                    this.Close();
                                    this.Hide();
                                }
                                else
                                {
                                    // Invalid login, show an error message
                                    MessageBox.Show("Invalid login credentials. Please try again.");
                                }
                            }
                            con.Close();
                        }
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
                errorProvider2.SetError(txtUserLoginPass, "Please enter a valid password");
            }
            else
            {
                txtUserPhoneEmailLogin.Text = "";
                errorProvider1.SetError(txtUserPhoneEmailLogin, "Please enter a valid email address");
            }
        }

        private void btnAdminLogin_Click(object sender, EventArgs e)
        {
            errorProvider1.SetError(txtAdminID, "");
            errorProvider2.SetError(txtAdminPass, "");

            string entered_ID = txtAdminID.Text;
            string enteredPword = txtAdminPass.Text;

            if (txtAdminID.Text != string.Empty)
            {
                if (txtAdminPass.Text != string.Empty)
                {
                    var con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\restaurant_service.mdf;Integrated Security=True");
                    try
                    {
                        con.Open();
                        // Use a parameterized query to prevent SQL injection
                        string sqlQuery = "SELECT * FROM Admin WHERE Admin_id = @AdminID AND password = @Password";
                        SqlCommand sqlCommand = new SqlCommand(sqlQuery, con);

                        // Add parameters for Employee_ID and Password
                        sqlCommand.Parameters.AddWithValue("@AdminID", entered_ID);
                        sqlCommand.Parameters.AddWithValue("@Password", enteredPword);
                        SqlDataReader reader = sqlCommand.ExecuteReader();
                        if (reader.HasRows)
                        {
                            // Valid login, do something here (e.g., open a new form)
                            MessageBox.Show("Login successful!");

                            Form3 frm3 = new Form3();
                            frm3.Show();
                            this.Hide();
                        }
                        else
                        {
                            // Invalid login, show an error message
                            MessageBox.Show("Invalid login credentials. Please try again.");
                        }
                    }

                    catch (SqlException ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
                errorProvider2.SetError(txtAdminPass, "Please enter a valid password");
            }
            else
            {
                txtAdminID.Text = "";
                errorProvider1.SetError(txtAdminID, "Please enter a valid email address");
            }

        }
        
        private void btnSignUp_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(conString))
            {
                try
                { 
                    con.Open();

                    string fname = txtSignName.Text;
                    string lname = txtSignSurname.Text;
                    string email = txtSignEmail.Text;
                    string phone = txtSignCellno.Text;
                    string pass = txtSignCrPass.Text;
                    string pword = txtSignConPass.Text;

                    if (pass == pword)
                    {
                        string commandQuery = $"INSERT INTO User_account (name, surname, email, phone_number, password)  VALUES ('{fname}', '{lname}', '{email}', '{phone}','{pword}')";

                        cnn = new SqlConnection(conString);
                        cnn.Open();

                        string query = @"Select user_id FROM User_account";
                        cmd = new SqlCommand(query, cnn);
                        adapter = new SqlDataAdapter(cmd);
                        ds = new DataSet();

                        adapter.Fill(ds, "User_account");



                        using (SqlCommand sqlCommand = new SqlCommand(commandQuery, con))
                        {


                            int rowsAffected = sqlCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Account Created Successfully.");
                                tabControl1.SelectedTab = tabPage2;
                            }
                            else
                            {
                                MessageBox.Show("No records were inserted.");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Password does not match");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                    // Handle the exception as needed (log it, show a user-friendly message, etc.)
                }
            }
        }


        private void txtSignConPass_TextChanged(object sender, EventArgs e)
        {
            txtSignCrPass.PasswordChar = '*';
        }

        private void txtUserLoginPass_TextChanged(object sender, EventArgs e)
        {
            txtUserLoginPass.PasswordChar = '*';
        }

        private void txtSignCrPass_TextChanged(object sender, EventArgs e)
        {
            txtSignCrPass.PasswordChar = '*';
        }
    }
}
