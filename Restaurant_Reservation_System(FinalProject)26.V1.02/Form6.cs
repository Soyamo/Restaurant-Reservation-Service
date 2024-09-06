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
        public Form6()
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
        }

        public Form6(string name, string surname, string email, string phone_number)
        {
            InitializeComponent();
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
            tabControl2.SelectedTab = tabPage6;
        }
        //7.(Button that submits orders from the menu item)
        private void btnSubmitOrder_Pace_Click(object sender, EventArgs e)
        {
            if (lbOrder_Pace.Items.Count > 0)
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
            lbOrder_Pace.Items.Add(cbcocktail1_Pace.Text);
        }
        private void cbcocktail2_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbCocktails_Pace.Visible = true;
            pbCocktails_Pace.Image = Properties.Resources.Martini;
            lbOrder_Pace.Items.Add(cbcocktail2_Pace.Text);
        }
        private void cbcocktail3_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbCocktails_Pace.Visible = true;
            pbCocktails_Pace.Image = Properties.Resources.manhattan;
            lbOrder_Pace.Items.Add(cbcocktail3_Pace.Text);
        }
        private void cbcocktail4_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbCocktails_Pace.Visible = true;
            pbCocktails_Pace.Image = Properties.Resources.PinaColada;
            lbOrder_Pace.Items.Add(cbcocktail4_Pace.Text);
        }
        private void cbcocktail5_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbCocktails_Pace.Visible = true;
            pbCocktails_Pace.Image = Properties.Resources.WhiskeySour;
            lbOrder_Pace.Items.Add(cbcocktail5_Pace.Text);
        }
        //(Pace Wines)
        private void cbWine1_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Pace.Visible = true;
            pbWines_Pace.Image = Properties.Resources.Rose;
            lbOrder_Pace.Items.Add(cbWine1_Pace.Text);
        }
        private void cbWine2_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Pace.Visible = true;
            pbWines_Pace.Image = Properties.Resources.Prosecco;
            lbOrder_Pace.Items.Add(cbWine2_Pace.Text);
        }
        private void cbWine3_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Pace.Visible = true;
            pbWines_Pace.Image = Properties.Resources.Riesling;
            lbOrder_Pace.Items.Add(cbWine3_Pace.Text);
        }
        private void cbWine4_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Pace.Visible = true;
            pbWines_Pace.Image = Properties.Resources.Merlot;
            lbOrder_Pace.Items.Add(cbWine2_Pace.Text);
        }
        private void cbWine5_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Pace.Visible = true;
            pbWines_Pace.Image = Properties.Resources.Chardonnay;
            lbOrder_Pace.Items.Add(cbWine2_Pace.Text);
        }
        //(Pace Non-Alcholic Beverages)
        private void cbNonBev1_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Pace.Visible = true;
            pbNonBev_Pace.Image = Properties.Resources.Lemonade;
            lbOrder_Pace.Items.Add(cbNonBev1_Pace.Text);
        }
        private void cbNonBev2_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Pace.Visible = true;
            pbNonBev_Pace.Image = Properties.Resources.MockTail;
            lbOrder_Pace.Items.Add(cbNonBev1_Pace.Text);
        }
        private void cbNonBev3_Vino_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Pace.Visible = true;
            pbNonBev_Pace.Image = Properties.Resources.SparklingWater;
            lbOrder_Pace.Items.Add(cbNonBev1_Pace.Text);
        }
        private void cbNonBev4_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Pace.Visible = true;
            pbNonBev_Pace.Image = Properties.Resources.IcedTea;
            lbOrder_Pace.Items.Add(cbNonBev1_Pace.Text);
        }
        private void cbNonBev5_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Pace.Visible = true;
            pbNonBev_Pace.Image = Properties.Resources.AppleJuice;
            lbOrder_Pace.Items.Add(cbNonBev1_Pace.Text);
        }
        //(Pace Hot Beverages)
        private void cbHotBev1_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbHotBev_Pace.Visible = true;
            pbHotBev_Pace.Image = Properties.Resources.Americano;
            lbOrder_Pace.Items.Add(cbHotBev1_Pace.Text);
        }
        private void cbHotBev2_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbHotBev_Pace.Visible = true;
            pbHotBev_Pace.Image = Properties.Resources.Mocchiato;
            lbOrder_Pace.Items.Add(cbHotBev2_Pace.Text);
        }
        private void cbHotBev3_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbHotBev_Pace.Visible = true;
            pbHotBev_Pace.Image = Properties.Resources.OolongTea;
            lbOrder_Pace.Items.Add(cbHotBev3_Pace.Text);
        }
        private void cbHotBev4_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbHotBev_Pace.Visible = true;
            pbHotBev_Pace.Image = Properties.Resources.ChaiTeaLatte;
            lbOrder_Pace.Items.Add(cbHotBev4_Pace.Text);
        }
        private void cbHotBev5_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbHotBev_Pace.Visible = true;
            pbHotBev_Pace.Image = Properties.Resources.Expresso;
            lbOrder_Pace.Items.Add(cbHotBev5_Pace.Text);
        }
        //====================================================================
        //(Pace Starters)
        private void cbStarter1_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbAppetizers_Pace.Visible = true;
            pbAppetizers_Pace.Image = Properties.Resources.Arancin;
            lbOrder_Pace.Items.Add(cbStarter1_Pace.Text);
        }
        private void cbStarter2_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbAppetizers_Pace.Visible = true;
            pbAppetizers_Pace.Image = Properties.Resources.Cevinche;
            lbOrder_Pace.Items.Add(cbStarter2_Pace.Text);
        }
        private void cbStarter3_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbAppetizers_Pace.Visible = true;
            pbAppetizers_Pace.Image = Properties.Resources.ShrimpCocktail;
            lbOrder_Pace.Items.Add(cbStarter3_Pace.Text);
        }
        private void cbStarter4_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbAppetizers_Pace.Visible = true;
            pbAppetizers_Pace.Image = Properties.Resources.P_Melon;
            lbOrder_Pace.Items.Add(cbStarter4_Pace.Text);
        }
        private void cbStarter5_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbAppetizers_Pace.Visible = true;
            pbAppetizers_Pace.Image = Properties.Resources.CrabCakes;
            lbOrder_Pace.Items.Add(cbStarter5_Pace.Text);
        }
        //(Pace deserts)
        private void cbDesert1_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbDeserts_Pace.Visible = true;
            pbDeserts_Pace.Image = Properties.Resources.MoltenLCake;
            lbOrder_Pace.Items.Add(cbDesert1_Pace.Text);
        }
        private void cbDesert2_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbDeserts_Pace.Visible = true;
            pbDeserts_Pace.Image = Properties.Resources.MillieFeuille;
            lbOrder_Pace.Items.Add(cbDesert2_Pace.Text);
        }
        private void cbDesert3_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbDeserts_Pace.Visible = true;
            pbDeserts_Pace.Image = Properties.Resources.BanoffeePie;
            lbOrder_Pace.Items.Add(cbDesert3_Pace.Text);
        }
        private void cbDesert4_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbDeserts_Pace.Visible = true;
            pbDeserts_Pace.Image = Properties.Resources.operaCake;
            lbOrder_Pace.Items.Add(cbDesert4_Pace.Text);
        }
        private void cbDesert5_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbDeserts_Pace.Visible = true;
            pbDeserts_Pace.Image = Properties.Resources.AppleTT;
            lbOrder_Pace.Items.Add(cbDesert5_Pace.Text);
        }
        //(Pace Main dishes)
        private void cbMain1_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Pace.Visible = true;
            pbMain_Pace.Image = Properties.Resources.Paella;
            lbOrder_Pace.Items.Add(cbMain1_Pace.Text);
        }
        private void cbMain2_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Pace.Visible = true;
            pbMain_Pace.Image = Properties.Resources.VealMarsala;
            lbOrder_Pace.Items.Add(cbMain2_Pace.Text);
        }
        private void cbMain3_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Pace.Visible = true;
            pbMain_Pace.Image = Properties.Resources.StuffedChickBreast;
            lbOrder_Pace.Items.Add(cbMain3_Pace.Text);
        }
        private void cbMain4_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Pace.Visible = true;
            pbMain_Pace.Image = Properties.Resources.MoulesMarin;
            lbOrder_Pace.Items.Add(cbMain4_Pace.Text);
        }
        private void cbMain5_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Pace.Visible = true;
            pbMain_Pace.Image = Properties.Resources.SpaghettiCarbo;
            lbOrder_Pace.Items.Add(cbMain5_Pace.Text);
        }
        private void cbMain6_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Pace.Visible = true;
            pbMain_Pace.Image = Properties.Resources.vegMoussaka;
            lbOrder_Pace.Items.Add(cbMain6_Pace.Text);
        }
        private void cbMain7_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Pace.Visible = true;
            pbMain_Pace.Image = Properties.Resources.GrilledSwordFish;
            lbOrder_Pace.Items.Add(cbMain7_Pace.Text);
        }
        private void cbMain8_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Pace.Visible = true;
            pbMain_Pace.Image = Properties.Resources.Coussoulet;
            lbOrder_Pace.Items.Add(cbMain8_Pace.Text);
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
    }
}
