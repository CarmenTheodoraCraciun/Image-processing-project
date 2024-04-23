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

        public FormApplyOnImage(Bitmap image, string type)
        {
            InitializeComponent();
            this.image = image;
            this.type = type;
        }

        private void NotVisible()
        {
            dataGridView1.Visible = false;
            listBox1.Visible = false;
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
                NotVisible();
                this.Text = "Grayscale";
                PutImage(this.image);
                ResizeWindow();
            }
            else if (type.Equals("Hsv"))
            {
                NotVisible();
                this.Text = "HSV";
                PutImage(Effects.ConvertToHSV(image));
                ResizeWindow();
            }
            else if (type.Equals("Binary"))
            {
                NotVisible();
                this.Text = "Binary";
                PutImage(this.image);
                ResizeWindow();
            }
            else if (type.Equals("BFS"))
            {
                this.Text = "Breadth-First Search";

                List<List<Point>> bfs = Effects.BFS(image);
                
                // Geometric data
                List<double> areas = Effects.CalculateAreas(bfs);
                List<double> perimeters = Effects.CalculatePerimeters(bfs);
                List<Point> centroids = Effects.CalculateCentroids(bfs);
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
                List<List<Point>> c = Effects.FindContours(image);
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
                NotVisible();
                this.Text = "Rosenfeld's algorithm";
                PutImage(Effects.ApplyRosenfeld(image));
                ResizeWindow();
            }
            else if (type.Equals("Dilate"))
            {
                NotVisible();
                this.Text = "Dilate Image";
                PutImage(Effects.DilateBinaryImage(image));
                ResizeWindow();
            }
            else if (type.Equals("Erode"))
            {
                NotVisible();
                this.Text = "Erode Image";
                PutImage(Effects.ErodeBinaryImage(image));
                ResizeWindow();
            }
            else if (type.Equals("Binarize"))
            {
                NotVisible();
                this.Text = "Apply Binarize Automatically";
                PutImage(Effects.BinarizeAutomatically(image));
                ResizeWindow();
            }
            else if (type.Equals("Negation"))
            {
                NotVisible();
                this.Text = "Apply Negation";
                PutImage(Effects.InvertImage(image));
                ResizeWindow();
            }
            else if (type.Equals("Contrast"))
            {
                NotVisible();
                this.Text = "Contrast Change";
                PutImage(Effects.AdjustContrast(image));
                ResizeWindow();
            }
            else if (type.Equals("GammaCorrection"))
            {
                NotVisible();
                this.Text = "Gamma Correction";
                PutImage(Effects.ApplyGammaCorrection(image));
                ResizeWindow();
            }
            else if (type.Equals("Brigthness"))
            {
                NotVisible();
                this.Text = "Brigthness Change";
                PutImage(Effects.AdjustBrightness(image));
                ResizeWindow();
            }
            else if (type.Equals("HistoEq"))
            {
                NotVisible();
                this.Text = "Histogram Equal Algorithm";
                PutImage(Effects.EqualizeHistogram(image));
                ResizeWindow();
            }
            else if (type.Equals("SmootSpace"))
            {
                NotVisible();
                this.Text = "Smoothing Image in space domain";
                PutImage(Effects.SmoothingImageSpace(image));
                ResizeWindow();
            }
            else if (type.Equals("DtcEdgesSpace"))
            {
                NotVisible();
                this.Text = "Detect Edges in space domain";
                PutImage(Effects.DetectEdgesSpace(image));
                ResizeWindow();
            }
            else if (type.Equals("SmootFreq"))
            {
                NotVisible();
                this.Text = "Smoothing Image in frequency domain";
                PutImage(Effects.SmoothingImageFreq(image));
                ResizeWindow();
            }
            else if (type.Equals("Gauss"))
            {
                NotVisible();
                this.Text = "Restoration Gauss";
                PutImage(Effects.RestorationGauss(image));
                ResizeWindow();
            }
            else if (type.Equals("Bid"))
            {
                NotVisible();
                this.Text = "Restoration Bidimensional";
                PutImage(Effects.RestorationBi(image));
                ResizeWindow();
            }
        }
    }
}
