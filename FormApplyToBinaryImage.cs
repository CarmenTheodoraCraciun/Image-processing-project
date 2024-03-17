using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
                Pen pen = new Pen(Color.Black);
                foreach (List<Point> points in pointsLists)
                {
                    for (int i = 0; i < points.Count - 1; i++)
                    {
                        g.DrawLine(pen, points[i], points[i + 1]);
                    }
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
                List<List<Point>> bfs = Effects.BFS(image);
                pictureBox1.Image = drawGraph(bfs);
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                this.Text = "Breadth-First Search";
            }
            else if (type.Equals("RsfAlg"))
            {
                pictureBox1.Image = Effects.ApplyRosenfeld(image);
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                this.Text = "Rosenfeld's algorithm";
            }
        }
    }
}
