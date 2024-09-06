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
    public partial class Form7 : Form
    {
        public Form7()
        {
            InitializeComponent();
            pbDeserts_Asoka.SizeMode = PictureBoxSizeMode.StretchImage;
            pbAppetizers_Asoka.SizeMode = PictureBoxSizeMode.StretchImage;
            pbMain_Asoka.SizeMode = PictureBoxSizeMode.StretchImage;
            pbAsoka.SizeMode = PictureBoxSizeMode.StretchImage;
            pbCocktails_Asoka.SizeMode = PictureBoxSizeMode.StretchImage;
            pbHotBev_Asoka.SizeMode = PictureBoxSizeMode.StretchImage;
            pbWines_Asoka.SizeMode = PictureBoxSizeMode.StretchImage;
            pbNonBev_Asoka.SizeMode = PictureBoxSizeMode.StretchImage;

            label21.Hide();
            rtxtAllergies_Asoka.Hide();
            cbTime_Asoka.SelectedIndex = 0;
            cbReserveType_Asoka.SelectedIndex = 0;
        }

        public Form7(string name, string surname, string email, string phone_number)
        {
            InitializeComponent();
        }
        //==============================================================================
        //(Asoka Buttons that allow to movement from one page to another)

        //1.(Submit Reservation Details)
        private void btnSubmit_Asoka_Click(object sender, EventArgs e)
        {
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
            tabControl2.SelectedTab = tabPage6;
        }
        //7.(Button that submits orders from the menu item)
        private void btnSubmitOrder_Asoka_Click(object sender, EventArgs e)
        {
            if(lbOrder_Asoka.Items.Count > 0)
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
            lbOrder_Asoka.Items.Add(cbStarter1_Asoka.Text);
        }
        private void cbStarter2_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbAppetizers_Asoka.Visible = true;
            pbAppetizers_Asoka.Image = Properties.Resources.burrata;
            lbOrder_Asoka.Items.Add(cbStarter2_Asoka.Text);
        }
        private void cbStarter3_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbAppetizers_Asoka.Visible = true;
            pbAppetizers_Asoka.Image = Properties.Resources.SearedScallops;
            lbOrder_Asoka.Items.Add(cbStarter3_Asoka.Text);
        }
        private void cbStarter4_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbAppetizers_Asoka.Visible = true;
            pbAppetizers_Asoka.Image = Properties.Resources.Caviar;
            lbOrder_Asoka.Items.Add(cbStarter4_Asoka.Text);
        }
        private void cbStarter5_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbAppetizers_Asoka.Visible = true;
            pbAppetizers_Asoka.Image = Properties.Resources.StuffedMushrooms;
            lbOrder_Asoka.Items.Add(cbStarter5_Asoka.Text);
        }
        //(Asoka Deserts)
        private void cbDesert1_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbDeserts_Asoka.Visible = true;
            pbDeserts_Asoka.Image = Properties.Resources.LTart;
            lbOrder_Asoka.Items.Add(cbDesert1_Asoka.Text);
        }
        private void cbDesert2_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbDeserts_Asoka.Visible = true;
            pbDeserts_Asoka.Image = Properties.Resources.Profiteroles;
            lbOrder_Asoka.Items.Add(cbDesert1_Asoka.Text);
        }
        private void cbDesert3_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbDeserts_Asoka.Visible = true;
            pbDeserts_Asoka.Image = Properties.Resources.Pavlova;
            lbOrder_Asoka.Items.Add(cbDesert1_Asoka.Text);
        }
        private void cbDesert4_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbDeserts_Asoka.Visible = true;
            pbDeserts_Asoka.Image = Properties.Resources.BreadPudding;
            lbOrder_Asoka.Items.Add(cbDesert1_Asoka.Text);
        }
        private void cbDesert5_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbDeserts_Asoka.Visible = true;
            pbDeserts_Asoka.Image = Properties.Resources.cheesecake;
            lbOrder_Asoka.Items.Add(cbDesert1_Asoka.Text);
        }
        //(Asoka Main dishes)
        private void cbMain1_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Asoka.Visible = true;
            pbMain_Asoka.Image = Properties.Resources.SearedAhiTuna;
            lbOrder_Asoka.Items.Add(cbMain1_Asoka.Text);
        }
        private void cbMain2_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Asoka.Visible = true;
            pbMain_Asoka.Image = Properties.Resources.vegwellington;
            lbOrder_Asoka.Items.Add(cbMain1_Asoka.Text);
        }
        private void cbMain3_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Asoka.Visible = true;
            pbMain_Asoka.Image = Properties.Resources.bouillabaise;
            lbOrder_Asoka.Items.Add(cbMain2_Asoka.Text);
        }
        private void cbMain4_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Asoka.Visible = true;
            pbMain_Asoka.Image = Properties.Resources.OssoBuco;
            lbOrder_Asoka.Items.Add(cbMain3_Asoka.Text);
        }
        private void cbMain5_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Asoka.Visible = true;
            pbMain_Asoka.Image = Properties.Resources.confitDuck;
            lbOrder_Asoka.Items.Add(cbMain4_Asoka.Text);
        }
        private void cbMain6_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Asoka.Visible = true;
            pbMain_Asoka.Image = Properties.Resources.PorkTenderLoin;
            lbOrder_Asoka.Items.Add(cbMain6_Asoka.Text);
        }
        private void cbMain7_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Asoka.Visible = true;
            pbMain_Asoka.Image = Properties.Resources.SeaBass;
            lbOrder_Asoka.Items.Add(cbMain7_Asoka.Text);
        }
        private void cbMain8_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbMain_Asoka.Visible = true;
            pbMain_Asoka.Image = Properties.Resources.EggplantParmesan;
            lbOrder_Asoka.Items.Add(cbMain8_Asoka.Text);
        }
        //(Asoka cocktails)
        private void cbcocktail1_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbCocktails_Asoka.Visible = true;
            pbCocktails_Asoka.Image = Properties.Resources.GinTonic;
            lbOrder_Asoka.Items.Add(cbcocktail1_Asoka.Text);
        }
        private void cbcocktail2_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbCocktails_Asoka.Visible = true;
            pbCocktails_Asoka.Image = Properties.Resources.Cosmopolitan;
            lbOrder_Asoka.Items.Add(cbcocktail2_Asoka.Text);
        }
        private void cbcocktail3_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbCocktails_Asoka.Visible = true;
            pbCocktails_Asoka.Image = Properties.Resources.Negori;
            lbOrder_Asoka.Items.Add(cbcocktail3_Asoka.Text);
        }
        private void cbcocktail4_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbCocktails_Asoka.Visible = true;
            pbCocktails_Asoka.Image = Properties.Resources.Magarita;
            lbOrder_Asoka.Items.Add(cbcocktail4_Asoka.Text);
        }
        private void cbcocktail5_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbCocktails_Asoka.Visible = true;
            pbCocktails_Asoka.Image = Properties.Resources.PinaColada;
            lbOrder_Asoka.Items.Add(cbcocktail5_Asoka.Text);
        }
        //(Asoka Wines)
        private void cbWine1_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Asoka.Visible = true;
            pbWines_Asoka.Image = Properties.Resources.Syrah;
            lbOrder_Asoka.Items.Add(cbWine1_Asoka.Text);
        }
        private void cbWine2_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Asoka.Visible = true;
            pbWines_Asoka.Image = Properties.Resources.ZinfWine;
            lbOrder_Asoka.Items.Add(cbWine2_Asoka.Text);
        }
        private void cbWine3_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Asoka.Visible = true;
            pbWines_Asoka.Image = Properties.Resources.PinotNoir;
            lbOrder_Asoka.Items.Add(cbWine3_Asoka.Text);
        }
        private void cbWine4_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Asoka.Visible = true;
            pbWines_Asoka.Image = Properties.Resources.SauvBlanc;
            lbOrder_Asoka.Items.Add(cbWine4_Asoka.Text);
        }
        private void cbWine5_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbWines_Asoka.Visible = true;
            pbWines_Asoka.Image = Properties.Resources.Rose;
            lbOrder_Asoka.Items.Add(cbWine5_Asoka.Text);
        }
        //(Asoka Non-Alocholic beverages)
        private void cbNonBev1_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Asoka.Visible = true;
            pbNonBev_Asoka.Image = Properties.Resources.FlavouredWater;
            lbOrder_Asoka.Items.Add(cbNonBev1_Asoka.Text);
        }
        private void cbNonBev2_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Asoka.Visible = true;
            pbNonBev_Asoka.Image = Properties.Resources.SparklingWater;
            lbOrder_Asoka.Items.Add(cbNonBev2_Asoka.Text);
        }
        private void cbNonBev3_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Asoka.Visible = true;
            pbNonBev_Asoka.Image = Properties.Resources.OrangeJuice;
            lbOrder_Asoka.Items.Add(cbNonBev3_Asoka.Text);
        }
        private void cbNonBev4_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Asoka.Visible = true;
            pbNonBev_Asoka.Image = Properties.Resources.MangoJuice;
            lbOrder_Asoka.Items.Add(cbNonBev4_Asoka.Text);
        }
        private void cbNonBev5_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbNonBev_Asoka.Visible = true;
            pbNonBev_Asoka.Image = Properties.Resources.Lemonade;
            lbOrder_Asoka.Items.Add(cbNonBev5_Asoka.Text);
        }
        //(Asoka Hot beverages)
        private void cbHotBev1_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbHotBev_Asoka.Visible = true;
            pbHotBev_Asoka.Image = Properties.Resources.HotChoc;
            lbOrder_Asoka.Items.Add(cbHotBev1_Asoka.Text);
        }
        private void cbHotBev2_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbHotBev_Asoka.Visible = true;
            pbHotBev_Asoka.Image = Properties.Resources.GoldenMilk;
            lbOrder_Asoka.Items.Add(cbHotBev1_Asoka.Text);
        }
        private void cbHotBev3_Asoka_CheckedChanged(object sender, EventArgs e)
        {
            pbHotBev_Asoka.Visible = true;
            pbHotBev_Asoka.Image = Properties.Resources.MatchaLatte;
            lbOrder_Asoka.Items.Add(cbHotBev1_Asoka.Text);
        }
        private void cbHotBev4_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbHotBev_Asoka.Visible = true;
            pbHotBev_Asoka.Image = Properties.Resources.MulledWine;
            lbOrder_Asoka.Items.Add(cbHotBev1_Asoka.Text);
        }
        private void cbHotBev5_Pace_CheckedChanged(object sender, EventArgs e)
        {
            pbHotBev_Asoka.Visible = true;
            pbHotBev_Asoka.Image = Properties.Resources.HotToody;
            lbOrder_Asoka.Items.Add(cbHotBev1_Asoka.Text);
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
                var itemRemove = new List<object>();
                foreach (var item in lbOrder_Asoka.SelectedItems)
                {
                    itemRemove.Add(item);
                }
                foreach (var item in itemRemove)
                {
                    lbOrder_Asoka.Items.Remove(item);
                }
            }
            else
            {
                MessageBox.Show("No item selected to remove.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
