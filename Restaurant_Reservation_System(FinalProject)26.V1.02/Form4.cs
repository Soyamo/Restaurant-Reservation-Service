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


namespace Restaurant_Reservation_System_FinalProject_26
{
    public partial class Form4 : Form
    {
        string conString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\restaurant_service.mdf;Integrated Security=True";
        SqlDataAdapter adapter;
        SqlConnection cnn;
        DataSet ds;
        SqlCommand cmd;

        public string UserName { get; set; }
        public string UserSurname { get; set; }
        public string UserEmail { get; set; }
        public string UserPhoneNumber { get; set; }
        public string UserHistory { get; set; }

        public Form4()
        {
            InitializeComponent();
        }

        public Form4(string name, string surname, string email, string phoneNumber, string history)
        {
            InitializeComponent();
            tabControl1.SelectedIndex = 0;
            lblHomeUserName.Text = name + " " + surname;


            UserName = name;
            UserSurname = surname;
            UserEmail = email;
            UserPhoneNumber = phoneNumber;
            UserHistory = history;
        }

        private void pbRes1_Click(object sender, EventArgs e)
        {
            Form5 Res1 = new Form5(UserName, UserSurname, UserEmail, UserPhoneNumber);
            Res1.Show();
            this.Hide();
        }

        private void pbRes2_Click(object sender, EventArgs e)
        {
            Form6 Res2 = new Form6(UserName, UserSurname, UserEmail, UserPhoneNumber);
            Res2.Show();
            this.Hide();
        }

        private void pbRes3_Click(object sender, EventArgs e)
        {
           Form7 Res3 = new Form7(UserName, UserSurname, UserEmail, UserPhoneNumber);
           Res3.Show();
           this.Hide();
        }

        private void btnHomeLogOff_Click(object sender, EventArgs e)
        {
            Form2 frm2 = new Form2();
            frm2.Show();
            this.Close();
        }

        private void Form4_Load(object sender, EventArgs e)
        {

        }

        private void lblHistory_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
