﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Linq.Expressions;
using System.Diagnostics;

namespace Restaurant_Reservation_System_FinalProject_26
{
    public partial class Form5 : Form
    {
        string conString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\restaurant_service.mdf;Integrated Security=True";
        SqlDataAdapter adapter;
        SqlConnection cnn;
        DataSet ds;
        SqlCommand cmd;
        decimal totalPrice = 0m;

        private int restaurant_id;
        private int totalSeatsAvailable = 0;
        private int userId;

        public Form5()
        {
            InitializeComponent();
        }

        public Form5(string name, string surname, string email, string phone_number)
        {
            InitializeComponent();

            pbDeserts_Vino.SizeMode = PictureBoxSizeMode.StretchImage;
            pbAppetizers_Vino.SizeMode = PictureBoxSizeMode.StretchImage;
            pbMain_Vino.SizeMode = PictureBoxSizeMode.StretchImage;
            pbVino_pay.SizeMode = PictureBoxSizeMode.StretchImage;
            pbCocktails_Vino.SizeMode = PictureBoxSizeMode.StretchImage;
            pbHotBev_Vino.SizeMode = PictureBoxSizeMode.StretchImage;
            pbWines_Vino.SizeMode = PictureBoxSizeMode.StretchImage;
            pbNonBev_Vino.SizeMode = PictureBoxSizeMode.StretchImage;

            label21.Hide();
            rtxtAllergies_Vino.Hide();
            cbTime_Vino.SelectedIndex = 0;
            cbReserveType_Vino.SelectedIndex = 0;
            //----------------------------------------------------------------

            txtFName_Vino.Text = name;
            LName_Vino.Text = surname;
            txtEmail_Vino.Text = email;
            txtPhone_Vino.Text = phone_number;
            numericUpDown_Vino.Value = 0;

            using (cnn = new SqlConnection(conString))
            {
                try
                {
                    cnn.Open();
                    // Query to retrieve reservation details from the database using the email
                    string selectQuery = "Select reservation_date, reservation_time, number_of_people, reservation_type, special_requests From Reservations Where user_id = (Select user_id From User_account Where email = @user_email)";
                    using (SqlCommand selectCmd = new SqlCommand(selectQuery, cnn))
                    {
                        // Add the email parameter to avoid SQL injection
                        selectCmd.Parameters.AddWithValue("@user_email", email);
                        using (SqlDataReader reader = selectCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Read and populate reservation details into the form controls
                                int numPeople = reader["number_of_people"] != DBNull.Value ? Convert.ToInt32(reader["number_of_people"]) : 0;
                                // Set the class-level available seats variable
                                totalSeatsAvailable = numPeople;
                                // Set the NumericUpDown control to 0 initially
                                numericUpDown_Vino.Value = 0;
                                // Reservation type and other details
                                cbReserveType_Vino.SelectedItem = reader["reservation_type"]?.ToString();
                                TimeSpan time = reader["reservation_time"] != DBNull.Value ? (TimeSpan)reader["reservation_time"] : TimeSpan.Zero;
                                string timeString = time.ToString(@"hh\:mm\:ss");
                                cbTime_Vino.SelectedItem = timeString;
                                string specialRequest = reader["special_requests"]?.ToString();
                                cbRequests_Vino.SelectedItem = specialRequest;
                                // Bold the reservation date in the calendar
                                if (reader["reservation_date"] != DBNull.Value)
                                {
                                    DateTime date = (DateTime)reader["reservation_date"];
                                    cd_vino.BoldedDates = new DateTime[] { date };
                                }
                                // Set the initial label showing total seats available
                                lblNoOfSeats_Vino.Text = $"Seats Left: {totalSeatsAvailable}";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
                finally
                {
                    if (cnn.State == ConnectionState.Open)
                    {
                        cnn.Close();
                    }
                }
            }

            // Subscribe to the ValueChanged event of the NumericUpDown
            numericUpDown_Vino.ValueChanged += numericUpDown_Vino_ValueChanged;
        }
        //==============================================================================
        //(Vino Santo Buttons that allow to movement from one page to another)

        //1.(Submit Reservation Details)
        private void btnSubmit_Vino_Click(object sender, EventArgs e)
        {
            decimal rsvpCost, specialRequestCost = 0, menuItemCost = 0;
            try
            {
                using (var cnn = new SqlConnection(conString))
                {
                    cnn.Open();

                    // Query to retrieve the user's ID from the database
                    string selectQuery = "SELECT user_id FROM User_account WHERE email = @email";
                    using (var selectCmd = new SqlCommand(selectQuery, cnn))
                    {
                        selectCmd.Parameters.AddWithValue("@email", txtEmail_Vino.Text);
                        object result = selectCmd.ExecuteScalar();
                        this.userId = result != null ? (int)result : throw new Exception("User not found.");
                    }

                    // Retrieve the rsvp_cost from the database
                    string selectRsvpCostQuery = "SELECT rsvp_price FROM Reservations WHERE user_id = @user_id";
                    using (var selectRsvpCostCmd = new SqlCommand(selectRsvpCostQuery, cnn))
                    {
                        selectRsvpCostCmd.Parameters.AddWithValue("@user_id", this.userId);
                        object rsvpResult = selectRsvpCostCmd.ExecuteScalar();
                        rsvpCost = rsvpResult != null ? (decimal)rsvpResult : throw new Exception("RSVP cost not found.");
                    }

                    // Retrieve the number of people from the database
                    string selectNumPeopleQuery = "SELECT number_of_people FROM Reservations WHERE user_id = @user_id";
                    using (var selectNumPeopleCmd = new SqlCommand(selectNumPeopleQuery, cnn))
                    {
                        selectNumPeopleCmd.Parameters.AddWithValue("@user_id", this.userId);
                        object numPeopleResult = selectNumPeopleCmd.ExecuteScalar();
                        int numPeople = numPeopleResult != null ? Convert.ToInt32(numPeopleResult) : 0;

                        // Calculate the reservation cost
                        decimal reservationCost = rsvpCost * numPeople;

                        // Calculate special request cost
                        var specialRequestCosts = new Dictionary<string, decimal>
                {
                    { "Projector required", 50 },
                    { "Auction setup", 100 },
                    { "Slide show", 20 },
                    { "Outdoor seating", 30 },
                    { "Private dining area", 40 },
                    { "Red carpet entry", 60 },
                    { "Balcony seating", 70 },
                    { "Candlelight dinner", 80 },
                    { "Private section", 90 }
                };

                        if (cbRequests_Vino.SelectedItem != null &&
                            specialRequestCosts.TryGetValue(cbRequests_Vino.SelectedItem.ToString(), out specialRequestCost))
                        {
                            // Cost found, use it
                        }

                        // Calculate menu item cost
                        foreach (var item in lbOrder_Vino.Items)
                        {
                            string listItem = item.ToString();
                            int dashIndex = listItem.IndexOf('-');
                            if (dashIndex != -1)
                            {
                                string priceString = listItem.Substring(dashIndex + 1).Trim().Replace("$", "");
                                if (decimal.TryParse(priceString, out decimal price))
                                {
                                    menuItemCost += price;
                                }
                            }
                        }

                        tabControl1.SelectedTab = tabPage3;
                        // Update the total cost
                        UpdateTotal(reservationCost, specialRequestCost, menuItemCost);
                        UpdateSummary();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                // Consider logging the exception details here
            }
            finally
            {
                if (cnn != null && cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                }
            }
        }
        //2.(Payment of reservation and menu items)
        private void btnPay_Vino_Click(object sender, EventArgs e)
        {
            string name = txtcrdholder_Vino.Text;
            string cvv = txtCVV_Vino.Text;
            string cardNumber = txtcrdNo_Vino.Text;
            bool isValid = true;

            // Validate cardholder's name
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Name cannot be empty.");
                isValid = false;
            }
            else
            {
                foreach (char c in name)
                {
                    if (!char.IsLetter(c) && !char.IsWhiteSpace(c))
                    {
                        MessageBox.Show("Name can only contain letters and spaces.");
                        isValid = false;
                        break;
                    }
                }
            }
            // Validate CVV
            if (isValid)
            {
                if (cvv.Length == 3 && int.TryParse(cvv, out _))
                {
                    // CVV is valid
                }
                else
                {
                    MessageBox.Show("Invalid CVV. Please enter exactly 3 digits.");
                    isValid = false;
                }
            }
            // Validate Card Number
            if (isValid)
            {
                if (cardNumber.Length == 16 && long.TryParse(cardNumber, out _))
                {
                    // Card number is valid
                }
                else
                {
                    MessageBox.Show("Invalid Card Number. Please enter exactly 16 digits.");
                    isValid = false;
                }
            }
            // Proceed if all validations are successful
            if (isValid)
            {
                MessageBox.Show("Transaction was successful!!!");
                tabControl1.SelectedTab = tabPage5;
            }
        }
        //3.(Back button)
        private void btnBackVino_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage3;
        }
        //4.(Button back to Home page)
        private void btnHomeVino_Click(object sender, EventArgs e)
        {
            Form4 frm4 = new Form4();
            frm4.Show();
            this.Close();
        }
        //5.(Back button)
        private void btnBack_pg2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage2;
        }
        //6.(Next button at Vino Santo menu that takes you to the drinks session)
        private void btnNext_Vino_Click(object sender, EventArgs e)
        {
            decimal totalPrice = 0m;

            // Iterate through all items in the lbOrder_Vino ListBox
            foreach (var item in lbOrder_Vino.Items)
            {
                // Check if the item contains the price part after the dash
                string listItem = item.ToString();
                int dashIndex = listItem.IndexOf('-');

                if (dashIndex != -1)
                {
                    // Extract the price part after the dash and remove the '$' sign
                    string priceString = listItem.Substring(dashIndex + 1).Trim().Replace("$", "");

                    // Try to convert the price part to decimal
                    if (decimal.TryParse(priceString, out decimal price))
                    {
                        totalPrice += price; // Sum the prices
                    }
                }
            }

            // Display the total price in a label on tabPage6
            String price1 = $"Total Price: ${totalPrice:F2}";
            lbOrder_Vino.Items.Add(price1);

            // Navigate to the next tab
            tabControl2.SelectedTab = tabPage6;
        }

        //7.(Button that submits orders from the menu item)
        private void btnSubmitOrder_Vino_Click(object sender, EventArgs e)
        {
            // Check if there are items in the order list
            if (lbOrder_Vino.Items.Count > 0)
            {
                MessageBox.Show("Successfully submitted order!!!");

                // Display subtotal and calculate total including tax
                lblSubTotal_Vino.Text = $"{totalPrice:F2}";
                decimal taxPerc = 0.15m; // Use decimal for accurate financial calculations
                decimal taxAmount = totalPrice * taxPerc; // Calculate the tax amount
                decimal totalAmount = totalPrice + taxAmount; // Calculate the total amount including tax
                lblTotal_Vino.Text = $"{totalAmount:F2}";

                // Navigate to tabPage4 to show confirmation message
                tabControl1.SelectedTab = tabPage4;
            }
            else
            {
                // Display a message if no items were selected
                MessageBox.Show("No order item from menu!!!");
            }

            // Always update and display the summary on tabPage6, even if no new items are selected
            tabControl2.SelectedTab = tabPage6;
            UpdateSummary(); // Ensure the summary reflects the current order from all previous tabs
        }
        //8.(Button that skips the menu section)
        private void btnSkip_Vino_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage4;
        }
        //9.(Button that takes you back to the food section)
        private void btnBack1_Vino_Click(object sender, EventArgs e)
        {
            tabControl2.SelectedTab = tabPage1;
        }
        //==============================================================================

        //==============================================================================
        //10. (Menu Items for the Vino Santo Restaurant PictureBox(s) & CheckBox(s))

        //(Vino Santo Deserts)
        private void cbDesert1_Vino_CheckedChanged(object sender, EventArgs e)
        {
            String dessertName = cbDesert1_Vino.Text;
            pbDeserts_Vino.Visible = true;
            pbDeserts_Vino.Image = Properties.Resources.CremeBrulee;

            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", dessertName);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{dessertName} - ${price:F2}";

                        // Add the product name and price to the list bo
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbDesert2_Vino_CheckedChanged(object sender, EventArgs e)
        {
            String dessertName = cbDesert2_Vino.Text;
            pbDeserts_Vino.Visible = true;
            pbDeserts_Vino.Image = Properties.Resources.tiramisu;

            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", dessertName);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{dessertName} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbDesert3_Vino_CheckedChanged(object sender, EventArgs e)
        {
            String dessertName = cbDesert3_Vino.Text;
            pbDeserts_Vino.Visible = true;
            pbDeserts_Vino.Image = Properties.Resources.Choc_Fondant;

            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", dessertName);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{dessertName} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbDesert4_Vino_CheckedChanged(object sender, EventArgs e)
        {
            String dessertName = cbDesert4_Vino.Text;
            pbDeserts_Vino.Visible = true;
            pbDeserts_Vino.Image = Properties.Resources.PannaCotta;

            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", dessertName);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{dessertName} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbDesert5_Vino_CheckedChanged(object sender, EventArgs e)
        {
            String dessertName = cbDesert5_Vino.Text;
            pbDeserts_Vino.Visible = true;
            pbDeserts_Vino.Image = Properties.Resources.cheesecake;

            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", dessertName);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{dessertName} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        //(Vino Santo Appetizers(starters))
        private void cbStarter1_Vino_CheckedChanged(object sender, EventArgs e)
        {
            String starter = cbStarter1_Vino.Text;
            pbAppetizers_Vino.Visible = true;
            pbAppetizers_Vino.Image = Properties.Resources.FoleGras;

            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", starter);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{starter} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbStarter2_Vino_CheckedChanged(object sender, EventArgs e)
        {
            String starter = cbStarter2_Vino.Text;
            pbAppetizers_Vino.Visible = true;
            pbAppetizers_Vino.Image = Properties.Resources.OysterRcfeller;

            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", starter);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{starter} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbStarter3_Vino_CheckedChanged(object sender, EventArgs e)
        {
            String starter = cbStarter3_Vino.Text;
            pbAppetizers_Vino.Visible = true;
            pbAppetizers_Vino.Image = Properties.Resources.tunatarare;

            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", starter);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{starter} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");

                    }
                }
            }
        }
        private void cbStarter4_Vino_CheckedChanged(object sender, EventArgs e)
        {
            String starter = cbStarter4_Vino.Text;
            pbAppetizers_Vino.Visible = true;
            pbAppetizers_Vino.Image = Properties.Resources.Escargot;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", starter);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{starter} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbStarter5_Vino_CheckedChanged(object sender, EventArgs e)
        {
            String starter = cbStarter5_Vino.Text;
            pbAppetizers_Vino.Visible = true;
            pbAppetizers_Vino.Image = Properties.Resources.BeefCarpaccio;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", starter);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{starter} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        //(Vino Santo Main Courses)
        private void cbMain1_Vino_CheckedChanged(object sender, EventArgs e)
        {
            String Main = cbMain1_Vino.Text;
            pbMain_Vino.Visible = true;
            pbMain_Vino.Image = Properties.Resources.BeefCarpaccio;

            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Main);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{Main} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbMain2_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Vino.Visible = true;
            pbMain_Vino.Image = Properties.Resources.DuckOrange;
            String Main = cbMain2_Vino.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Main);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{Main} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbMain3_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Vino.Visible = true;
            pbMain_Vino.Image = Properties.Resources.LobsterThermidor;
            String Main = cbMain3_Vino.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Main);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{Main} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbMain4_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Vino.Visible = true;
            pbMain_Vino.Image = Properties.Resources.LambRack;
            String Main = cbMain4_Vino.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Main);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{Main} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbMain5_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Vino.Visible = true;
            pbMain_Vino.Image = Properties.Resources.Risotto;
            String Main = cbMain5_Vino.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Main);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{Main} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbMain6_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Vino.Visible = true;
            pbMain_Vino.Image = Properties.Resources.SalmonCroute;
            String Main = cbMain6_Vino.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Main);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{Main} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbMain7_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Vino.Visible = true;
            pbMain_Vino.Image = Properties.Resources.coqVin;
            String Main = cbMain7_Vino.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Main);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{Main} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbMain8_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Vino.Visible = true;
            pbMain_Vino.Image = Properties.Resources.BeefWelling;
            String Main = cbMain8_Vino.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Main);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{Main} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        //===============================================================================================
        //11.(Button that removes menu items from the order list)
        private void btnRemove_Vino_Click(object sender, EventArgs e)
        {
            if (lbOrder_Vino.SelectedItems.Count > 0)
            {
                var itemRemove = new List<object>();
                decimal totalDeduction = 0m; // Variable to keep track of the total amount to deduct
                foreach (var item in lbOrder_Vino.SelectedItems)
                {
                    itemRemove.Add(item);

                    // Extract price from the selected item to deduct from the total
                    string listItem = item.ToString();
                    int dashIndex = listItem.IndexOf('-');
                    if (dashIndex != -1)
                    {
                        string priceString = listItem.Substring(dashIndex + 1).Trim().Replace("$", "");
                        if (decimal.TryParse(priceString, out decimal price))
                        {
                            totalDeduction += price; // Sum the prices to be deducted
                        }
                    }
                }
                foreach (var item in itemRemove)
                {
                    lbOrder_Vino.Items.Remove(item); // Remove items from the order list
                }
                // Deduct the total price based on removed items
                UpdateTotal(0, 0, -totalDeduction);

                // Update the summary to reflect the changes
                UpdateSummary();
            }
            else
            {
                MessageBox.Show("No item selected to remove.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        //12.(Card Number textBox to show type of card in the pictureBox)
        private void txtcrdNo_Vino_TextChanged(object sender, EventArgs e)
        {
            if (txtcrdNo_Vino.Text.Length > 0)
            {
                char firstDigit = txtcrdNo_Vino.Text[0];
                if (firstDigit == '4')
                {
                    pbVino_pay.Visible = true;
                    pbVino_pay.Image = Properties.Resources.visa;
                }
                else if (firstDigit == '5')
                {
                    pbVino_pay.Visible = true;
                    pbVino_pay.Image = Properties.Resources.mastercard;
                }
                else
                {
                    pbVino_pay.Visible = false;
                    pbVino_pay.Image = null;
                }
            }
            else
            {
                pbVino_pay.Visible = false;
                pbVino_pay.Image = null;
            }
        }
        //13.(CheckBox that allows to specify Allergies)
        private void cbAllergies_Vino_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAllergies_Vino.Checked == true)
            {
                rtxtAllergies_Vino.Show();
                label21.Show();
            }
            else
            {
                rtxtAllergies_Vino.Hide();
                label21.Hide();
            }
        }
        //14.(Vino Santo Customer review)
        private void btnReview_Vino_Click(object sender, EventArgs e)
        {
            lbReview_Vino.Items.Add(rtxtReview_Vino.Text);
        }
        //===============================================================================================
        //15.(Vino Santo Drink(s) pictureBox(s) & checkBox(s))

        //Cocktails
        private void cbcocktail1_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbCocktails_Vino.Visible = true;
            pbCocktails_Vino.Image = Properties.Resources.Martini;
            String cocktail = cbcocktail1_Vino.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", cocktail);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{cocktail} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                        UpdateSummary();
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbcocktail2_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbCocktails_Vino.Visible = true;
            pbCocktails_Vino.Image = Properties.Resources.Magarita;
            String cocktail = cbcocktail2_Vino.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", cocktail);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{cocktail} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                        UpdateSummary();
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbCocktail3_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbCocktails_Vino.Visible = true;
            pbCocktails_Vino.Image = Properties.Resources.OldFash;
            String cocktail = cbCocktail3_Vino.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", cocktail);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{cocktail} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                        UpdateSummary();
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbCocktail4_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbCocktails_Vino.Visible = true;
            pbCocktails_Vino.Image = Properties.Resources.Negori;
            String cocktail = cbCocktail4_Vino.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", cocktail);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{cocktail} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                        UpdateSummary();
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbCocktail5_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbCocktails_Vino.Visible = true;
            pbCocktails_Vino.Image = Properties.Resources.Mojito;
            String cocktail = cbCocktail5_Vino.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", cocktail);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{cocktail} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                        UpdateSummary();
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        //Wines
        private void cbWine1_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Vino.Visible = true;
            pbWines_Vino.Image = Properties.Resources.CarbSauv;
            String wines = cbWine1_Vino.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", wines);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{wines} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                        UpdateSummary();
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbWine2_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Vino.Visible = true;
            pbWines_Vino.Image = Properties.Resources.Chardonnay;
            String wines = cbWine2_Vino.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", wines);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{wines} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                        UpdateSummary();
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbWine3_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Vino.Visible = true;
            pbWines_Vino.Image = Properties.Resources.PinotNoir;
            String wines = cbWine3_Vino.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", wines);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{wines} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                        UpdateSummary();
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbWine4_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Vino.Visible = true;
            pbWines_Vino.Image = Properties.Resources.SauvBlanc;
            String wines = cbWine4_Vino.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", wines);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{wines} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                        UpdateSummary();
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbWine5_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Vino.Visible = true;
            pbWines_Vino.Image = Properties.Resources.Merlot;
            String wines = cbWine5_Vino.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", wines);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{wines} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                        UpdateSummary();
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        //Non-Alcoholic Beverages
        private void cbNonBev1_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Vino.Visible = true;
            pbNonBev_Vino.Image = Properties.Resources.SparklingWater;
            String NonBev = cbNonBev1_Vino.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", NonBev);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{NonBev} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                        UpdateSummary();
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbNonBev2_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Vino.Visible = true;
            pbNonBev_Vino.Image = Properties.Resources.OrangeJuice;
            String NonBev = cbNonBev2_Vino.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", NonBev);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{NonBev} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                        UpdateSummary();
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbNonBev3_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Vino.Visible = true;
            pbNonBev_Vino.Image = Properties.Resources.IcedTea;
            String NonBev = cbNonBev3_Vino.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", NonBev);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{NonBev} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                        UpdateSummary();
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbNonBev4_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Vino.Visible = true;
            pbNonBev_Vino.Image = Properties.Resources.MockTail;
            String NonBev = cbNonBev4_Vino.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", NonBev);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{NonBev} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                        UpdateSummary();
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbNonBev5_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Vino.Visible = true;
            pbNonBev_Vino.Image = Properties.Resources.MangoJuice;
            String NonBev = cbNonBev5_Vino.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", NonBev);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{NonBev} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                        UpdateSummary();
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        //Hot Beverages
        private void cbHotBev1_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbHotBev_Vino.Visible = true;
            pbHotBev_Vino.Image = Properties.Resources.Expresso;
            String HotBev = cbHotBev1_Vino.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", HotBev);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{HotBev} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                        UpdateSummary();
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbHotBev2_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbHotBev_Vino.Visible = true;
            pbHotBev_Vino.Image = Properties.Resources.Cuppacino;
            String HotBev = cbHotBev2_Vino.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", HotBev);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{HotBev} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                        UpdateSummary();
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbHotBev3_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbHotBev_Vino.Visible = true;
            pbHotBev_Vino.Image = Properties.Resources.Latte;
            String HotBev = cbHotBev3_Vino.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", HotBev);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{HotBev} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                        UpdateSummary();
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbHotBev4_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbHotBev_Vino.Visible = true;
            pbHotBev_Vino.Image = Properties.Resources.BlackTea;
            String HotBev = cbHotBev4_Vino.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", HotBev);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{HotBev} - ${price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Vino.Items.Add(displayText);
                        UpdateTotal(0, 0, price);
                        UpdateSummary();
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbHotBev5_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbHotBev_Vino.Visible = true;
            pbHotBev_Vino.Image = Properties.Resources.HerbalTea;
            String HotBev = cbHotBev5_Vino.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", HotBev);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{HotBev} - ${price:F2}";

                        // Add the product name and price to the list box

                        lbOrder_Vino.Items.Add(displayText);

                        UpdateSummary();
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbReserveType_Vino_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                using (cnn = new SqlConnection(conString))
                {
                    cnn.Open();
                    string query = $"Select number_of_people From Reservations Where restaurant_id = @restaurant_id and reservation_type = @reservation_type";
                    cmd = new SqlCommand(query, cnn);
                    cmd.Parameters.AddWithValue("@restaurant_id", restaurant_id);
                    cmd.Parameters.AddWithValue("@reservation_type", cbReserveType_Vino.SelectedItem.ToString());
                    object seatsAvailable = cmd.ExecuteScalar();

                    if (seatsAvailable != null)
                    {
                        int currentSeatsAvailable = (int)seatsAvailable;
                        lblNoOfSeats_Vino.Text = $"Seats Available: {currentSeatsAvailable}";
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void numericUpDown_Vino_ValueChanged(object sender, EventArgs e)
        {
            // Calculate the remaining seats by subtracting the entered seats from the total available seats
            int seatsEntered = (int)numericUpDown_Vino.Value;
            int seatsLeft = totalSeatsAvailable - seatsEntered;

            // Update the label dynamically
            if (seatsLeft >= 0)
            {
                lblNoOfSeats_Vino.Text = $"Seats Left: {seatsLeft}";
            }
            else
            {
                // Show an error message if the entered value exceeds available seats
                lblNoOfSeats_Vino.Text = "Seats Left: Not enough seats available";
            }
        }
        private void UpdateTotal(decimal reservationCost, decimal specialRequestCost, decimal menuItemCost)
        {
            totalPrice = reservationCost + specialRequestCost + menuItemCost;

            // Clear previous total price entry if it exists
            for (int i = lbOrder_Vino.Items.Count - 1; i >= 0; i--)
            {
                if (lbOrder_Vino.Items[i].ToString().StartsWith("Total Price:"))
                {
                    lbOrder_Vino.Items.RemoveAt(i);
                }
            }

            // Create the total price display text
            string totalDisplayText = $"Total Price: ${totalPrice:F2}";

            // Add total price at the end of the list
            lbOrder_Vino.Items.Add(totalDisplayText);

            // Update the summary
            UpdateSummary();
        }
        public struct ItemSummary
        {
            public int Count;
            public decimal TotalPrice;
        }
        private void UpdateSummary()
        {
            lbSummary_Vino.Items.Clear(); // Clear existing items in summary

            // Dictionary to hold item names and their summary data
            Dictionary<string, ItemSummary> itemCounts = new Dictionary<string, ItemSummary>();

            // Loop through items in lbOrder_Vino
            foreach (var item in lbOrder_Vino.Items)
            {
                string listItem = item.ToString();
                int dashIndex = listItem.IndexOf('-');

                if (dashIndex != -1)
                {
                    // Extract item name and price
                    string itemName = listItem.Substring(0, dashIndex).Trim();
                    string priceString = listItem.Substring(dashIndex + 1).Trim().Replace("$", "");

                    if (decimal.TryParse(priceString, out decimal price))
                    {
                        // Count occurrences and sum prices
                        if (itemCounts.ContainsKey(itemName))
                        {
                            var currentSummary = itemCounts[itemName];
                            currentSummary.Count++;
                            currentSummary.TotalPrice += price;
                            itemCounts[itemName] = currentSummary; // Update the entry
                        }
                        else
                        {
                            itemCounts[itemName] = new ItemSummary { Count = 1, TotalPrice = price }; // Initialize
                        }
                    }
                }
            }

            // Populate lbSummary_Vino with formatted items
            decimal totalSummaryPrice = 0;
            foreach (var kvp in itemCounts)
            {
                string displayText = $"{kvp.Key} ({kvp.Value.Count}) - ${kvp.Value.TotalPrice:F2}";
                lbSummary_Vino.Items.Add(displayText); // Format: Item (Count) - TotalPrice
                totalSummaryPrice += kvp.Value.TotalPrice;
            }

            // Retrieve the reservation cost and special request cost from the database
            decimal reservationCost = 0;
            decimal specialRequestCost = 0;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT rsvp_price FROM Reservations WHERE user_id = @user_id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@user_id", userId);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        reservationCost = Convert.ToDecimal(result);
                    }
                    conn.Close();
                }
            }
            // Add the reservation cost and special request cost
            string reservationCostDisplayText = $"Reservation Cost: ${reservationCost:F2}";
            lbSummary_Vino.Items.Add(reservationCostDisplayText);

            string specialRequestCostDisplayText = $"Special Request Cost: ${specialRequestCost:F2}";
            lbSummary_Vino.Items.Add(specialRequestCostDisplayText);

            // Add the total summary price
            decimal totalSummaryPriceWithCosts = totalSummaryPrice + reservationCost + specialRequestCost;
            string totalSummaryDisplayText = $"Total: ${totalSummaryPriceWithCosts:F2}";
            lbSummary_Vino.Items.Add(totalSummaryDisplayText);
        }

        private void lblSubTotal_Vino_Click(object sender, EventArgs e)
        {

        }
    }
}
