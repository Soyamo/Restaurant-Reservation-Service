using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace Restaurant_Reservation_System_FinalProject_26
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            pictureBox2.Click += pictureBox2_Click;
            pictureBox3.Click += pictureBox3_Click;
            pictureBox4.Click += pictureBox4_Click;
        }

        private void btnProceed_Click(object sender, EventArgs e)
        {
            Form2 frm2 = new Form2();
            frm2.Show();
            this.Hide();
        }
        //Instagram
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            OpenUrl("http://www.instagram.com");
        }
        //FaceBook
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            OpenUrl("http://www.facebook.com");
        }
        //X
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            OpenUrl("http://www.x.com");
        }

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Unable to open the URL: " + ex.Message);
            }
        }
    }
}
