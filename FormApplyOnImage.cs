using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PI_Project
{
    public partial class FormApplyOnImage : Form
    {
        public Bitmap image { get; private set; }
        public string type { get; set; }
        private Mat _adaptiveBinMat;
        private Bitmap _adaptiveBinBitmap;

        public FormApplyOnImage(Bitmap image, string type)
        {
            InitializeComponent();
            this.image = image;
            this.type = type;
        }

        private void NotVisibleGridListBox()
        {
            dataGridView1.Visible = false;
            listBox1.Visible = false;
        }

        private void NotVisiblePrcessingTime()
        {
            label2.Visible = false;
            textBox1.Visible = false;
        }

        private void NotVisibleImg()
        {
            pictureBox2.Visible = false;
        }

        private void PutImage(Bitmap image)
        {
            pictureBox1.Image = image;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Width = image.Width;
            pictureBox1.Height = image.Height;
            
        }
        private void ResizeWindow()
        {
            this.Width = pictureBox1.Width;
            this.Height = pictureBox1.Height;
        }

        private void ApplyToBinaryImage_Load(object sender, EventArgs e)
        {
            if (type.Equals("Gray"))
            {
                NotVisibleImg();
                NotVisibleGridListBox();
                NotVisiblePrcessingTime();
                this.Text = "Grayscale";
                PutImage(this.image);
                ResizeWindow();
            }
            else if (type.Equals("Hsv"))
            {
                NotVisibleImg();
                NotVisibleGridListBox();
                NotVisiblePrcessingTime();
                this.Text = "HSV";
                PutImage(Effects.ConvertToHSV(image));
                ResizeWindow();
            }
            else if (type.Equals("Binary"))
            {
                NotVisibleImg();
                NotVisibleGridListBox();
                NotVisiblePrcessingTime();
                this.Text = "Binary";
                PutImage(this.image);
                ResizeWindow();
            }
            else if (type.Equals("BFS"))
            {
                this.Text = "Breadth-First Search";
                NotVisibleImg();
                NotVisiblePrcessingTime();
                List<List<System.Drawing.Point>> bfs = Effects.BFS(image);
                
                // Geometric data
                List<double> areas = Effects.CalculateAreas(bfs);
                List<double> perimeters = Effects.CalculatePerimeters(bfs);
                List<System.Drawing.Point> centroids = Effects.CalculateCentroids(bfs);
                dataGridView1.Columns.Add("AreaColumn", "Area");
                dataGridView1.Columns.Add("PerimeterColumn", "Perimeter");
                dataGridView1.Columns.Add("CentroidColumn", "Centroid");

                for (int i = 0; i < Math.Max(Math.Max(areas.Count, perimeters.Count), centroids.Count); i++)
                {
                    string areaText = i < areas.Count ? areas[i].ToString() : "";
                    string perimeterText = i < perimeters.Count ? perimeters[i].ToString() : "";
                    string centroidText = i < centroids.Count ? $"({centroids[i].X}, {centroids[i].Y})" : "";
                    dataGridView1.Rows.Add(areaText, perimeterText, centroidText);
                }

                // Get Contour
                List<List<System.Drawing.Point>> c = Effects.FindContours(image);
                Bitmap processedImage1 = Effects.DrawImage(c);
                pictureBox1.Image = processedImage1;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

                // Get Chain Code
                List<int> chainCode = Effects.extractChainCode(bfs);
                listBox1.Items.Clear();
                foreach (int code in chainCode)
                {
                    listBox1.Items.Add(code.ToString() + ", ");
                }
            }
            else if (type.Equals("RsfAlg"))
            {
                NotVisibleImg();
                NotVisiblePrcessingTime();
                NotVisibleGridListBox();
                this.Text = "Rosenfeld's algorithm";
                PutImage(Effects.ApplyRosenfeld(image));
                ResizeWindow();
            }
            else if (type.Equals("Dilate"))
            {
                NotVisibleImg();
                NotVisiblePrcessingTime();
                NotVisibleGridListBox();
                this.Text = "Dilate Image";
                PutImage(Effects.DilateBinaryImage(image));
                ResizeWindow();
            }
            else if (type.Equals("Erode"))
            {
                NotVisibleImg();
                NotVisiblePrcessingTime();
                NotVisibleGridListBox();
                this.Text = "Erode Image";
                PutImage(Effects.ErodeBinaryImage(image));
                ResizeWindow();
            }
            else if (type.Equals("Binarize"))
            {
                NotVisibleImg();
                NotVisiblePrcessingTime();
                NotVisibleGridListBox();
                this.Text = "Apply Binarize Automatically";
                PutImage(Effects.BinarizeAutomatically(image));
                ResizeWindow();
            }
            else if (type.Equals("Negation"))
            {
                NotVisibleImg();
                NotVisiblePrcessingTime();
                NotVisibleGridListBox();
                this.Text = "Apply Negation";
                PutImage(Effects.InvertImage(image));
                ResizeWindow();
            }
            else if (type.Equals("Contrast"))
            {
                NotVisibleImg();
                NotVisiblePrcessingTime();
                NotVisibleGridListBox();
                this.Text = "Contrast Change";
                PutImage(Effects.AdjustContrast(image));
                ResizeWindow();
            }
            else if (type.Equals("GammaCorrection"))
            {
                NotVisibleImg();
                NotVisiblePrcessingTime();
                NotVisibleGridListBox();
                this.Text = "Gamma Correction";
                PutImage(Effects.ApplyGammaCorrection(image));
                ResizeWindow();
            }
            else if (type.Equals("Brigthness"))
            {
                NotVisibleImg();
                NotVisiblePrcessingTime();
                NotVisibleGridListBox();
                this.Text = "Brigthness Change";
                PutImage(Effects.AdjustBrightness(image));
                ResizeWindow();
            }
            else if (type.Equals("HistoEq"))
            {
                NotVisibleImg();
                NotVisiblePrcessingTime();
                NotVisibleGridListBox();
                this.Text = "Histogram Equal Algorithm";
                PutImage(Effects.EqualizeHistogram(image));
                ResizeWindow();
            }
            else if (type.Equals("SmootSpace"))
            {
                NotVisibleImg();
                NotVisiblePrcessingTime();
                NotVisibleGridListBox();
                this.Text = "Smoothing Image in space domain";
                PutImage(Effects.SmoothingImageSpace(image));
                ResizeWindow();
            }
            else if (type.Equals("DtcEdgesSpace"))
            {
                NotVisibleImg();
                NotVisiblePrcessingTime();
                NotVisibleGridListBox();
                this.Text = "Detect Edges in space domain";
                PutImage(Effects.DetectEdgesSpace(image));
                ResizeWindow();
            }
            else if (type.Equals("SmootFreq"))
            {
                NotVisibleImg();
                NotVisiblePrcessingTime();
                NotVisibleGridListBox();
                this.Text = "Smoothing Image in frequency domain";
                PutImage(Effects.SmoothingImageFreq(image));
                ResizeWindow();
            }
            else if (type.Equals("DtcEdgesFreq"))
            {
                NotVisibleImg();
                NotVisiblePrcessingTime();
                NotVisibleGridListBox();
                this.Text = "Detect Edges in frequency domain";
                PutImage(Effects.DetectEdgesSpace(image));
                ResizeWindow();
            }
            else if (type.Equals("Gauss"))
            {
                NotVisibleImg();
                NotVisibleGridListBox();
                this.Text = "Restoration Gauss";
                (Bitmap img, double time) = Effects.RestorationGauss(image);
                PutImage(img);
                ResizeWindow();
                this.Width = pictureBox1.Height + 45;
                textBox1.Text = time.ToString();
            }
            else if (type.Equals("Bid"))
            {
                NotVisibleImg();
                NotVisibleGridListBox();
                this.Text = "Restoration Bidimensional";
                (Bitmap img, double time) = Effects.RestorationBi(image);
                PutImage(img);
                ResizeWindow();
                this.Width = pictureBox1.Height + 45;
                textBox1.Text = time.ToString();
            }
            else if (type.Equals("AdpBin"))
            {
                NotVisiblePrcessingTime();
                NotVisibleGridListBox();
                NotVisiblePrcessingTime();
                this.Text = "Adaptive binarization of edge points";
                if (_adaptiveBinMat == null)
                    (_adaptiveBinBitmap, _adaptiveBinMat) = Effects.AdaptiveEdgeThresholding(image);
                PutImage(_adaptiveBinBitmap);
                ResizeWindow();
            }
            else if (type.Equals("EdgeExt"))
            {
                NotVisiblePrcessingTime();
                NotVisibleGridListBox();
                this.Text = "Edge extension by hysteresis";
                if (_adaptiveBinMat == null)
                    (_adaptiveBinBitmap, _adaptiveBinMat) = Effects.AdaptiveEdgeThresholding(image);
                PutImage(Effects.EdgeExtensionThresholding(_adaptiveBinMat));
                ResizeWindow();
            }
        }
    }
}
