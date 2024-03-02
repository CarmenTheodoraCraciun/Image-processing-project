using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PI_Project
{
    public partial class FormHistogram : Form
    {
        public Image image { get; private set; }
        public string type { get; set; }

        public FormHistogram(Image image,string type)
        {
            InitializeComponent();
            this.image = image;
            this.type = type;
        }

        private void creatColorHist()
        {
            // Setam invizibilitatea elementelor ce tin exclusiv de imaginea gri
            pictureBox1.Visible = false;
            pictureBox2.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            
            // Generam histograma pentru imaginea data
            int[][] histograms = Effects.GenerateColorHistogram((Bitmap)this.image);

            // Adauga valorile histogramelor in cele trei obiecte Chart
            for (int i = 0; i < histograms.Length && i < 3; i++)
            {
                Chart chart = null;

                // Alege obiectul Chart corespunzător in funcție de indexul i
                if (i == 0) // Rosu
                    chart = chart1;
                else if (i == 1) // Albastru
                    chart = chart2;
                else if (i == 2) // Verde
                    chart = chart3;

                if (chart != null)
                {
                    Series series = chart.Series[0];
                    series.IsVisibleInLegend = false; // Setam invizibiltatea legendei
                    series.Points.Clear(); // Curatam punctele existente

                    for (int j = 0; j < histograms[i].Length; j++)
                    {
                        series.Points.AddXY(j, histograms[i][j]);
                    }

                    // Setam culorile pentru fiecare serie
                    if (i == 0)
                    {
                        series.Color = Color.Red;
                        series.LegendText = "Red";
                    }
                    else if (i == 1)
                    {
                        series.Color = Color.Green;
                        series.LegendText = "Green";
                    }
                    else if (i == 2)
                    {
                        series.Color = Color.Blue;
                        series.LegendText = "Blue";
                    }

                    // Calculam latimea ferestrei
                    int width = 0;
                    if (chart1 != null)
                        width += chart1.Width;
                    if (chart2 != null)
                        width += chart2.Width;
                    if (chart3 != null)
                        width += chart3.Width;

                    // Setam marimea ferestrei
                    this.ClientSize = new Size(width, chart1.Height + 50);
                }
            }

            // Determinam pragurile multiple si le afisam
            int[] thresholds = Effects.FindMultipleThresholds(histograms);
            textBox1.Text = thresholds[0].ToString();
            textBox2.Text = thresholds[1].ToString();
            textBox3.Text = thresholds[2].ToString();
        }

        private void createGreyHist()
        {
            // Setam invizibilitatea a doua chart-uri
            chart2.Visible = false;
            chart3.Visible = false;

            // Generam histograma
            int[] histogram = Effects.GenerateGrayscaleHistogram((Bitmap)this.image);

            // Adauga valorile histogramelor in controlul Chart
            Series series = chart1.Series[0];
            series.IsVisibleInLegend = false;
            series.Color = Color.Black;
            series.Points.Clear(); // Curatam punctele existente

            for (int i = 0; i < histogram.Length; i++)
            {
                series.Points.AddXY(i, histogram[i]);
            }

            int[] thresholdsForGrayscale = Effects.FindMultipleThresholdsForGrayscale(histogram);
            label1.Text = "Grey";
            textBox1.Text = thresholdsForGrayscale[0].ToString();

            // Inseram imaginile
            Bitmap segmentedGrayImage = Effects.ApplyThresholdsForGrayscale((Bitmap)this.image, thresholdsForGrayscale);
            pictureBox1.Image = segmentedGrayImage;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            Bitmap ditheredImage = Effects.ApplyFloydSteinbergDithering(segmentedGrayImage);
            pictureBox2.Image = ditheredImage;
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;

            this.ClientSize = new Size(2* pictureBox1.Width + chart1.Width, chart1.Height + 50);
        }

        private void Histogram_Load(object sender, EventArgs e)
        {

            if (type.Equals("color"))
            {
                creatColorHist();
                this.Text = "Color Histogram";
            }
            else if (type.Equals("grey"))
            {
                createGreyHist();
                this.Text = "Grey Histogram";
            }
            
        }
    }
}
