using System;
using System.Drawing;
using System.Windows.Forms;

namespace Snake
{
    public partial class Start : Form
    {
        private Snake s;
        PictureBox theme = new PictureBox();

        public Start()
        {
            InitializeComponent();
        }

        private void Start_Load(object sender, EventArgs e)
        {
            // Select first theme by default
            //s = new Snake(this);
            //HighlightSelected(theme1);
        }

        private void HighlightSelected(PictureBox selected)
        {
            theme1.BorderStyle = BorderStyle.None;
            pictureBox1.BorderStyle = BorderStyle.None;
            pictureBox2.BorderStyle = BorderStyle.None;
            selected.BorderStyle = BorderStyle.Fixed3D;
        }

        private void theme1_Click(object sender, EventArgs e)
        {

            theme = theme1;
            HighlightSelected(theme1);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

            theme = pictureBox1;

            HighlightSelected(pictureBox1);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

            theme = pictureBox2;

            HighlightSelected(pictureBox2);
        }

        private void Go_Click(object sender, EventArgs e)
        {

            if (theme.BackgroundImage == null)
            {
                MessageBox.Show("Please select a theme first!", "No Theme Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            s = new Snake(this);
            s.BackgroundImage = theme.BackgroundImage;

            this.Hide();
            s.FormClosed += (ss, ee) => this.Show(); // Return to menu when game closes
            s.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}