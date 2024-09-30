using Restaurant_Reservation_System_FinalProject_26.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace Restaurant_Reservation_System_FinalProject_26
{
    public partial class Form6 : Form
    {
        string conString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\restaurant_service.mdf;Integrated Security=True";
        SqlDataAdapter adapter;
        SqlConnection cnn;
        DataSet ds;
        SqlCommand cmd;
        decimal totalPrice = 0m;
        private decimal reservationPrice;
        private decimal rsvpPrice;
        private int seatsEntered;
        private string reservationType;
        private int restaurant_id;
        private int totalSeatsAvailable = 0;
        private int userId;
        private string email;

        private string userEmail;

        public Form6()
        {
            InitializeComponent();
        }
        public Form6(string name, string surname, string email, string phone_number)
        {
            InitializeComponent();

            pbDeserts_Pace.SizeMode = PictureBoxSizeMode.StretchImage;
            pbAppetizers_Pace.SizeMode = PictureBoxSizeMode.StretchImage;
            pbMain_Pace.SizeMode = PictureBoxSizeMode.StretchImage;
            pbPace.SizeMode = PictureBoxSizeMode.StretchImage;
            pbCocktails_Pace.SizeMode = PictureBoxSizeMode.StretchImage;
            pbHotBev_Pace.SizeMode = PictureBoxSizeMode.StretchImage;
            pbWines_Pace.SizeMode = PictureBoxSizeMode.StretchImage;
            pbNonBev_Pace.SizeMode = PictureBoxSizeMode.StretchImage;

            label21.Hide();
            rtxtAllergies_Pace.Hide();
            cbTime_Pace.SelectedIndex = 0;
            cbReserveType_Pace.SelectedIndex = 0;

            txtFName_Pace.Text = name;
            txtLName_Pace.Text = surname;
            txtEmail_Pace.Text = email;
            txtPhone_Pace.Text = phone_number;

            txtcrdholder_Pace.Text = name + " " + surname;

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
                                numericUpDown1_Pace.Value = 0;
                                // Reservation type and other details
                                cbReserveType_Pace.SelectedItem = reader["reservation_type"]?.ToString(); //reserve type
                                TimeSpan time = reader["reservation_time"] != DBNull.Value ? (TimeSpan)reader["reservation_time"] : TimeSpan.Zero;
                                string timeString = time.ToString(@"hh\:mm\:ss");
                                cbTime_Pace.SelectedItem = timeString;
                                string specialRequest = reader["special_requests"]?.ToString(); //special request
                                cbReserveType_Pace.SelectedItem = specialRequest;
                                // Bold the reservation date in the calendar
                                if (reader["reservation_date"] != DBNull.Value)
                                {
                                    DateTime date = (DateTime)reader["reservation_date"];
                                    cd_Pace.BoldedDates = new DateTime[] { date };
                                }
                                // Set the initial label showing total seats available
                                lblNoOfSeats_Pace.Text = $"Seats Left: {totalSeatsAvailable}";
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









        }
        //==============================================================================
        //(Pace Buttons that allow to movement from one page to another)

        //1.(Submit Reservation Details)
        private void btnSubmit_Pace_Click(object sender, EventArgs e)
        {


             reservationType = cbReserveType_Pace.SelectedItem.ToString();
                
            string query = "SELECT rsvp_price FROM Reservations WHERE reservation_type = @Reservation_type";
            string queryUserId = "SELECT user_id FROM Use_details WHERE email = @UserEmail";


            using (SqlConnection connection = new SqlConnection(conString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Reservation_type", reservationType);

                connection.Open();
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    reservationPrice = Convert.ToDecimal(result); // Save the price

                }
                else
                {
                    MessageBox.Show($"No price found for {reservationType}.");
                }
                connection.Close();

                connection.Open();
                SqlCommand commandUserId = new SqlCommand(queryUserId, connection);
                commandUserId.Parameters.AddWithValue("@UserEmail", txtEmail_Pace.Text); // Assuming there's a textbox for email.
                int user_id;
                object userIdResult = commandUserId.ExecuteScalar();
                if (userIdResult != null)
                {
                    user_id = Convert.ToInt32(userIdResult); // Save the user_id
                }
                else
                {
                    MessageBox.Show($"No user found with the email {txtEmail_Pace.Text}.");
                    return;
                }
                connection.Close();

                

                try
                {
                    // Get the date from DatePicker (already a DateTime object)
                    DateTime reservationDate = cd_Pace.SelectionStart;

                    TimeSpan reservationTimeSpan;
                    if (TimeSpan.TryParse(cbTime_Pace.Text, out reservationTimeSpan))
                    {
                        DateTime reservationDateTime = reservationDate.Date + reservationTimeSpan;
                        string sql = "INSERT INTO Reservations (user_id, restaurant_id, reservation_date, reservation_time, number_of_people, reservation_type, special_requests, rsvp_price) VALUES (@RSVP_UserID, RSVP_ResID, @RSVP_date, @RSVP_Time, @No_Of_Guests, @Event_Type, @Special_req, @RSVP_Price)";
                        using (SqlConnection cnn = new SqlConnection(conString))
                        {
                            using (SqlCommand cmd = new SqlCommand(sql, cnn))
                            {
                                decimal numeric = numericUpDown1_Pace.Value;

                                cmd.Parameters.AddWithValue("@RSVP_UserID", user_id);
                                cmd.Parameters.AddWithValue("@RSVP_ResID", 4);
                                cmd.Parameters.AddWithValue("@RSVP_date", reservationDate);
                                cmd.Parameters.AddWithValue("@RSVP_Time", reservationDateTime);
                                cmd.Parameters.AddWithValue("@No_Of_Guests", numeric);
                                cmd.Parameters.AddWithValue("@Event_Type", cbReserveType_Pace.Text);
                                cmd.Parameters.AddWithValue("@Special_req", cbRequest_Pace.Text);
                                cmd.Parameters.AddWithValue("@RSVP_Price", reservationPrice);
                                cnn.Open();
                                cmd.ExecuteNonQuery();
                                cnn.Close();
                                MessageBox.Show("Reservation Booked successfully!");
                            }
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

            
        }
        //2.(Payment of reservation and menu items)
        private void btnPay_Pace_Click(object sender, EventArgs e)
        {
            string name = txtcrdholder_Pace.Text;
            string cvv = txtCVV_Pace.Text;
            string cardNumber = txtcrdNo_Pace.Text;
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
                SaveSummaryToFile();
            }
            // Proceed if all validations are successful
            if (isValid)
            {
                MessageBox.Show("Transaction was successful!!!");
                tabControl1.SelectedTab = tabPage5;
            }
            using (SqlConnection cnn = new SqlConnection(conString))
            {
                try
                {
                    cnn.Open();
                    // Query to retrieve reservation details from the database using the email
                    string selectQuery = "SELECT reservation_date, reservation_time, number_of_people, reservation_type, special_requests, name FROM Reservations " +
                                         "INNER JOIN User_account ON Reservations.user_id = User_account.user_id " +
                                         "WHERE email = @userEmail"; // Use the correct parameter name

                    using (SqlCommand selectCmd = new SqlCommand(selectQuery, cnn))
                    {
                        selectCmd.Parameters.AddWithValue("@userEmail", userEmail); // Use the email stored from the login
                        using (SqlDataReader reader = selectCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Retrieve reservation details
                                name = reader["name"]?.ToString() ?? "Unknown";
                                int numPeople = reader["number_of_people"] != DBNull.Value ? Convert.ToInt32(reader["number_of_people"]) : 0;
                                totalSeatsAvailable = numPeople;

                                // Set other controls with reservation details
                                cbReserveType_Pace.SelectedItem = reader["reservation_type"]?.ToString();
                                TimeSpan time = reader["reservation_time"] != DBNull.Value ? (TimeSpan)reader["reservation_time"] : TimeSpan.Zero;
                                cbTime_Pace.SelectedItem = time.ToString(@"hh\:mm\:ss");

                                string specialRequest = reader["special_requests"]?.ToString() ?? "None";
                                cbReserveType_Pace.SelectedItem = specialRequest;

                                DateTime date = reader["reservation_date"] != DBNull.Value ? (DateTime)reader["reservation_date"] : DateTime.MinValue;
                                cd_Pace.BoldedDates = new DateTime[] { date };

                                lblNoOfSeats_Pace.Text = $"Seats Left: {totalSeatsAvailable}";

                                // Write summary information to a file
                                string filePath = "reservation_summary.txt";
                                using (StreamWriter writer = new StreamWriter(filePath, true)) // true to append to the file
                                {
                                    // Write customer and reservation details
                                    writer.WriteLine("=== Reservation Details ===");
                                    writer.WriteLine($"Customer Name: {name}");
                                    writer.WriteLine($"Reservation Date: {date:yyyy-MM-dd}");
                                    writer.WriteLine($"Reservation Time: {time:hh\\:mm\\:ss}");
                                    writer.WriteLine($"Number of People: {numPeople}");
                                    writer.WriteLine($"Reservation Type: {cbReserveType_Pace.SelectedItem?.ToString()}");
                                    writer.WriteLine($"Special Requests: {specialRequest}");
                                    writer.WriteLine();

                                    // Write the summary information from lbSummary_Pace
                                    writer.WriteLine("=== Order Summary ===");
                                    foreach (var item in lbSummary_Pace.Items)
                                    {
                                        writer.WriteLine(item.ToString());
                                    }
                                    writer.WriteLine("=====================");
                                    writer.WriteLine(); // Add a blank line for separation
                                }
                                MessageBox.Show("Payment processed and reservation details saved to file.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Switch to the payment tab
                                tabControl1.SelectedTab = tabPage5;
                            }
                            else
                            {
                                MessageBox.Show("No reservation found for the provided email.");
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
        }

        //3.(Back button)
        private void btnBackPace_pg3_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage3;
        }
        //4.(Button back to Home page)
        private void btnHomePace_Click(object sender, EventArgs e)
        {
            Form4 frm4 = new Form4();
            frm4.Show();
            this.Close();
        }
        //5.(Back button)
        private void btnBackPace_pg2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab= tabPage2;
        }
        //6.(Next button at Pace menu that takes you to the drinks session)
        private void btnNext_Pace_Click(object sender, EventArgs e)
        {
            // Clear any previous total price entry before calculating
            lbOrder_Pace.Items.Remove("Total Price:");
            decimal totalPrice = 0m;
            // Calculate total price
            foreach (var item in lbOrder_Pace.Items)
            {
                string listItem = item.ToString();
                int dashIndex = listItem.IndexOf('-');
                if (dashIndex != -1)
                {
                    string priceString = listItem.Substring(dashIndex + 1).Trim().Replace("R", "");
                    if (decimal.TryParse(priceString, out decimal price))
                    {
                        totalPrice += price; // Sum the prices
                    }
                }
            }
            tabControl2.SelectedTab = tabPage6;
        }
        //7.(Button that submits orders from the menu item)
        private void btnSubmitOrder_Pace_Click(object sender, EventArgs e)
        {
            if (lbOrder_Pace.Items.Count > 0)
            {
                MessageBox.Show("Successfully submitted order!!!");

                // Display subtotal and calculate total including tax
                lblSubTotal_Pace.Text = $"{totalPrice:F2}";
                decimal taxPerc = 0.15m; // Use decimal for accurate financial calculations
                decimal taxAmount = totalPrice * taxPerc; // Calculate the tax amount
                decimal totalAmount = totalPrice + taxAmount +(reservationPrice * seatsEntered) ; // Calculate the total amount including tax
                lblTotal_Pace.Text = $"{totalAmount:F2}";
                // Navigate to tabPage4 to show confirmation message
                tabControl1.SelectedTab = tabPage4;
            }
            else
            {
                // Display a message if no items were selected
                MessageBox.Show("No order item from menu!!!");
            }
            tabControl2.SelectedTab = tabPage6;
            UpdateSummary();
        }
        //8.(Button that skips the menu section)
        private void btnSkip_Pace_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage4;
        }
        //9.(Button that takes you back to the food section)
        private void btnBack1_Pace_Click(object sender, EventArgs e)
        {
            tabControl2.SelectedTab= tabPage1;
        }
        //==============================================================================

        //==============================================================================
        //10. (Menu Items for the Pace Restaurant PictureBox(s) & CheckBox(s))

        //(Pace CockTails)
        private void cbcocktail1_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbCocktails_Pace.Visible = true;
            pbCocktails_Pace.Image = Properties.Resources.Mojito;
            String Cock = cbcocktail1_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Cock);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{Cock} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbcocktail2_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbCocktails_Pace.Visible = true;
            pbCocktails_Pace.Image = Properties.Resources.Martini;
            String Cock = cbcocktail2_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Cock);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{Cock} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Cocktail not found.");
                    }
                }
            }
        }
        private void cbcocktail3_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbCocktails_Pace.Visible = true;
            pbCocktails_Pace.Image = Properties.Resources.manhattan;
            String Cock = cbcocktail3_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Cock);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{Cock} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbcocktail4_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbCocktails_Pace.Visible = true;
            pbCocktails_Pace.Image = Properties.Resources.PinaColada;
            String Cock = cbcocktail4_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Cock);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{Cock} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbcocktail5_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbCocktails_Pace.Visible = true;
            pbCocktails_Pace.Image = Properties.Resources.WhiskeySour;
            String Cock = cbcocktail5_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Cock);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{Cock} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        //(Pace Wines)
        private void cbWine1_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Pace.Visible = true;
            pbWines_Pace.Image = Properties.Resources.Rose;
            String Wine = cbWine1_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Wine);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{Wine} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbWine2_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Pace.Visible = true;
            pbWines_Pace.Image = Properties.Resources.Prosecco;
            String Wine = cbWine2_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Wine);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{Wine} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbWine3_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Pace.Visible = true;
            pbWines_Pace.Image = Properties.Resources.Riesling;
            String Wine = cbWine3_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Wine);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{Wine} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbWine4_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Pace.Visible = true;
            pbWines_Pace.Image = Properties.Resources.Merlot;
            String Wine = cbWine4_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Wine);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{Wine} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbWine5_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Pace.Visible = true;
            pbWines_Pace.Image = Properties.Resources.Chardonnay;
            String Wine = cbWine5_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Wine);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{Wine} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        //(Pace Non-Alcholic Beverages)
        private void cbNonBev1_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Pace.Visible = true;
            pbNonBev_Pace.Image = Properties.Resources.Lemonade;
            String Bev = cbNonBev1_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Bev);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{Bev} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbNonBev2_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Pace.Visible = true;
            pbNonBev_Pace.Image = Properties.Resources.MockTail;
            String Bev = cbNonBev2_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Bev);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{Bev} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
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
            pbNonBev_Pace.Visible = true;
            pbNonBev_Pace.Image = Properties.Resources.SparklingWater;
            String Bev = cbNonBev3_Vino.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Bev);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{Bev} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbNonBev4_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Pace.Visible = true;
            pbNonBev_Pace.Image = Properties.Resources.IcedTea;
            String Bev = cbNonBev4_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Bev);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{Bev} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbNonBev5_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Pace.Visible = true;
            pbNonBev_Pace.Image = Properties.Resources.AppleJuice;
            String Bev = cbNonBev5_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Bev);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{Bev} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        //(Pace Hot Beverages)
        private void cbHotBev1_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbHotBev_Pace.Visible = true;
            pbHotBev_Pace.Image = Properties.Resources.Americano;
            String Bev = cbHotBev1_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Bev);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{Bev} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbHotBev2_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbHotBev_Pace.Visible = true;
            pbHotBev_Pace.Image = Properties.Resources.Mocchiato;
            String Bev = cbHotBev2_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Bev);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{Bev} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbHotBev3_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbHotBev_Pace.Visible = true;
            pbHotBev_Pace.Image = Properties.Resources.OolongTea;
            String Bev = cbHotBev3_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Bev);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{Bev} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbHotBev4_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbHotBev_Pace.Visible = true;
            pbHotBev_Pace.Image = Properties.Resources.ChaiTeaLatte;
            String Bev = cbHotBev4_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Bev);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{Bev} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbHotBev5_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbHotBev_Pace.Visible = true;
            pbHotBev_Pace.Image = Properties.Resources.Expresso;
            String Bev = cbHotBev5_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Bev);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{Bev} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        //====================================================================
        //(Pace Starters)
        private void cbStarter1_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbAppetizers_Pace.Visible = true;
            pbAppetizers_Pace.Image = Properties.Resources.Arancin;
            String menu = cbStarter1_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", menu);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{menu} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbStarter2_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbAppetizers_Pace.Visible = true;
            pbAppetizers_Pace.Image = Properties.Resources.Cevinche;
            String menu = cbStarter2_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", menu);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{menu} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbStarter3_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbAppetizers_Pace.Visible = true;
            pbAppetizers_Pace.Image = Properties.Resources.ShrimpCocktail;
            String menu = cbStarter3_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", menu);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{menu} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbStarter4_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbAppetizers_Pace.Visible = true;
            pbAppetizers_Pace.Image = Properties.Resources.P_Melon;
            String menu = cbStarter4_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", menu);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{menu} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbStarter5_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbAppetizers_Pace.Visible = true;
            pbAppetizers_Pace.Image = Properties.Resources.CrabCakes;
            String menu = cbStarter5_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", menu);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{menu} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        //(Pace deserts)
        private void cbDesert1_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbDeserts_Pace.Visible = true;
            pbDeserts_Pace.Image = Properties.Resources.MoltenLCake;
            String menu = cbDesert1_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", menu);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{menu} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbDesert2_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbDeserts_Pace.Visible = true;
            pbDeserts_Pace.Image = Properties.Resources.MillieFeuille;
            String menu = cbDesert2_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", menu);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{menu} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbDesert3_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbDeserts_Pace.Visible = true;
            pbDeserts_Pace.Image = Properties.Resources.BanoffeePie;
            String menu = cbDesert3_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", menu);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{menu} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbDesert4_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbDeserts_Pace.Visible = true;
            pbDeserts_Pace.Image = Properties.Resources.operaCake;
            String menu = cbDesert4_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", menu);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{menu} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbDesert5_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbDeserts_Pace.Visible = true;
            pbDeserts_Pace.Image = Properties.Resources.AppleTT;
            String menu = cbDesert5_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", menu);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{menu} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        //(Pace Main dishes)
        private void cbMain1_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Pace.Visible = true;
            pbMain_Pace.Image = Properties.Resources.Paella;
            String menu = cbMain1_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", menu);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{menu} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbMain2_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Pace.Visible = true;
            pbMain_Pace.Image = Properties.Resources.VealMarsala;
            String menu = cbMain2_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", menu);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{menu} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbMain3_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Pace.Visible = true;
            pbMain_Pace.Image = Properties.Resources.StuffedChickBreast;
            String menu = cbMain3_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", menu);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{menu} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbMain4_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Pace.Visible = true;
            pbMain_Pace.Image = Properties.Resources.MoulesMarin;
            String menu = cbMain4_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", menu);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{menu} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbMain5_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Pace.Visible = true;
            pbMain_Pace.Image = Properties.Resources.SpaghettiCarbo;
            String menu = cbMain5_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", menu);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{menu} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbMain6_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Pace.Visible = true;
            pbMain_Pace.Image = Properties.Resources.vegMoussaka;
            String menu = cbMain6_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", menu);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{menu} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbMain7_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Pace.Visible = true;
            pbMain_Pace.Image = Properties.Resources.GrilledSwordFish;
            String menu = cbMain7_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", menu);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{menu} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbMain8_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Pace.Visible = true;
            pbMain_Pace.Image = Properties.Resources.Coussoulet;
            String menu = cbMain8_Pace.Text;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                string query = "SELECT price FROM MenuItems WHERE name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", menu);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        string displayText = $"{menu} - R{price:F2}";

                        // Add the product name and price to the list box
                        lbOrder_Pace.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        //12.(Card Number textBox to show type of card in the pictureBox)
        private void txtcrdNo_Pace_TextChanged(object sender, EventArgs e)
        {
            if (txtcrdNo_Pace.Text.Length > 0)
            {
                char firstDigit = txtcrdNo_Pace.Text[0];
                if (firstDigit == '4')
                {
                    pbPace.Visible = true;
                    pbPace.Image = Properties.Resources.visa;
                }
                else if (firstDigit == '5')
                {
                    pbPace.Visible = true;
                    pbPace.Image = Properties.Resources.mastercard;
                }
                else
                {
                    pbPace.Visible = false;
                    pbPace.Image = null;
                }
            }
            else
            {
                pbPace.Visible = false;
                pbPace.Image = null;
            }
        }
        //(CheckBox that allows to specify Allergies)
        private void cbAllergies_Pace_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAllergies_Pace.Checked == true)
            {
                rtxtAllergies_Pace.Show();
                label21.Show();
            }
            else
            {
                rtxtAllergies_Pace.Hide();
                label21.Hide();
            }
        }
        //(Pace Customer review)
        private void btnReview_Pace_Click(object sender, EventArgs e)
        {
            lbReview_Pace.Items.Add(rtxtReview_Pace.Text);
        }
        //(Button that removes menu items from the order list)
        private void btnRemove_Pace_Click(object sender, EventArgs e)
        {
                if (lbOrder_Pace.SelectedItems.Count > 0)
                {
                    List<object> itemsToRemove = new List<object>();
                    decimal totalDeduction = 0m; // Keep track of the total deduction amount

                    foreach (var item in lbOrder_Pace.SelectedItems)
                    {
                        itemsToRemove.Add(item);

                        // Extract price from the selected item
                        string listItem = item.ToString();
                        int dashIndex = listItem.IndexOf('-');
                        if (dashIndex != -1)
                        {
                            string priceString = listItem.Substring(dashIndex + 1).Trim().Replace("R", "");
                            if (decimal.TryParse(priceString, out decimal price))
                            {
                                totalDeduction += price; // Accumulate the amount to deduct
                            }
                        }
                    }

                    // Remove selected items
                    foreach (var item in itemsToRemove)
                    {
                        lbOrder_Pace.Items.Remove(item);
                    }

                    // Debugging output to check total deduction after removal
                    Console.WriteLine($"Total Deduction: {totalDeduction}");

                    // Recalculate the total price after item removal
                    UpdateTotal(-totalDeduction);
                }
                else
                {
                    MessageBox.Show("No item selected to remove.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
        }
        //===================================================================
        private void UpdateTotal(decimal price)
        {
            totalPrice += price; // Increment the total pricelblTotalPrice.Text = $"Total: ${totalPrice:F2}"; // Optional: display the running total on the UI

            // Clear previous total price entry if it exists
            for (int i = lbOrder_Pace.Items.Count - 1; i >= 0; i--)
            {
                if (lbOrder_Pace.Items[i].ToString().StartsWith("Total Price:"))
                {
                    lbOrder_Pace.Items.RemoveAt(i);
                }
            }
            // Create the total price display text
            string totalDisplayText = $"Total Price: R{totalPrice:F2}";

            // Add total price at the end of the list
            lbOrder_Pace.Items.Add(totalDisplayText);
        }
        public struct ItemSummary
        {
            public int Count;
            public decimal TotalPrice;
        }
        private void UpdateSummary()
        {
            lbSummary_Pace.Items.Clear(); // Clear existing items in summary
            // Dictionary to hold item names and their summary data
            Dictionary<string, ItemSummary> itemCounts = new Dictionary<string, ItemSummary>();
            // Loop through items in lbOrder_Vino
            foreach (var item in lbOrder_Pace.Items)
            {
                string listItem = item.ToString();
                int dashIndex = listItem.IndexOf('-');

                if (dashIndex != -1)
                {
                    // Extract item name and price
                    string itemName = listItem.Substring(0, dashIndex).Trim();
                    string priceString = listItem.Substring(dashIndex + 1).Trim().Replace("R", "");

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
            foreach (var kvp in itemCounts)
            {
                string displayText = $"{kvp.Key} ({kvp.Value.Count}) - R{kvp.Value.TotalPrice:F2}";
                lbSummary_Pace.Items.Add(displayText); // Format: Item (Count) - TotalPrice
            }
            string displayText1 = $"{reservationType}   Seats Booked: {seatsEntered}     R{reservationPrice * seatsEntered:F2}"; // Calculate the total amount including tax

            lbSummary_Pace.Items.Add(displayText1);
        }

        private void numericUpDown1_Pace_ValueChanged(object sender, EventArgs e)
        {
             seatsEntered = (int)numericUpDown1_Pace.Value;
            int maxAvailableSeats = totalSeatsAvailable; // Consider using a more descriptive variable name
            int seatsLeft = maxAvailableSeats - seatsEntered;

            
            // Update the label dynamically
            if (seatsLeft >= 0)
            {
                lblNoOfSeats_Pace.Text = $"Seats Left: {seatsLeft}";
            }
            else
            {
                // This condition should not occur due to the input validation above
                lblNoOfSeats_Pace.Text = "Seats Left: Not enough seats available";
            }
        }

        private void tabPage6_Click(object sender, EventArgs e)
        {

        }

        private void SaveSummaryToFile()//
        {
            string filePath = "summary.txt"; // Update with your actual file path

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var item in lbSummary_Pace.Items)
                {
                    writer.WriteLine(item.ToString());
                }
            }
        }

        private void BtnSeeHst_Click(object sender, EventArgs e)
        {
            if (!lbPaceHistory.Visible)
            {
                // Show the ListBox and load the history
                lbPaceHistory.Visible = true;
                lbPaceHistory.Items.Clear(); // Clear previous items

                // Define the path to the history file, using the email as the filename
                string historyFilePath = $"{email}_history.txt";

                if (File.Exists(historyFilePath))
                {
                    // Read the history from the file and add it to the ListBox
                    string[] historyLines = File.ReadAllLines(historyFilePath);
                    foreach (string line in historyLines)
                    {
                        lbPaceHistory.Items.Add(line);
                    }
                }
                else
                {
                    // If no history file is found, display a message
                    lbPaceHistory.Items.Add("No history available.");
                }
            }
            else
            {
                // Hide the ListBox if it is already visible
                lbPaceHistory.Visible = false;
            }
        }

        private void Form6_Load(object sender, EventArgs e)
        {

        }
    }
    
}
