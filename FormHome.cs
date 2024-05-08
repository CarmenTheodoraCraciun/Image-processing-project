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

        private Bitmap original = null;
        private Bitmap grayscaleImage = null;
        private Bitmap binaryImage = null;

        /*-----------------------------------------
        * Handler
        *-----------------------------------------*/

        // Incarca imaginea si afisaza-o
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog Opfile = new OpenFileDialog();
            Opfile.Filter = "Image Files (*.bmp, *.png, *.jpg, *.jpeg)|*.bmp;*.png;*.jpg;*.jpeg";
            if (DialogResult.OK == Opfile.ShowDialog())
            {
                this.pictureBox1.Image = new Bitmap(Opfile.FileName);
                this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

                original = (Bitmap)(this.pictureBox1.Image);
                grayscaleImage = null;
                binaryImage = null;

                button2.Enabled = true;
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
               this.pictureBox1.Image.Save(sfd.FileName, ImageFormat.Jpeg);
            }
        }

        // Convertire la nuante de gri
        

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
                binaryImage = Effects.ConvertToBinary(grayscaleImage);
            FormApplyOnImage window = null;
            if (radioButton17.Checked)
                window = new FormApplyOnImage(grayscaleImage, "Gray");
            else if (radioButton18.Checked)
                window = new FormApplyOnImage((Bitmap)pictureBox1.Image, "Hsv");
            else if (radioButton19.Checked)
                window = new FormApplyOnImage(binaryImage, "Binary");
            else if (radioButton4.Checked)
                window = new FormApplyOnImage(binaryImage, "BFS");
            else if (radioButton3.Checked)
                window = new FormApplyOnImage(binaryImage, "RsfAlg");
            else if (radioButton6.Checked)
                window = new FormApplyOnImage(binaryImage, "Dilate");
            else if (radioButton5.Checked)
                window = new FormApplyOnImage(binaryImage, "Erode");
            else if (radioButton7.Checked)
                window = new FormApplyOnImage(grayscaleImage, "Binarize");
            else if (radioButton8.Checked)
                window = new FormApplyOnImage((Bitmap)pictureBox1.Image, "Negation");
            else if (radioButton9.Checked)
                window = new FormApplyOnImage((Bitmap)pictureBox1.Image, "Contrast");
            else if (radioButton10.Checked)
                window = new FormApplyOnImage((Bitmap)pictureBox1.Image, "GammaCorrection");
            else if (radioButton11.Checked)
                window = new FormApplyOnImage((Bitmap)pictureBox1.Image, "Brigthness");
            else if (radioButton12.Checked)
                window = new FormApplyOnImage(grayscaleImage, "HistoEq");
            else if (radioButton13.Checked)
                window = new FormApplyOnImage(original, "SmootSpace");
            else if (radioButton14.Checked)
                window = new FormApplyOnImage(original, "DtcEdgesSpace");
            else if (radioButton15.Checked)
                window = new FormApplyOnImage(original, "SmootFreq");
            else if (radioButton16.Checked)
                window = new FormApplyOnImage(original, "DtcEdgesFreq");
            else if (radioButton20.Checked)
                window = new FormApplyOnImage(original, "Gauss");
            else if (radioButton21.Checked)
                window = new FormApplyOnImage(original, "Bid");
            else if (radioButton22.Checked)
                window = new FormApplyOnImage(original, "AdpBin");
            else if (radioButton23.Checked)
                window = new FormApplyOnImage(original, "EdgeExt");
            else
                MessageBox.Show("Choose the type of algorithm");

            if (window != null)
                window.Show();
        }
    }
}
