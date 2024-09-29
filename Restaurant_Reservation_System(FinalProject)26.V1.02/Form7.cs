using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Restaurant_Reservation_System_FinalProject_26
{
    public partial class Form7 : Form
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
        public Form7()
        {
            InitializeComponent();     
        }

        public Form7(string name, string surname, string email, string phone_number)
        {
            InitializeComponent();
            label21.Hide();
            rtxtAllergies_Asoka.Hide();
            cbTime_Asoka.SelectedIndex = 0;
            cbReserveType_Asoka.SelectedIndex = 0;
            txtFName_Asoka.Text = name;
            txtLName_Asoka.Text = surname;
            txtEmail_Asoka.Text = email;
            txtPhone_Asoka.Text = phone_number;

            pbDeserts_Asoka.SizeMode = PictureBoxSizeMode.StretchImage;
            pbAppetizers_Asoka.SizeMode = PictureBoxSizeMode.StretchImage;
            pbMain_Asoka.SizeMode = PictureBoxSizeMode.StretchImage;
            pbAsoka.SizeMode = PictureBoxSizeMode.StretchImage;
            pbCocktails_Asoka.SizeMode = PictureBoxSizeMode.StretchImage;
            pbHotBev_Asoka.SizeMode = PictureBoxSizeMode.StretchImage;
            pbWines_Asoka.SizeMode = PictureBoxSizeMode.StretchImage;
            pbNonBev_Asoka.SizeMode = PictureBoxSizeMode.StretchImage;



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
                                numericUpDown1_Asoka.Value = 0;
                                // Reservation type and other details
                                cbReserveType_Asoka.SelectedItem = reader["reservation_type"]?.ToString(); //reserve type
                                TimeSpan time = reader["reservation_time"] != DBNull.Value ? (TimeSpan)reader["reservation_time"] : TimeSpan.Zero;
                                string timeString = time.ToString(@"hh\:mm\:ss");
                                cbTime_Asoka.SelectedItem = timeString;
                                string specialRequest = reader["special_requests"]?.ToString(); //special request
                                cbRequest_Asoka.SelectedItem = specialRequest;
                                // Bold the reservation date in the calendar
                                if (reader["reservation_date"] != DBNull.Value)
                                {
                                    DateTime date = (DateTime)reader["reservation_date"];
                                    cd_Asoka.BoldedDates = new DateTime[] { date };
                                }
                                // Set the initial label showing total seats available
                                lblNoOfSeats_Asoka.Text = $"Seats Left: {totalSeatsAvailable}";
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
        //(Asoka Buttons that allow to movement from one page to another)

        //1.(Submit Reservation Details)
        private void btnSubmit_Asoka_Click(object sender, EventArgs e)
        {
            reservationType = cbReserveType_Asoka.SelectedItem.ToString();

            string query = "SELECT rsvp_price FROM Reservations WHERE reservation_type = @Reservation_type";

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

            }
            tabControl1.SelectedTab = tabPage3;
        }
        //2.(Payment of reservation and menu items)
        private void btnPay_Asoka_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage5;
        }
        //3.(Back button)
        private void btnBackAsoka_pg3_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage3;
        }
        //4.(Button back to Home page)
        private void btnHomeAsoka_Click(object sender, EventArgs e)
        {
            Form4 frm4 = new Form4();
            frm4.Show();
            this.Close();
        }
        //5.(Back button)
        private void btnBackAsoka_pg2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab= tabPage2;
        }
        //6.(Next button at Pace menu that takes you to the drinks session)
        private void btnNext_Asoka_Click(object sender, EventArgs e)
        {
            // Clear any previous total price entry before calculating
            lbOrder_Asoka.Items.Remove("Total Price:");
            decimal totalPrice = 0m;
            // Calculate total price
            foreach (var item in lbOrder_Asoka.Items)
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
        private void btnSubmitOrder_Asoka_Click(object sender, EventArgs e)
        {
            if (lbOrder_Asoka.Items.Count > 0)
            {
                MessageBox.Show("Successfully submitted order!!!");

                // Display subtotal and calculate total including tax
                lblSubTotal_Asoka.Text = $"{totalPrice:F2}";
                decimal taxPerc = 0.15m; // Use decimal for accurate financial calculations
                decimal taxAmount = totalPrice * taxPerc; // Calculate the tax amount
                decimal totalAmount = totalPrice + taxAmount + (reservationPrice * seatsEntered); // Calculate the total amount including tax
                lblTotal_Asoka.Text = $"{totalAmount:F2}";
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
        private void btnSkip_Asoka_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage4;
        }
        //9.(Button that takes you back to the food section)
        private void btnBack1_Asoka_Click(object sender, EventArgs e)
        {
            tabControl2.SelectedTab = tabPage1;
        }
        private void Form7_Load(object sender, EventArgs e)
        {

        }
        //======================================================================
        //(Asoka starters)
        private void cbStarter1_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbAppetizers_Asoka.Visible = true;
            pbAppetizers_Asoka.Image = Properties.Resources.LobsterBisque;
            String menu = cbStarter1_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Item not found.");
                    }
                }
            }
        }
        private void cbStarter2_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbAppetizers_Asoka.Visible = true;
            pbAppetizers_Asoka.Image = Properties.Resources.burrata;
            String menu = cbStarter2_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                         UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbStarter3_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbAppetizers_Asoka.Visible = true;
            pbAppetizers_Asoka.Image = Properties.Resources.SearedScallops;
            String menu = cbStarter3_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                         UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbStarter4_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbAppetizers_Asoka.Visible = true;
            pbAppetizers_Asoka.Image = Properties.Resources.Caviar;
            String menu = cbStarter4_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                         UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbStarter5_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbAppetizers_Asoka.Visible = true;
            pbAppetizers_Asoka.Image = Properties.Resources.StuffedMushrooms;
            String menu = cbStarter5_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        //(Asoka Deserts)
        private void cbDesert1_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbDeserts_Asoka.Visible = true;
            pbDeserts_Asoka.Image = Properties.Resources.LTart;
            String menu = cbDesert1_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                         UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbDesert2_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbDeserts_Asoka.Visible = true;
            pbDeserts_Asoka.Image = Properties.Resources.Profiteroles;
            String menu = cbDesert2_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                         UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbDesert3_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbDeserts_Asoka.Visible = true;
            pbDeserts_Asoka.Image = Properties.Resources.Pavlova;
            String menu = cbDesert3_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbDesert4_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbDeserts_Asoka.Visible = true;
            pbDeserts_Asoka.Image = Properties.Resources.BreadPudding;
            String menu = cbDesert4_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                         UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbDesert5_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbDeserts_Asoka.Visible = true;
            pbDeserts_Asoka.Image = Properties.Resources.cheesecake;
            String menu = cbDesert5_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        //(Asoka Main dishes)
        private void cbMain1_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Asoka.Visible = true;
            pbMain_Asoka.Image = Properties.Resources.SearedAhiTuna;
            String menu = cbMain1_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbMain2_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Asoka.Visible = true;
            pbMain_Asoka.Image = Properties.Resources.vegwellington;
            String menu = cbMain2_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                        // UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbMain3_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Asoka.Visible = true;
            pbMain_Asoka.Image = Properties.Resources.bouillabaise;
            String menu = cbMain3_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                         UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbMain4_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Asoka.Visible = true;
            pbMain_Asoka.Image = Properties.Resources.OssoBuco;
            String menu = cbMain4_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbMain5_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Asoka.Visible = true;
            pbMain_Asoka.Image = Properties.Resources.confitDuck;
            String menu = cbMain5_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbMain6_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Asoka.Visible = true;
            pbMain_Asoka.Image = Properties.Resources.PorkTenderLoin;
            String menu = cbMain6_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                        // UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbMain7_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Asoka.Visible = true;
            pbMain_Asoka.Image = Properties.Resources.SeaBass;
            String menu = cbMain7_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbMain8_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Asoka.Visible = true;
            pbMain_Asoka.Image = Properties.Resources.EggplantParmesan;
            String menu = cbMain8_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        //(Asoka cocktails)
        private void cbcocktail1_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbCocktails_Asoka.Visible = true;
            pbCocktails_Asoka.Image = Properties.Resources.GinTonic;
            String menu = cbcocktail1_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbcocktail2_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbCocktails_Asoka.Visible = true;
            pbCocktails_Asoka.Image = Properties.Resources.Cosmopolitan;
            String menu = cbcocktail2_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                         UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbcocktail3_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbCocktails_Asoka.Visible = true;
            pbCocktails_Asoka.Image = Properties.Resources.Negori;
            String menu = cbcocktail3_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                         UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbcocktail4_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbCocktails_Asoka.Visible = true;
            pbCocktails_Asoka.Image = Properties.Resources.Magarita;
            String menu = cbcocktail4_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbcocktail5_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbCocktails_Asoka.Visible = true;
            pbCocktails_Asoka.Image = Properties.Resources.PinaColada;
            String menu = cbcocktail5_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        //(Asoka Wines)
        private void cbWine1_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Asoka.Visible = true;
            pbWines_Asoka.Image = Properties.Resources.Syrah;
            String menu = cbWine1_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbWine2_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Asoka.Visible = true;
            pbWines_Asoka.Image = Properties.Resources.ZinfWine;
            String menu = cbWine2_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbWine3_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Asoka.Visible = true;
            pbWines_Asoka.Image = Properties.Resources.PinotNoir;
            String menu = cbWine3_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbWine4_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Asoka.Visible = true;
            pbWines_Asoka.Image = Properties.Resources.SauvBlanc;
            String menu = cbWine4_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbWine5_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Asoka.Visible = true;
            pbWines_Asoka.Image = Properties.Resources.Rose;
            String menu = cbWine5_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        //(Asoka Non-Alocholic beverages)
        private void cbNonBev1_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Asoka.Visible = true;
            pbNonBev_Asoka.Image = Properties.Resources.FlavouredWater;
            String menu = cbNonBev1_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbNonBev2_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Asoka.Visible = true;
            pbNonBev_Asoka.Image = Properties.Resources.SparklingWater;
            String menu = cbNonBev2_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbNonBev3_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Asoka.Visible = true;
            pbNonBev_Asoka.Image = Properties.Resources.OrangeJuice;
            String menu = cbNonBev3_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbNonBev4_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Asoka.Visible = true;
            pbNonBev_Asoka.Image = Properties.Resources.MangoJuice;
            String menu = cbNonBev4_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbNonBev5_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Asoka.Visible = true;
            pbNonBev_Asoka.Image = Properties.Resources.Lemonade;
            String menu = cbNonBev5_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        //(Asoka Hot beverages)
        private void cbHotBev1_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbHotBev_Asoka.Visible = true;
            pbHotBev_Asoka.Image = Properties.Resources.HotChoc;
            String menu = cbHotBev1_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbHotBev2_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbHotBev_Asoka.Visible = true;
            pbHotBev_Asoka.Image = Properties.Resources.GoldenMilk;
            String menu = cbHotBev2_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                         UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        private void cbHotBev3_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbHotBev_Asoka.Visible = true;
            pbHotBev_Asoka.Image = Properties.Resources.MatchaLatte;
            String menu = cbHotBev3_Asoka.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
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
            pbHotBev_Asoka.Visible = true;
            pbHotBev_Asoka.Image = Properties.Resources.MulledWine;
            String menu = cbHotBev4_Pace.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
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
            pbHotBev_Asoka.Visible = true;
            pbHotBev_Asoka.Image = Properties.Resources.HotToody;
            String menu = cbHotBev5_Pace.Text;
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
                        lbOrder_Asoka.Items.Add(displayText);
                        UpdateTotal(price);
                    }
                    else
                    {
                        MessageBox.Show("Dessert not found.");
                    }
                }
            }
        }
        //(Card Number textBox to show type of card in the pictureBox)
        private void txtcrdNo_Asoka_TextChanged(object sender, EventArgs e)
        {
            if (txtcrdNo_Asoka.Text.Length > 0)
            {
                char firstDigit = txtcrdNo_Asoka.Text[0];
                if (firstDigit == '4')
                {
                    pbAsoka.Visible = true;
                    pbAsoka.Image = Properties.Resources.visa;
                }
                else if (firstDigit == '5')
                {
                    pbAsoka.Visible = true;
                    pbAsoka.Image = Properties.Resources.mastercard;
                }
                else
                {
                    pbAsoka.Visible = false;
                    pbAsoka.Image = null;
                }
            }
            else
            {
                pbAsoka.Visible = false;
                pbAsoka.Image = null;
            }
        }
        //(CheckBox that allows to specify Allergies)
        private void cbAllergies_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAllergies_Asoka.Checked == true)
            {
                rtxtAllergies_Asoka.Show();
                label21.Show();
            }
            else
            {
                rtxtAllergies_Asoka.Hide();
                label21.Hide();
            }
        }
        //(Button that removes menu items from the order list)
        private void btnRemove_Asoka_Click(object sender, EventArgs e)
        {
            if (lbOrder_Asoka.SelectedItems.Count > 0)
            {
                List<object> itemsToRemove = new List<object>();
                decimal totalDeduction = 0m; // Keep track of the total deduction amount

                foreach (var item in lbOrder_Asoka.SelectedItems)
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
                    lbOrder_Asoka.Items.Remove(item);
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

        private void UpdateTotal(decimal price)
        {
            totalPrice += price; // Increment the total pricelblTotalPrice.Text = $"Total: ${totalPrice:F2}"; // Optional: display the running total on the UI

            // Clear previous total price entry if it exists
            for (int i = lbOrder_Asoka.Items.Count - 1; i >= 0; i--)
            {
                if (lbOrder_Asoka.Items[i].ToString().StartsWith("Total Price:"))
                {
                    lbOrder_Asoka.Items.RemoveAt(i);
                }
            }
            // Create the total price display text
            string totalDisplayText = $"Total Price: R{totalPrice:F2}";

            // Add total price at the end of the list
            lbOrder_Asoka.Items.Add(totalDisplayText);
        }
        public struct ItemSummary
        {
            public int Count;
            public decimal TotalPrice;
        }
        private void UpdateSummary()
        {
            lbSummary_Asoka.Items.Clear(); // Clear existing items in summary
            // Dictionary to hold item names and their summary data
            Dictionary<string, ItemSummary> itemCounts = new Dictionary<string, ItemSummary>();
            // Loop through items in lbOrder_Vino
            foreach (var item in lbOrder_Asoka.Items)
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
                lbSummary_Asoka.Items.Add(displayText); // Format: Item (Count) - TotalPrice
            }
            string displayText1 = $"{reservationType}   Seats Booked: {seatsEntered}     R{reservationPrice * seatsEntered:F2}"; // Calculate the total amount including tax

            lbSummary_Asoka.Items.Add(displayText1);
        }

        private void numericUpDown1_Asoka_ValueChanged(object sender, EventArgs e)
        {
            seatsEntered = (int)numericUpDown1_Asoka.Value;
            int maxAvailableSeats = totalSeatsAvailable; // Consider using a more descriptive variable name
            int seatsLeft = maxAvailableSeats - seatsEntered;


            // Update the label dynamically
            if (seatsLeft >= 0)
            {
                lblNoOfSeats_Asoka.Text = $"Seats Left: {seatsLeft}";
            }
            else
            {
                // This condition should not occur due to the input validation above
                lblNoOfSeats_Asoka.Text = "Seats Left: Not enough seats available";
            }
        }
    }
}
