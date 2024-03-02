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

        private Bitmap ConvertToGrayscale(Bitmap originalImage)
        {
            Bitmap grayscaleImage = new Bitmap(originalImage.Width, originalImage.Height);
            // Parcurgem pixel cu pixel imaginea
            for (int i = 0; i < originalImage.Width; i++)
            {
                for (int j = 0; j < originalImage.Height; j++)
                {
                    // Luam culoare de pe pozitia ij
                    Color pixelColor = originalImage.GetPixel(i, j);
                    // Calculam valoare media RGB
                    int grayValue = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);
                    // Cream culoare pe scara gri
                    Color grayColor = Color.FromArgb(grayValue, grayValue, grayValue);
                    // Setam pixelul pe pozitia i, j a noi imagini
                    grayscaleImage.SetPixel(i, j, grayColor);
                }
            }
            return grayscaleImage;
        }

        private Bitmap ConvertToHSV(Bitmap originalImage)
        {
            Bitmap hsvImage = new Bitmap(originalImage.Width, originalImage.Height);

            for (int i = 0; i < originalImage.Width; i++)
            {
                for (int j = 0; j < originalImage.Height; j++)
                {
                    Color rgbColor = originalImage.GetPixel(i, j);
                    // Converteste RGB la HSV
                    float h, s, v;
                    RGBToHSV(rgbColor.R, rgbColor.G, rgbColor.B, out h, out s, out v);
                    // Converteste HSV înapoi la RGB pentru afișare.
                    Color hsvColor = HSVToRGB(h, s, v);
                    // Setează pixelul în noua imagine.
                    hsvImage.SetPixel(i, j, hsvColor);
                }
            }
            return hsvImage;
        }

        private void RGBToHSV(int r, int g, int b, out float h, out float s, out float v)
        {
            float min, max, delta;

            min = Math.Min(Math.Min(r, g), b);
            max = Math.Max(Math.Max(r, g), b);
            v = max;

            delta = max - min;

            if (max != 0)
                s = delta / max; // saturatia
            else
            {
                // r = g = b = 0 (neagru)
                s = 0;
                h = -1; // valoare nedefinita pentru nuanta
                return;
            }

            if (r == max)
                h = (g - b) / delta; // intre galben si magenta
            else if (g == max)
                h = 2 + (b - r) / delta; // intre cyan si galben
            else
                h = 4 + (r - g) / delta; // intre magenta si cyan
            h *= 60; // converteste la grade
            if (h < 0)
                h += 360;
        }

        private Color HSVToRGB(float h, float s, float v)
        {
            int i;
            float f, p, q, t;
            if (s == 0)
                return Color.FromArgb((int)v, (int)v, (int)v); // Achromatic (gri)

            h /= 60; // sector 0 până la 5
            i = (int)Math.Floor(h);
            f = h - i; // partea fracționară a lui h
            p = v * (1 - s);
            q = v * (1 - s * f);
            t = v * (1 - s * (1 - f));

            switch (i)
            {
                case 0:
                    return Color.FromArgb((int)v, (int)t, (int)p);
                case 1:
                    return Color.FromArgb((int)q, (int)v, (int)p);
                case 2:
                    return Color.FromArgb((int)p, (int)v, (int)t);
                case 3:
                    return Color.FromArgb((int)p, (int)q, (int)v);
                case 4:
                    return Color.FromArgb((int)t, (int)p, (int)v);
                default: // case 5:
                    return Color.FromArgb((int)v, (int)p, (int)q);
            }
        }

        private Bitmap ConvertToBinary(Bitmap grayscaleImage, int threshold)
        {
            Bitmap binaryImage = new Bitmap(grayscaleImage.Width, grayscaleImage.Height);

            for (int i = 0; i < grayscaleImage.Width; i++)
            {
                for (int j = 0; j < grayscaleImage.Height; j++)
                {
                    Color pixelColor = grayscaleImage.GetPixel(i, j);
                    int grayValue = pixelColor.R;  // Avem o imagine în tonuri de gri, așa că putem utiliza oricare dintre canalele R, G, B

                    // Aplică pragul
                    Color binaryColor = (grayValue < threshold) ? Color.Black : Color.White;
                    binaryImage.SetPixel(i, j, binaryColor);
                }
            }

            return binaryImage;
        }

        /*-----------------------------------------
        * Functiile event handler
        *-----------------------------------------*/

        private void ImageEffects_Load(object sender, EventArgs e)
        {

        }

        // Incarcare imagine
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
                Bitmap grayscaleImage = ConvertToGrayscale((Bitmap)pictureBox1.Image);
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
                Bitmap hsvImage = ConvertToHSV((Bitmap)pictureBox1.Image);
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
            Bitmap binaryImage = ConvertToBinary((Bitmap)pictureBox2.Image, threshold);
            this.pictureBox2.Image = binaryImage;
            this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
        }
    }
}
