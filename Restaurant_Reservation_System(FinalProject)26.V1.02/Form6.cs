using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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

        }
        //==============================================================================
        //(Pace Buttons that allow to movement from one page to another)

        //1.(Submit Reservation Details)
        private void btnSubmit_Pace_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage3;
        }
        //2.(Payment of reservation and menu items)
        private void btnPay_Pace_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage5;
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
                    string priceString = listItem.Substring(dashIndex + 1).Trim().Replace("$", "");
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
                lblSubTotal_Pace.Text = "${totalPrice:F2}";
                decimal taxPerc = 0.15m; // Use decimal for accurate financial calculations
                decimal taxAmount = totalPrice * taxPerc; // Calculate the tax amount
                decimal totalAmount = totalPrice + taxAmount; // Calculate the total amount including tax
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
                        string displayText = $"{Cock} - ${price:F2}";

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
                        string displayText = $"{Cock} - ${price:F2}";

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
                        string displayText = $"{Cock} - ${price:F2}";

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
                        string displayText = $"{Cock} - ${price:F2}";

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
                        string displayText = $"{Cock} - ${price:F2}";

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
                        string displayText = $"{Wine} - ${price:F2}";

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
                        string displayText = $"{Wine} - ${price:F2}";

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
                        string displayText = $"{Wine} - ${price:F2}";

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
                        string displayText = $"{Wine} - ${price:F2}";

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
                        string displayText = $"{Wine} - ${price:F2}";

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
                        string displayText = $"{Bev} - ${price:F2}";

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
                        string displayText = $"{Bev} - ${price:F2}";

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
                        string displayText = $"{Bev} - ${price:F2}";

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
                        string displayText = $"{Bev} - ${price:F2}";

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
                        string displayText = $"{Bev} - ${price:F2}";

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
                        string displayText = $"{Bev} - ${price:F2}";

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
                        string displayText = $"{Bev} - ${price:F2}";

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
                        string displayText = $"{Bev} - ${price:F2}";

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
                        string displayText = $"{Bev} - ${price:F2}";

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
                        string displayText = $"{Bev} - ${price:F2}";

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
                        string displayText = $"{menu} - ${price:F2}";

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
                        string displayText = $"{menu} - ${price:F2}";

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
                        string displayText = $"{menu} - ${price:F2}";

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
                        string displayText = $"{menu} - ${price:F2}";

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
                        string displayText = $"{menu} - ${price:F2}";

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
                        string displayText = $"{menu} - ${price:F2}";

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
                        string displayText = $"{menu} - ${price:F2}";

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
                        string displayText = $"{menu} - ${price:F2}";

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
                        string displayText = $"{menu} - ${price:F2}";

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
                        string displayText = $"{menu} - ${price:F2}";

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
                        string displayText = $"{menu} - ${price:F2}";

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
                        string displayText = $"{menu} - ${price:F2}";

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
                        string displayText = $"{menu} - ${price:F2}";

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
                        string displayText = $"{menu} - ${price:F2}";

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
                        string displayText = $"{menu} - ${price:F2}";

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
                        string displayText = $"{menu} - ${price:F2}";

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
                        string displayText = $"{menu} - ${price:F2}";

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
                        string displayText = $"{menu} - ${price:F2}";

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
                var itemRemove = new List<object>();
                foreach (var item in lbOrder_Pace.SelectedItems)
                {
                    itemRemove.Add(item);
                }
                foreach (var item in itemRemove)
                {
                    lbOrder_Pace.Items.Remove(item);
                }
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
            string totalDisplayText = $"Total Price: ${totalPrice:F2}";

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
            foreach (var kvp in itemCounts)
            {
                string displayText = $"{kvp.Key} ({kvp.Value.Count}) - ${kvp.Value.TotalPrice:F2}";
                lbSummary_Pace.Items.Add(displayText); // Format: Item (Count) - TotalPrice
            }
        }

    }
}
