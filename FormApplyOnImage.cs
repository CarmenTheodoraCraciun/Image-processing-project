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

        private void ApplyToBinaryImage_Load(object sender, EventArgs e)
        {
            if (type.Equals("BFS"))
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
                dataGridView1.Visible = false;
                listBox1.Visible = false;
                this.Text = "Rosenfeld's algorithm";

                Bitmap processedImage = Effects.ApplyRosenfeld(image);
                pictureBox1.Image = processedImage;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

                MessageBox.Show($"{processedImage.Width}, {processedImage.Height}");
                pictureBox1.Width = processedImage.Width;
                pictureBox1.Height = processedImage.Height;
                this.Width = processedImage.Width;
                this.Height = processedImage.Height;
            }
            else if (type.Equals("Dilate"))
            {
                dataGridView1.Visible = false;
                listBox1.Visible= false;
                this.Text = "Dilate Image";

                Bitmap processedImage = Effects.DilateBinaryImage(image);
                pictureBox1.Image = processedImage;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                this.Width = processedImage.Width;
                this.Height = processedImage.Height;
            }
            else if (type.Equals("Erode"))
            {
                dataGridView1.Visible = false;
                listBox1.Visible = false;
                this.Text = "Erode Image";

                Bitmap processedImage = Effects.ErodeBinaryImage(image);
                pictureBox1.Image = processedImage;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                this.Width = processedImage.Width;
                this.Height = processedImage.Height;
            }
            else if (type.Equals("Binarize"))
            {
                dataGridView1.Visible = false;
                listBox1.Visible = false;
                this.Text = "Apply Binarize Automatically";

                Bitmap processedImage = Effects.BinarizeAutomatically(image);
                pictureBox1.Image = processedImage;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                this.Width = processedImage.Width;
                this.Height = processedImage.Height;
            }
            else if (type.Equals("Negation"))
            {
                dataGridView1.Visible = false;
                listBox1.Visible = false;
                this.Text = "Apply Negation";

                Bitmap processedImage = Effects.InvertImage(image);
                pictureBox1.Image = processedImage;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                this.Width = processedImage.Width;
                this.Height = processedImage.Height;
            }
            else if (type.Equals("Contrast"))
            {
                dataGridView1.Visible = false;
                listBox1.Visible = false;
                this.Text = "Contrast Change";

                Bitmap processedImage = Effects.AdjustContrast(image);
                pictureBox1.Image = processedImage;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                this.Width = processedImage.Width;
                this.Height = processedImage.Height;
            }
            else if (type.Equals("GammaCorrection"))
            {
                dataGridView1.Visible = false;
                listBox1.Visible = false;
                this.Text = "Gamma Correction";

                Bitmap processedImage = Effects.ApplyGammaCorrection(image);
                pictureBox1.Image = processedImage;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                this.Width = processedImage.Width;
                this.Height = processedImage.Height;
            }
            else if (type.Equals("Brigthness"))
            {
                dataGridView1.Visible = false;
                listBox1.Visible = false;
                this.Text = "Brigthness Change";

                Bitmap processedImage = Effects.AdjustBrightness(image);
                pictureBox1.Image = processedImage;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                this.Width = processedImage.Width;
                this.Height = processedImage.Height;
            }
            else if (type.Equals("HistoEq"))
            {
                dataGridView1.Visible = false;
                listBox1.Visible = false;
                this.Text = "Histogram Equal Algorithm";

                Bitmap processedImage = Effects.EqualizeHistogram(image);
                pictureBox1.Image = processedImage;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                this.Width = processedImage.Width;
                this.Height = processedImage.Height;
            }
            else if (type.Equals("Smoot"))
            {
                dataGridView1.Visible = false;
                listBox1.Visible = false;
                this.Text = "Smoothing Image";

                Bitmap processedImage = Effects.SmoothingImage(image);
                pictureBox1.Image = processedImage;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                this.Width = processedImage.Width;
                this.Height = processedImage.Height;
            }
            else if (type.Equals("DtcEdges"))
            {
                dataGridView1.Visible = false;
                listBox1.Visible = false;
                this.Text = "Detect Edges";

                Bitmap processedImage = Effects.DetectEdges(image);
                pictureBox1.Image = processedImage;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                this.Width = processedImage.Width;
                this.Height = processedImage.Height;
            }
        }
    }
}
