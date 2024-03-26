using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PI_Project
{
    public partial class FormApplyToBinaryImage : Form
    {
        public Bitmap image { get; private set; }
        public string type { get; set; }

        private Bitmap drawGraph(List<List<Point>> pointsLists)
        {
            // Determină dimensiunile imaginii în funcție de dimensiunile punctelor
            int maxX = 0;
            int maxY = 0;
            foreach (List<Point> points in pointsLists)
            {
                foreach (Point point in points)
                {
                    if (point.X > maxX)
                        maxX = point.X;
                    if (point.Y > maxY)
                        maxY = point.Y;
                }
            }

            // Creează o imagine cu dimensiunile corespunzătoare
            Bitmap graphBitmap = new Bitmap(maxX + 1, maxY + 1);
            using (Graphics g = Graphics.FromImage(graphBitmap))
            {
                g.Clear(Color.White);

                // Desenează graficul pe imaginea nouă
                foreach (List<Point> points in pointsLists)
                {
                    foreach (Point point in points)
                    {
                        // Setează simbolul în funcție de existența punctului
                        char symbol = points.Count > 0 ? '>' : '<';
                        g.DrawString(symbol.ToString(), new Font("Arial", 8), Brushes.Black, point.X, point.Y);
                    }
                }
            }

            return graphBitmap;
        }

        private static Bitmap drawGraph(List<Point> points)
        {
            // Determină dimensiunile imaginii în funcție de coordonatele punctelor
            int maxX = 0;
            int maxY = 0;
            foreach (var point in points)
            {
                if (point.X > maxX)
                    maxX = point.X;
                if (point.Y > maxY)
                    maxY = point.Y;
            }

            // Creează o imagine cu dimensiunile corespunzătoare
            Bitmap graphBitmap = new Bitmap(maxX + 1, maxY + 1);
            using (Graphics g = Graphics.FromImage(graphBitmap))
            {
                g.Clear(Color.White);

                // Desenează liniile între puncte
                Pen pen = new Pen(Color.Black);
                pen.Width = 2; // Setează grosimea liniei
                pen.StartCap = LineCap.Round; // Capăt rotund al liniei

                for (int i = 0; i < points.Count - 1; i++)
                {
                    g.DrawLine(pen, points[i], points[i + 1]);
                }
            }

            return graphBitmap;
        }

        public FormApplyToBinaryImage(Bitmap image, string type)
        {
            InitializeComponent();
            this.image = image;
            this.type = type;
        }

        private void ApplyToBinaryImage_Load(object sender, EventArgs e)
        {
            if (type.Equals("BFS"))
            {
                List<List<Point>> bfs = Effects.BFS(image); //lista de puncte in binar
                int objects = bfs.Count; //numarul de obiecte
                List<double> areas = Effects.CalculateAreas(bfs);
                List<double> perimeters = Effects.CalculatePerimeters(bfs);
                List<Point> centroids = Effects.CalculateCentroids(bfs);

                pictureBox1.Image = drawGraph(bfs);
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                this.Text = "Breadth-First Search";
                pictureBox2.Image = drawGraph(centroids);
                pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                listBox1.Items.Add("Area\t\tPerimeter\t\tCentroid");

                int[] sortedIndices = Enumerable.Range(0, areas.Count).OrderByDescending(i => areas[i]).ToArray();

                List<double> sortedPerimeters = sortedIndices.Select(i => perimeters[i]).ToList();
                List<Point> sortedCentroids = sortedIndices.Select(i => centroids[i]).ToList();

                // Adaugă fiecare valoare într-un format de tabel în ListBox
                for (int i = 0; i < 27; i++)
                {
                    string areaText = areas[sortedIndices[i]].ToString();
                    string perimeterText = sortedPerimeters[i].ToString();
                    string centroidText = $"({sortedCentroids[i].X}, {sortedCentroids[i].Y})";

                    listBox1.Items.Add($"{areaText,-15}\t\t{perimeterText,-15}\t\t{centroidText}");
                }
            }
            else if (type.Equals("RsfAlg"))
            {
                Bitmap processedImage = Effects.ApplyRosenfeld(image);
                pictureBox1.Image = processedImage;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

                pictureBox1.Width = processedImage.Width;
                pictureBox1.Height = processedImage.Height;

                this.Text = "Rosenfeld's algorithm";

                pictureBox2.Visible = false;
                listBox1.Visible = false;
            }
        }
    }
}
