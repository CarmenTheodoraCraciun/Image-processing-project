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
        * Helper
        *-----------------------------------------*/

        private Bitmap grayscaleImage = null;
        private Bitmap hsvImage = null;
        private Bitmap binaryImage = null;

        /*-----------------------------------------
        * Handler
        *-----------------------------------------*/

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog Opfile = new OpenFileDialog();
            Opfile.Filter = "Image Files (*.bmp, *.png, *.jpg, *.jpeg)|*.bmp;*.png;*.jpg;*.jpeg";
            if (DialogResult.OK == Opfile.ShowDialog())
            {
                this.pictureBox1.Image = new Bitmap(Opfile.FileName);
                this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

                grayscaleImage = null;
                hsvImage = null;
                binaryImage = null;

                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true; 
                button6.Enabled = true;
                button7.Enabled = true;
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
            if(grayscaleImage == null)
                grayscaleImage = Effects.ConvertToGrayscale((Bitmap)pictureBox1.Image);

            label2.Text = "Apply grayscale.";
            this.pictureBox2.Image = grayscaleImage;
            this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
        }

        // Convertire RGB la HSV
        private void button4_Click(object sender, EventArgs e)
        {
            if(hsvImage == null)
                hsvImage = Effects.ConvertToHSV((Bitmap)pictureBox1.Image);
            label2.Text = "Apply HSV.";
            this.pictureBox2.Image = hsvImage;
            this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
        }

        // Convertirea de la grey la binar
        private void button5_Click(object sender, EventArgs e)
        {
            if(grayscaleImage == null)
                grayscaleImage = Effects.ConvertToGrayscale((Bitmap)pictureBox1.Image);
            if (binaryImage == null)
                binaryImage = Effects.ConvertToBinary(grayscaleImage, 128);

            label2.Text = "Apply binary.";
            this.pictureBox2.Image = binaryImage;
            this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
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
                if (grayscaleImage == null)
                    grayscaleImage = Effects.ConvertToGrayscale((Bitmap)pictureBox1.Image);

                label2.Text = "Grayscale.";
                this.pictureBox2.Image = grayscaleImage;
                this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;

                FormHistogram histogram = new FormHistogram(grayscaleImage, "grey");
                histogram.Show();

            }
            else
                MessageBox.Show("Choose the type of the photo");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (grayscaleImage == null)
                grayscaleImage = Effects.ConvertToGrayscale((Bitmap)pictureBox1.Image);
            if (binaryImage == null)
                binaryImage = Effects.ConvertToBinary(grayscaleImage, 128);

            label2.Text = "Apply binary.";
            this.pictureBox2.Image = binaryImage;
            this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;

            if (radioButton4.Checked)
            {
                FormApplyToBinaryImage window = new FormApplyToBinaryImage(binaryImage, "BFS");
                window.Show();
            }
            else if (radioButton3.Checked)
            {
                FormApplyToBinaryImage window = new FormApplyToBinaryImage(binaryImage, "RsfAlg");
                window.Show();
            }
            else
                MessageBox.Show("Choose the type of algorithm");
        }
    }
}
