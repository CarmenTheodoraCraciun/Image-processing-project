using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PI_Project
{
    public partial class ImageEffects : Form
    {
        public ImageEffects()
        {
            InitializeComponent();
        }

        /*-----------------------------------------
* Functii ajutatoare
*-----------------------------------------*/


        private void ImageEffects_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog Opfile = new OpenFileDialog();
            Opfile.Filter = "Image Files (*.bmp, *.png, *.jpg, *.jpeg)|*.bmp;*.png;*.jpg;*.jpeg";
            if (DialogResult.OK == Opfile.ShowDialog())
            {
                this.pictureBox1.Image = new Bitmap(Opfile.FileName);
                this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
                button6.Enabled = true;
            }
        }

        // Salvare imagine pe disk
        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JPEG files(*.jpeg)|*.jpeg";
            if (DialogResult.OK == sfd.ShowDialog())
            {
                if (this.pictureBox2 != null)
                    this.pictureBox2.Image.Save(sfd.FileName, ImageFormat.Jpeg);
                else
                    this.pictureBox1.Image.Save(sfd.FileName, ImageFormat.Jpeg);
            }
        }

        // Convertire la nuante de gri
        private void button3_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                label2.Text = "The image in grayscale.";
                Bitmap grayscaleImage = Effects.ConvertToGrayscale((Bitmap)pictureBox1.Image);
                this.pictureBox2.Image = grayscaleImage;
                this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                button5.Enabled = true;
            }
        }

        // Convertire RGB la HSV
        private void button4_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                label2.Text = "The image in HSV.";
                Bitmap hsvImage = Effects.ConvertToHSV((Bitmap)pictureBox1.Image);
                this.pictureBox2.Image = hsvImage;
                this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                button5.Enabled = false;
            }
        }

        // Convertirea de la grey la binar
        private void button5_Click(object sender, EventArgs e)
        {
            label2.Text = "The image binary.";
            int threshold = 128;
            Bitmap binaryImage = Effects.ConvertToBinary((Bitmap)pictureBox2.Image, threshold);
            this.pictureBox2.Image = binaryImage;
            this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            button7.Enabled = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                FormHistogram histogram = new FormHistogram(pictureBox1.Image,"color");
                histogram.Show();
            }
            else if (radioButton2.Checked)
            {
                FormHistogram histogram = new FormHistogram((Bitmap)Effects.ConvertToGrayscale((Bitmap)pictureBox1.Image),"grey");
                histogram.Show();

            }
            else
                MessageBox.Show("Choose the type of the photo");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            
            if (radioButton4.Checked)
            {
                FormApplyToBinaryImage window = new FormApplyToBinaryImage((Bitmap)pictureBox2.Image, "BFS");
                window.Show();
            }
            else if (radioButton3.Checked)
            {
                FormApplyToBinaryImage window = new FormApplyToBinaryImage((Bitmap)pictureBox2.Image, "RsfAlg");
                window.Show();
            }
            else
                MessageBox.Show("Choose the type of algorithm");
        }
    }
}
