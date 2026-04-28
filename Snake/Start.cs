using System;
using System.Drawing;
using System.Windows.Forms;

namespace Snake
{
    public partial class Start : Form
    {
        private Snake s;

        public Start()
        {
            InitializeComponent();
        }

        private void Start_Load(object sender, EventArgs e)
        {
            // Select first theme by default
            s = new Snake();
            HighlightSelected(theme1);
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
            s = new Snake();
            s.BackgroundImage = theme1.BackgroundImage;
            HighlightSelected(theme1);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            s = new Snake();
            s.BackgroundImage = pictureBox1.BackgroundImage;
            HighlightSelected(pictureBox1);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            s = new Snake();
            s.BackgroundImage = pictureBox2.BackgroundImage;
            HighlightSelected(pictureBox2);
        }

        private void Go_Click(object sender, EventArgs e)
        {
            if (s == null)
            {
                MessageBox.Show("Please select a theme first!", "No Theme Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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