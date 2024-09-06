using System;
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

namespace Restaurant_Reservation_System_FinalProject_26
{
    public partial class Form5 : Form
    {
        string conString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\restaurant_service.mdf;Integrated Security=True";
        SqlDataAdapter adapter;
        SqlConnection cnn;
        DataSet ds;
        SqlCommand cmd;

        public Form5()
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
        }

        public Form5(string name, string surname, string email, string phone_number)
        {
            InitializeComponent();

            txtFName_Vino.Text = name;
            LName_Vino.Text = surname;
            txtEmail_Vino.Text = email;
            txtPhone_Vino.Text = phone_number;
        }
        //==============================================================================
        //(Vino Santo Buttons that allow to movement from one page to another)

        //1.(Submit Reservation Details)
        private void btnSubmit_Vino_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage3;
        }
        //2.(Payment of reservation and menu items)
        private void btnPay_Vino_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Transaction was successful!!!");
            tabControl1.SelectedTab = tabPage5;
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
            tabControl1.SelectedTab= tabPage2;
        }
        //6.(Next button at Vino Santo menu that takes you to the drinks session)
        private void btnNext_Vino_Click(object sender, EventArgs e)
        {
            tabControl2.SelectedTab = tabPage6;
        }
        //7.(Button that submits orders from the menu item)
        private void btnSubmitOrder_Vino_Click(object sender, EventArgs e)
        {
            if(lbOrder_Vino.Items.Count > 0)
            {
                MessageBox.Show("Successfully submitted order!!!");
                tabControl1.SelectedTab = tabPage4;
            }
            else
            {
                MessageBox.Show("No order item from menu!!!");
            } 
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
            pbDeserts_Vino.Visible = true;
            pbDeserts_Vino.Image = Properties.Resources.CremeBrulee;
            lbOrder_Vino.Items.Add(cbDesert1_Vino.Text);
        }
        private void cbDesert2_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbDeserts_Vino.Visible = true;
            pbDeserts_Vino.Image = Properties.Resources.tiramisu;
            lbOrder_Vino.Items.Add(cbDesert2_Vino.Text);
        }
        private void cbDesert3_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbDeserts_Vino.Visible = true;
            pbDeserts_Vino.Image = Properties.Resources.Choc_Fondant;
            lbOrder_Vino.Items.Add(cbDesert3_Vino.Text);
        }
        private void cbDesert4_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbDeserts_Vino.Visible = true;
            pbDeserts_Vino.Image = Properties.Resources.PannaCotta;
            lbOrder_Vino.Items.Add(cbDesert4_Vino.Text);
        }
        private void cbDesert5_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbDeserts_Vino.Visible = true;
            pbDeserts_Vino.Image = Properties.Resources.cheesecake;
            lbOrder_Vino.Items.Add(cbDesert5_Vino.Text);
        }
        //(Vino Santo Appetizers(starters))
        private void cbStarter1_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbAppetizers_Vino.Visible = true;
            pbAppetizers_Vino.Image = Properties.Resources.FoleGras;
            lbOrder_Vino.Items.Add(cbStarter1_Vino.Text);
        }
        private void cbStarter2_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbAppetizers_Vino.Visible = true;
            pbAppetizers_Vino.Image = Properties.Resources.OysterRcfeller;
            lbOrder_Vino.Items.Add(cbStarter2_Vino.Text);
        }
        private void cbStarter3_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbAppetizers_Vino.Visible = true;
            pbAppetizers_Vino.Image = Properties.Resources.tunatarare;
            lbOrder_Vino.Items.Add(cbStarter3_Vino.Text);
        }
        private void cbStarter4_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbAppetizers_Vino.Visible = true;
            pbAppetizers_Vino.Image = Properties.Resources.Escargot;
            lbOrder_Vino.Items.Add(cbStarter4_Vino.Text);
        }
        private void cbStarter5_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbAppetizers_Vino.Visible = true;
            pbAppetizers_Vino.Image = Properties.Resources.BeefCarpaccio;
            lbOrder_Vino.Items.Add(cbStarter5_Vino.Text);
        }
        //(Vino Santo Main Courses)
        private void cbMain1_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Vino.Visible = true;
            pbMain_Vino.Image = Properties.Resources.BeefCarpaccio;
            lbOrder_Vino.Items.Add(cbMain1_Vino.Text);
        }
        private void cbMain2_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Vino.Visible = true;
            pbMain_Vino.Image = Properties.Resources.DuckOrange;
            lbOrder_Vino.Items.Add(cbMain2_Vino.Text);
        }
        private void cbMain3_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Vino.Visible = true;
            pbMain_Vino.Image = Properties.Resources.LobsterThermidor;
            lbOrder_Vino.Items.Add(cbMain3_Vino.Text);
        }
        private void cbMain4_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Vino.Visible = true;
            pbMain_Vino.Image = Properties.Resources.LambRack;
            lbOrder_Vino.Items.Add(cbMain4_Vino.Text);
        }
        private void cbMain5_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Vino.Visible = true;
            pbMain_Vino.Image = Properties.Resources.Risotto;
            lbOrder_Vino.Items.Add(cbMain5_Vino.Text);
        }
        private void cbMain6_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Vino.Visible = true;
            pbMain_Vino.Image = Properties.Resources.SalmonCroute;
            lbOrder_Vino.Items.Add(cbMain6_Vino.Text);
        }
        private void cbMain7_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Vino.Visible = true;
            pbMain_Vino.Image = Properties.Resources.coqVin;
            lbOrder_Vino.Items.Add(cbMain7_Vino.Text);
        }
        private void cbMain8_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Vino.Visible = true;
            pbMain_Vino.Image = Properties.Resources.BeefWelling;
            lbOrder_Vino.Items.Add(cbMain8_Vino.Text);
        }
        //===============================================================================================
        //11.(Button that removes menu items from the order list)
        private void btnRemove_Vino_Click(object sender, EventArgs e)
        {
            if(lbOrder_Vino.SelectedItems.Count > 0)
            {
                var itemRemove = new List<object>();
                foreach (var item in lbOrder_Vino.SelectedItems)
                {
                    itemRemove.Add(item);
                }
                foreach (var item in itemRemove)
                {
                    lbOrder_Vino.Items.Remove(item);
                }
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
            lbOrder_Vino.Items.Add(cbcocktail1_Vino.Text);
        }
        private void cbcocktail2_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbCocktails_Vino.Visible = true;
            pbCocktails_Vino.Image = Properties.Resources.Magarita;
            lbOrder_Vino.Items.Add(cbcocktail2_Vino.Text);
        }
        private void cbCocktail3_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbCocktails_Vino.Visible = true;
            pbCocktails_Vino.Image = Properties.Resources.OldFash;
            lbOrder_Vino.Items.Add(cbCocktail3_Vino.Text);
        }
        private void cbCocktail4_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbCocktails_Vino.Visible = true;
            pbCocktails_Vino.Image = Properties.Resources.Negori;
            lbOrder_Vino.Items.Add(cbCocktail4_Vino.Text);
        }
        private void cbCocktail5_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbCocktails_Vino.Visible = true;
            pbCocktails_Vino.Image = Properties.Resources.Mojito;
            lbOrder_Vino.Items.Add(cbCocktail5_Vino.Text);
        }
        //Wines
        private void cbWine1_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Vino.Visible = true;
            pbWines_Vino.Image = Properties.Resources.CarbSauv;
            lbOrder_Vino.Items.Add(cbWine1_Vino.Text);
        }
        private void cbWine2_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Vino.Visible = true;
            pbWines_Vino.Image = Properties.Resources.Chardonnay;
            lbOrder_Vino.Items.Add(cbWine2_Vino.Text);
        }
        private void cbWine3_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Vino.Visible = true;
            pbWines_Vino.Image = Properties.Resources.PinotNoir;
            lbOrder_Vino.Items.Add(cbWine3_Vino.Text);
        }
        private void cbWine4_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Vino.Visible = true;
            pbWines_Vino.Image = Properties.Resources.SauvBlanc;
            lbOrder_Vino.Items.Add(cbWine4_Vino.Text);
        }
        private void cbWine5_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Vino.Visible = true;
            pbWines_Vino.Image = Properties.Resources.Merlot;
            lbOrder_Vino.Items.Add(cbWine5_Vino.Text);
        }
        //Non-Alcoholic Beverages
        private void cbNonBev1_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Vino.Visible = true;
            pbNonBev_Vino.Image = Properties.Resources.SparklingWater;
            lbOrder_Vino.Items.Add(cbNonBev1_Vino.Text);
        }
        private void cbNonBev2_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Vino.Visible = true;
            pbNonBev_Vino.Image = Properties.Resources.OrangeJuice;
            lbOrder_Vino.Items.Add(cbNonBev2_Vino.Text);
        }
        private void cbNonBev3_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Vino.Visible = true;
            pbNonBev_Vino.Image = Properties.Resources.IcedTea;
            lbOrder_Vino.Items.Add(cbNonBev3_Vino.Text);
        }
        private void cbNonBev4_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Vino.Visible = true;
            pbNonBev_Vino.Image = Properties.Resources.MockTail;
            lbOrder_Vino.Items.Add(cbNonBev4_Vino.Text);
        }
        private void cbNonBev5_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Vino.Visible = true;
            pbNonBev_Vino.Image = Properties.Resources.MangoJuice;
            lbOrder_Vino.Items.Add(cbNonBev5_Vino.Text);
        }
        //Hot Beverages
        private void cbHotBev1_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbHotBev_Vino.Visible = true;
            pbHotBev_Vino.Image = Properties.Resources.Expresso;
            lbOrder_Vino.Items.Add(cbHotBev1_Vino.Text);
        }
        private void cbHotBev2_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbHotBev_Vino.Visible = true;
            pbHotBev_Vino.Image = Properties.Resources.Cuppacino;
            lbOrder_Vino.Items.Add(cbHotBev2_Vino.Text);
        }
        private void cbHotBev3_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbHotBev_Vino.Visible = true;
            pbHotBev_Vino.Image = Properties.Resources.Latte;
            lbOrder_Vino.Items.Add(cbHotBev3_Vino.Text);
        }
        private void cbHotBev4_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbHotBev_Vino.Visible = true;
            pbHotBev_Vino.Image = Properties.Resources.BlackTea;
            lbOrder_Vino.Items.Add(cbHotBev4_Vino.Text);
        }
        private void cbHotBev5_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbHotBev_Vino.Visible = true;
            pbHotBev_Vino.Image = Properties.Resources.HerbalTea;
            lbOrder_Vino.Items.Add(cbHotBev5_Vino.Text);
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            
        }
    }  
}
