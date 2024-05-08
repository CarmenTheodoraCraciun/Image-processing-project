using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using OpenCvSharp;

using System.Drawing.Imaging;

using Accord.Imaging;
using Accord.Math;
using Accord.Imaging.Filters;
using System.Numerics;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace PI_Project
{
    public static class Effects
    {
        // Creaza o imagine pornind de la o matrice de puncte
        public static Bitmap DrawImage(List<List<System.Drawing.Point>> pointsLists)
        {
            int maxX = 0;
            int maxY = 0;
            foreach (List<System.Drawing.Point> points in pointsLists)
            {
                foreach (System.Drawing.Point point in points)
                {
                    if (point.X > maxX)
                        maxX = point.X;
                    if (point.Y > maxY)
                        maxY = point.Y;
                }
            }
            Bitmap graphBitmap = new Bitmap(maxX + 1, maxY + 1);
            using (Graphics g = Graphics.FromImage(graphBitmap))
            {
                g.Clear(Color.White);
                foreach (List<System.Drawing.Point> points in pointsLists)
                {
                    foreach (System.Drawing.Point point in points)
                    {
                        char symbol = points.Count > 0 ? '>' : '<';
                        g.DrawString(symbol.ToString(), new Font("Arial", 8), Brushes.Black, point.X, point.Y);
                    }
                }
            }
            return graphBitmap;
        }

        /*******************************TEME*******************************/

        /*********************Tema 2*********************/

        // Formula ponderata pentru calculul valorii grayscale
        private static int CalculateWeightedValue(Color color)
        {
            return (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
        }

        // Conversia unei imagini la gri
        public static Bitmap ConvertToGrayscale(Bitmap originalImage)
        {
            Bitmap grayscaleImage = new Bitmap(originalImage.Width, originalImage.Height);
            for (int i = 0; i < originalImage.Width; i++)
            {
                for (int j = 0; j < originalImage.Height; j++)
                {
                    Color pixelColor = originalImage.GetPixel(i, j);
                    int grayValue = CalculateWeightedValue(pixelColor);
                    Color grayColor = Color.FromArgb(grayValue, grayValue, grayValue);
                    grayscaleImage.SetPixel(i, j, grayColor);
                }
            }
            return grayscaleImage;
        }

        // Conversia de la RGB la HSV
        private static void RGBToHSV(int r, int g, int b, out float h, out float s, out float v)
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

        // Conversia de la HSV la RGB
        private static Color HSVToRGB(float h, float s, float v)
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
                default:
                    return Color.FromArgb((int)v, (int)p, (int)q);
            }
        }

        // Conversia la HSV
        public static Bitmap ConvertToHSV(Bitmap originalImage)
        {
            Bitmap hsvImage = new Bitmap(originalImage.Width, originalImage.Height);

            for (int i = 0; i < originalImage.Width; i++)
            {
                for (int j = 0; j < originalImage.Height; j++)
                {
                    Color rgbColor = originalImage.GetPixel(i, j);
                    float h, s, v;
                    RGBToHSV(rgbColor.R, rgbColor.G, rgbColor.B, out h, out s, out v);
                    Color hsvColor = HSVToRGB(h, s, v);
                    hsvImage.SetPixel(i, j, hsvColor);
                }
            }
            return hsvImage;
        }

        // Conversia de la o imagine gri la binara (alb negru)
        public static Bitmap ConvertToBinary(Bitmap grayscaleImage, int threshold = 128)
        {
            Bitmap binaryImage = new Bitmap(grayscaleImage.Width, grayscaleImage.Height);

            for (int i = 0; i < grayscaleImage.Width; i++)
            {
                for (int j = 0; j < grayscaleImage.Height; j++)
                {
                    Color pixelColor = grayscaleImage.GetPixel(i, j);
                    int grayValue = pixelColor.R;
                    Color binaryColor = (grayValue < threshold) ? Color.Black : Color.White;
                    binaryImage.SetPixel(i, j, binaryColor);
                }
            }
            return binaryImage;
        }

        /*********************Tema 3*********************/

        // Generarea histogramei color. Histograma este organizata pe trei canale de culoare: rosu, verde si albastru.
        public static int[][] GenerateColorHistogram(Bitmap image)
        {
            int[][] histograms = new int[3][];
            histograms[0] = new int[256]; // Red
            histograms[1] = new int[256]; // Green
            histograms[2] = new int[256]; // Blue
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    Color pixelColor = image.GetPixel(i, j);
                    ++histograms[0][pixelColor.R];
                    ++histograms[1][pixelColor.G];
                    ++histograms[2][pixelColor.B];
                }
            }
            return histograms;
        }

        // Generarea histogramei gri
        public static int[] GenerateGrayscaleHistogram(Bitmap grayscaleImage)
        {
            int[] histogram = new int[256];
            for (int x = 0; x < grayscaleImage.Width; x++)
            {
                for (int y = 0; y < grayscaleImage.Height; y++)
                {
                    Color pixelColor = grayscaleImage.GetPixel(x, y);
                    int intensity = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);
                    histogram[intensity]++;
                }
            }
            return histogram;
        }

        // Indentificarea maximului local
        private static List<int> FindLocalMaxima(int[] histogram)
        {
            List<int> localMaxima = new List<int>();
            for (int i = 1; i < histogram.Length - 1; i++)
            {
                if (histogram[i] > histogram[i - 1] && histogram[i] > histogram[i + 1])
                    localMaxima.Add(i);
            }
            return localMaxima;
        }

        // Determinarea pragurilor multiple pentru o imagine color
        public static int[] FindMultipleThresholds(int[][] histograms)
        {
            int numChannels = histograms.Length;
            int[] thresholds = new int[numChannels];
            for (int channel = 0; channel < numChannels; channel++)
            {
                List<int> localMaxima = FindLocalMaxima(histograms[channel]);
                thresholds[channel] = localMaxima.Max();
            }
            return thresholds;
        }

        // Determinarea pragurilor multiple pentru o imagine gri
        public static int[] FindMultipleThresholdsForGrayscale(int[] histogram)
        {
            List<int> localMaxima = FindLocalMaxima(histogram);
            int[] thresholds = localMaxima.ToArray();
            return thresholds;
        }

        // Verifica la ce interval apartine valoarea de pixel
        private static int FindSegmentForGrayscale(int value, int[] thresholds)
        {
            for (int i = 0; i < thresholds.Length; i++)
                if (value <= thresholds[i])
                    return i;
            return thresholds.Length;
        }

        // Aplicarea pragurilor pentru imaginea gri
        public static Bitmap ApplyThresholdsForGrayscale(Bitmap originalImage, int[] thresholds)
        {
            Bitmap segmentedGrayImage = new Bitmap(originalImage.Width, originalImage.Height);
            for (int i = 0; i < originalImage.Width; i++)
            {
                for (int j = 0; j < originalImage.Height; j++)
                {
                    Color pixelColor = originalImage.GetPixel(i, j);
                    int grayValue = CalculateWeightedValue(pixelColor);
                    int segment = FindSegmentForGrayscale(grayValue, thresholds);
                    int newColor = (int)(255 / (thresholds.Length + 1) * (segment + 1));
                    segmentedGrayImage.SetPixel(i, j, Color.FromArgb(newColor, newColor, newColor));
                }
            }
            return segmentedGrayImage;
        }

        // Aplica difuzia erorii pentru un pixel specific în timpul algoritmului Floyd-Steinberg
        private static void DiffuseError(Bitmap image, int x, int y, int error, double factor)
        {
            Color currentColor = image.GetPixel(x, y);
            int newR = (int)Math.Max(0, Math.Min(255, currentColor.R + error * factor));
            int newG = (int)Math.Max(0, Math.Min(255, currentColor.G + error * factor));
            int newB = (int)Math.Max(0, Math.Min(255, currentColor.B + error * factor));
            Color newColor = Color.FromArgb(newR, newG, newB);
            image.SetPixel(x, y, newColor);
        }

        // Algoritmul Floyd-Steinberg pentru corectie asupra imaginii în tonuri de gri. Algoritmul reduce numărul de nivele de intensitate și difuzează eroarea către pixelii învecinați.
        public static Bitmap ApplyFloydSteinbergDithering(Bitmap originalImage)
        {
            Bitmap ditheredImage = new Bitmap(originalImage);
            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    Color oldColor = originalImage.GetPixel(x, y);
                    int oldGrayValue = CalculateWeightedValue(oldColor);
                    int newGrayValue = (oldGrayValue < 128) ? 0 : 255;
                    Color newColor = Color.FromArgb(newGrayValue, newGrayValue, newGrayValue);
                    ditheredImage.SetPixel(x, y, newColor);
                    int error = oldGrayValue - newGrayValue;
                    if (x + 1 < originalImage.Width)
                        DiffuseError(originalImage, x + 1, y, error, 7 / 16.0);
                    if (x - 1 >= 0 && y + 1 < originalImage.Height)
                        DiffuseError(originalImage, x - 1, y + 1, error, 3 / 16.0);
                    if (y + 1 < originalImage.Height)
                        DiffuseError(originalImage, x, y + 1, error, 5 / 16.0);
                    if (x + 1 < originalImage.Width && y + 1 < originalImage.Height)
                        DiffuseError(originalImage, x + 1, y + 1, error, 1 / 16.0);
                }
            }
            return ditheredImage;
        }

        /*********************Tema 4*********************/

        // Algoritmul Traversare in latime
        public static List<List<System.Drawing.Point>> BFS(Bitmap bitmap)
        {
            List<List<System.Drawing.Point>> objects = new List<List<System.Drawing.Point>>();
            bool[,] visited = new bool[bitmap.Width, bitmap.Height];
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);
                    int luminance = (int)(0.299 * pixelColor.R + 0.587 * pixelColor.G + 0.114 * pixelColor.B);
                    if (luminance < 128 && !visited[x, y])
                    {
                        List<System.Drawing.Point> obj = new List<System.Drawing.Point>();
                        Queue<System.Drawing.Point> queue = new Queue<System.Drawing.Point>();
                        queue.Enqueue(new System.Drawing.Point(x, y));
                        visited[x, y] = true;
                        while (queue.Count > 0)
                        {
                            System.Drawing.Point current = queue.Dequeue();
                            obj.Add(current);
                            int[] dx = { 1, -1, 0, 0 };
                            int[] dy = { 0, 0, 1, -1 };
                            for (int i = 0; i < 4; i++)
                            {
                                int newX = current.X + dx[i];
                                int newY = current.Y + dy[i];
                                if (newX >= 0 && newX < bitmap.Width && newY >= 0 && newY < bitmap.Height &&
                                    !visited[newX, newY])
                                {
                                    Color newPixelColor = bitmap.GetPixel(newX, newY);
                                    int newLuminance = (int)(0.299 * newPixelColor.R + 0.587 * newPixelColor.G + 0.114 * newPixelColor.B);
                                    if (newLuminance < 128)
                                    {
                                        queue.Enqueue(new System.Drawing.Point(newX, newY));
                                        visited[newX, newY] = true;
                                    }
                                }
                            }
                        }
                        objects.Add(obj);
                    }
                }
            }

            return objects;
        }

        // Algoritmul Rosenfeld
        public static Bitmap ApplyRosenfeld(Bitmap inputImage)
        {
            Bitmap outputImage = new Bitmap(inputImage.Width, inputImage.Height);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    Color pixelColor = inputImage.GetPixel(x, y);
                    int averageColor = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    if (averageColor < 128)
                        outputImage.SetPixel(x, y, Color.Black);
                    else
                        outputImage.SetPixel(x, y, Color.White);
                }
            }
            return outputImage;
        }

        /*********************Tema 5*********************/

        // Calculul ariilor obiectelor dintr-o imagine data ca matrice de puncte
        public static List<double> CalculateAreas(List<List<System.Drawing.Point>> objects)
        {
            List<double> areas = new List<double>();
            foreach (var obj in objects)
            {
                double area = obj.Count;
                areas.Add(area);
            }
            return areas;
        }

        // Calculul perimetrelor obiectelor dintr-o imagine data ca matrice de puncte
        public static List<double> CalculatePerimeters(List<List<System.Drawing.Point>> objects)
        {
            List<double> perimeters = new List<double>();
            foreach (var obj in objects)
            {
                double perimeter = 0;
                for (int i = 0; i < obj.Count; i++)
                {
                    System.Drawing.Point currentPoint = obj[i];
                    System.Drawing.Point nextPoint = obj[(i + 1) % obj.Count];
                    double distance = Math.Sqrt(Math.Pow(nextPoint.X - currentPoint.X, 2) + Math.Pow(nextPoint.Y - currentPoint.Y, 2));
                    perimeter += distance;
                }
                perimeters.Add(perimeter);
            }
            return perimeters;
        }

        // Calculul centrele de greutate obiectelor dintr-o imagine data ca matrice de puncte
        public static List<System.Drawing.Point> CalculateCentroids(List<List<System.Drawing.Point>> objects)
        {
            List<System.Drawing.Point> centroids = new List<System.Drawing.Point>();
            foreach (var obj in objects)
            {
                int sumX = 0;
                int sumY = 0;
                int totalPoints = obj.Count;
                foreach (var point in obj)
                {
                    sumX += point.X;
                    sumY += point.Y;
                }
                int centerX = sumX / totalPoints;
                int centerY = sumY / totalPoints;
                centroids.Add(new System.Drawing.Point(centerX, centerY));
            }
            return centroids;
        }

        /*********************Tema 6*********************/

        // Convertire a unui Bitmap la Mat
        private static Mat BitmapToMat(Bitmap bitmap)
        {
            var bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);
            var mat = new Mat(bitmap.Height, bitmap.Width, MatType.CV_8UC4, bmpData.Scan0);
            bitmap.UnlockBits(bmpData);
            return mat;
        }

        // Gasirea contururilor unei imagini
        public static List<List<System.Drawing.Point>> FindContours(Bitmap binaryImage)
        {
            Mat image = BitmapToMat(binaryImage);
            Cv2.CvtColor(image, image, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(image, image, 127, 255, ThresholdTypes.Binary);
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(image, out contours, out hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);
            List<List<System.Drawing.Point>> contoursList = new List<List<System.Drawing.Point>>();
            foreach (var contour in contours)
            {
                List<System.Drawing.Point> contourList = new List<System.Drawing.Point>();
                foreach (var point in contour)
                    contourList.Add(new System.Drawing.Point(point.X, point.Y));
                contoursList.Add(contourList);
            }
            return contoursList;
        }

        // Extragerea codurilor inlantuite pentru obiectele din imagine
        public static List<int> extractChainCode(List<List<System.Drawing.Point>> contours)
        {
            if (contours.Count == 0)
            {
                MessageBox.Show("Nu s-au putut găsi contururi.");
                return new List<int>();
            }
            var chainCode = new List<int>();
            foreach (var contour in contours)
            {
                for (int i = 0; i < contour.Count - 1; i++)
                {
                    var point1 = contour[i];
                    var point2 = contour[i + 1];
                    var deltaX = point2.X - point1.X;
                    var deltaY = point2.Y - point1.Y;
                    int direction = -1; 
                    if (deltaX == 0 && deltaY == -1)
                        direction = 0;
                    else if (deltaX == 1 && deltaY == -1)
                        direction = 1;
                    else if (deltaX == 1 && deltaY == 0)
                        direction = 2;
                    else if (deltaX == 1 && deltaY == 1)
                        direction = 3;
                    else if (deltaX == 0 && deltaY == 1)
                        direction = 4;
                    else if (deltaX == -1 && deltaY == 1)
                        direction = 5;
                    else if (deltaX == -1 && deltaY == 0)
                        direction = 6;
                    else if (deltaX == -1 && deltaY == -1)
                        direction = 7;
                    if (direction != -1)
                        chainCode.Add(direction);
                }
            }
            return chainCode;
        }

        /*********************Tema 7*********************/

        // Convertire a unui Mat la Bitmap
        private static Bitmap MatToBitmap(Mat mat)
        {
            try
            {
                if (mat == null || mat.Width == 0 || mat.Height == 0)
                    return null;
                Bitmap bitmap = null;
                if (mat.Depth() == MatType.CV_8U)
                {
                    if (mat.Channels() == 1)
                    {
                        bitmap = new Bitmap(mat.Width, mat.Height, mat.Width * mat.Channels(), PixelFormat.Format8bppIndexed, mat.Data);
                        ColorPalette pal = bitmap.Palette;
                        for (int i = 0; i < 256; i++)
                            pal.Entries[i] = Color.FromArgb(255, i, i, i);
                        bitmap.Palette = pal;
                    }
                    else if (mat.Channels() == 3)
                        bitmap = new Bitmap(mat.Width, mat.Height, mat.Width * mat.Channels(), PixelFormat.Format24bppRgb, mat.Data);
                    else if (mat.Channels() == 4)
                        bitmap = new Bitmap(mat.Width, mat.Height, mat.Width * mat.Channels(), PixelFormat.Format32bppArgb, mat.Data);
                }
                return bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error converting Mat to Bitmap: " + ex.Message);
                return null;
            }
        }

        // Convertire a unui Bitmap la OpenCvSharp.Mat
        private static Mat BitmapToMatOpenCV(Bitmap bitmap)
        {
            if (bitmap == null)
                return null;
            int width = bitmap.Width;
            int height = bitmap.Height;
            Mat mat = new Mat(height, width, MatType.CV_8UC3);
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            try
            {
                unsafe
                {
                    byte* bmpPtr = (byte*)bmpData.Scan0.ToPointer();
                    for (int y = 0; y < height; y++)
                    {
                        byte* matPtr = (byte*)mat.Ptr(y).ToPointer();
                        for (int x = 0; x < width; x++)
                        {
                            matPtr[3 * x + 0] = bmpPtr[3 * x + 0]; // Blue
                            matPtr[3 * x + 1] = bmpPtr[3 * x + 1]; // Green
                            matPtr[3 * x + 2] = bmpPtr[3 * x + 2]; // Red
                        }
                        bmpPtr += bmpData.Stride;
                    }
                }
            }
            finally
            {
                bitmap.UnlockBits(bmpData);
            }
            return mat;
        }

        // Dilatarea unei imagini binare
        public static Bitmap DilateBinaryImage(Bitmap binary, int kernelSize = 3)
        {
            Mat binaryImage = BitmapToMatOpenCV(binary);
            Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(kernelSize, kernelSize));
            Mat dilatedImage = new Mat();
            Cv2.Dilate(binaryImage, dilatedImage, kernel);

            return MatToBitmap(dilatedImage);
        }

        // Erodarea unei imagini binare
        public static Bitmap ErodeBinaryImage(Bitmap binary, int kernelSize = 3)
        {
            Mat binaryImage = BitmapToMat(binary);
            Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(kernelSize, kernelSize));
            Mat erodedImage = new Mat();
            Cv2.Erode(binaryImage, erodedImage, kernel);
            Bitmap resultBitmap = MatToBitmap(erodedImage);
            return resultBitmap;
        }

        /*********************Tema 8*********************/

        // Calculeaza probabilitatea pentru fiecare nivel de intensitate grayscale in functie de histograma imaginii
        private static double[] CalculateProbabilities(int[] histogram, int totalPixels)
        {
            double[] probabilities = new double[256];
            for (int i = 0; i < 256; i++)
                probabilities[i] = (double)histogram[i] / totalPixels;
            return probabilities;
        }

        // Calculeaza probabilitatile cumulative pentru fiecare nivel de intensitate
        private static double[] CalculateCumulativeProbabilities(double[] probabilities)
        {
            double[] cumulativeProbabilities = new double[256];
            cumulativeProbabilities[0] = probabilities[0];
            for (int i = 1; i < 256; i++)
                cumulativeProbabilities[i] = cumulativeProbabilities[i - 1] + probabilities[i];
            return cumulativeProbabilities;
        }

        // Calculeaza media globala a intensitatilor grayscale.
        private static double CalculateGlobalMean(double[] probabilities)
        {
            double globalMean = 0;
            for (int i = 0; i < 256; i++)
                globalMean += i * probabilities[i];
            return globalMean;
        }

        // Gaseste pragul optim de binarizare folosind metoda Otsu. Pragul optim este acel nivel de intensitate care maximizează variatia interclaselor intre clasele de pixeli (obiect si fundal).
        private static int FindOptimalThreshold(double[] cumulativeProbabilities, double[] probabilities, double globalMean)
        {
            double maxVariance = 0;
            int optimalThreshold = 0;
            for (int t = 0; t < 256; t++)
            {
                double backgroundProbability = cumulativeProbabilities[t];
                double foregroundProbability = 1 - backgroundProbability;
                if (backgroundProbability == 0 || foregroundProbability == 0)
                    continue;
                double backgroundMean = 0;
                for (int i = 0; i <= t; i++)
                    backgroundMean += i * probabilities[i] / backgroundProbability;
                double foregroundMean = (globalMean - backgroundProbability * backgroundMean) / foregroundProbability;
                double betweenClassVariance = backgroundProbability * foregroundProbability * Math.Pow(backgroundMean - foregroundMean, 2);
                if (betweenClassVariance > maxVariance)
                {
                    maxVariance = betweenClassVariance;
                    optimalThreshold = t;
                }
            }
            return optimalThreshold;
        }

        // Efectueaza binarizarea automata a unei imagini grayscale
        public static Bitmap BinarizeAutomatically(Bitmap grayscaleImage)
        {
            int[] histogram = GenerateGrayscaleHistogram(grayscaleImage);
            double[] probabilities = CalculateProbabilities(histogram, grayscaleImage.Width * grayscaleImage.Height);
            double[] cumulativeProbabilities = CalculateCumulativeProbabilities(probabilities);
            double globalMean = CalculateGlobalMean(probabilities);
            int optimalThreshold = FindOptimalThreshold(cumulativeProbabilities, probabilities, globalMean);
            Bitmap binaryImage = new Bitmap(grayscaleImage.Width, grayscaleImage.Height);
            for (int x = 0; x < grayscaleImage.Width; x++)
            {
                for (int y = 0; y < grayscaleImage.Height; y++)
                {
                    Color pixelColor = grayscaleImage.GetPixel(x, y);
                    int intensity = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);
                    Color binaryColor = intensity < optimalThreshold ? Color.Black : Color.White;
                    binaryImage.SetPixel(x, y, binaryColor);
                }
            }
            return binaryImage;
        }

        // Negativarea imaginii
        public static Bitmap InvertImage(Bitmap originalImage)
        {
            Bitmap invertedImage = new Bitmap(originalImage.Width, originalImage.Height);
            for (int x = 0; x < originalImage.Width; x++)
            {
                for (int y = 0; y < originalImage.Height; y++)
                {
                    Color originalColor = originalImage.GetPixel(x, y);
                    Color invertedColor = Color.FromArgb(
                        255 - originalColor.R,   // Inversarea componentei roșii
                        255 - originalColor.G,   // Inversarea componentei verzi
                        255 - originalColor.B    // Inversarea componentei albastre
                    );
                    invertedImage.SetPixel(x, y, invertedColor);
                }
            }
            return invertedImage;
        }

        // Schimbarea contrastului
        public static Bitmap AdjustContrast(Bitmap originalImage, float contrastLevel = 2.5f)
        {
            Bitmap adjustedImage = new Bitmap(originalImage.Width, originalImage.Height);
            for (int x = 0; x < originalImage.Width; x++)
            {
                for (int y = 0; y < originalImage.Height; y++)
                {
                    Color originalPixel = originalImage.GetPixel(x, y);
                    int grayIntensity = (int)(originalPixel.R * 0.3 + originalPixel.G * 0.59 + originalPixel.B * 0.11);
                    int adjustedIntensity = (int)(contrastLevel * (grayIntensity - 128) + 128);
                    adjustedIntensity = Math.Max(0, Math.Min(255, adjustedIntensity));
                    Color adjustedPixel = Color.FromArgb(originalPixel.A, adjustedIntensity, adjustedIntensity, adjustedIntensity);
                    adjustedImage.SetPixel(x, y, adjustedPixel);
                }
            }
            return adjustedImage;
        }

        // Aplicarea corectiei gamma
        public static Bitmap ApplyGammaCorrection(Bitmap originalImage, double gamma = 1.5f)
        {
            Bitmap adjustedImage = (Bitmap)originalImage.Clone();
            for (int x = 0; x < adjustedImage.Width; x++)
            {
                for (int y = 0; y < adjustedImage.Height; y++)
                {
                    Color originalColor = adjustedImage.GetPixel(x, y);
                    double red = originalColor.R / 255.0;
                    double green = originalColor.G / 255.0;
                    double blue = originalColor.B / 255.0;
                    double correctedRed = Math.Pow(red, 1.0 / gamma);
                    double correctedGreen = Math.Pow(green, 1.0 / gamma);
                    double correctedBlue = Math.Pow(blue, 1.0 / gamma);
                    correctedRed = Math.Max(0.0, Math.Min(1.0, correctedRed));
                    correctedGreen = Math.Max(0.0, Math.Min(1.0, correctedGreen));
                    correctedBlue = Math.Max(0.0, Math.Min(1.0, correctedBlue));
                    Color correctedColor = Color.FromArgb(
                        (int)(correctedRed * 255),
                        (int)(correctedGreen * 255),
                        (int)(correctedBlue * 255)
                    );
                    adjustedImage.SetPixel(x, y, correctedColor);
                }
            }
            return adjustedImage;
        }

        // Schimbarea luminozitatii
        public static Bitmap AdjustBrightness(Bitmap originalImage, int brightnessLevel = 70)
        {
            Bitmap adjustedImage = new Bitmap(originalImage.Width, originalImage.Height);
            for (int x = 0; x < originalImage.Width; x++)
            {
                for (int y = 0; y < originalImage.Height; y++)
                {
                    Color originalColor = originalImage.GetPixel(x, y);
                    int newRed = originalColor.R + brightnessLevel;
                    int newGreen = originalColor.G + brightnessLevel;
                    int newBlue = originalColor.B + brightnessLevel;
                    newRed = Math.Max(0, Math.Min(255, newRed));
                    newGreen = Math.Max(0, Math.Min(255, newGreen));
                    newBlue = Math.Max(0, Math.Min(255, newBlue));
                    Color adjustedColor = Color.FromArgb(newRed, newGreen, newBlue);
                    adjustedImage.SetPixel(x, y, adjustedColor);
                }
            }
            return adjustedImage;
        }

        // Algoritmul de egalizare al histogramei
        public static Bitmap EqualizeHistogram(Bitmap grayscaleImage)
        {           
            int[] histogram = Effects.GenerateGrayscaleHistogram(grayscaleImage);
            double[] probabilities = Effects.CalculateProbabilities(histogram, grayscaleImage.Width * grayscaleImage.Height);
            double[] cumulativeProbabilities = Effects.CalculateCumulativeProbabilities(probabilities);
            int[] transformationMap = new int[256];
            for (int i = 0; i < 256; i++)
                transformationMap[i] = (int)Math.Round(255 * cumulativeProbabilities[i]);
            Bitmap equalizedImage = new Bitmap(grayscaleImage.Width, grayscaleImage.Height);
            for (int x = 0; x < grayscaleImage.Width; x++)
            {
                for (int y = 0; y < grayscaleImage.Height; y++)
                {
                    Color pixelColor = grayscaleImage.GetPixel(x, y);
                    int intensity = pixelColor.R;
                    int newIntensity = transformationMap[intensity];
                    Color newPixelColor = Color.FromArgb(newIntensity, newIntensity, newIntensity);
                    equalizedImage.SetPixel(x, y, newPixelColor);
                }
            }
            return equalizedImage;
        }

        /*********************Tema 9*********************/

        // Filtru "trece jos" (de netezire a imaginilor, de eliminare a zgomotelor) in domeniul spatial
        public static Bitmap SmoothingImageSpace(Bitmap bitmap)
        {
            Bitmap smoothedBitmap = (Bitmap)bitmap.Clone();
            int width = bitmap.Width;
            int height = bitmap.Height;
            int filterSize = 3;
            int margin = filterSize / 2;
            for (int y = margin; y < height - margin; y++)
            {
                for (int x = margin; x < width - margin; x++)
                {
                    int sumR = 0, sumG = 0, sumB = 0;
                    for (int j = -margin; j <= margin; j++)
                    {
                        for (int i = -margin; i <= margin; i++)
                        {
                            Color pixel = bitmap.GetPixel(x + i, y + j);
                            sumR += pixel.R;
                            sumG += pixel.G;
                            sumB += pixel.B;
                        }
                    }
                    int avgR = sumR / (filterSize * filterSize);
                    int avgG = sumG / (filterSize * filterSize);
                    int avgB = sumB / (filterSize * filterSize);
                    Color smoothedColor = Color.FromArgb(avgR, avgG, avgB);
                    smoothedBitmap.SetPixel(x, y, smoothedColor);
                }
            }
            return smoothedBitmap;
        }

        // Filtru "trece sus" (de evidenţiere a muchiilor) in domeniul spatial
        public static Bitmap DetectEdgesSpace(Bitmap bitmap)
        {
            Bitmap edgeBitmap = (Bitmap)bitmap.Clone();
            int width = bitmap.Width;
            int height = bitmap.Height;
            int[,] kernelX = { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            int[,] kernelY = { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };
            int margin = 1;
            for (int y = margin; y < height - margin; y++)
            {
                for (int x = margin; x < width - margin; x++)
                {
                    int gradientX = 0, gradientY = 0;
                    for (int j = -margin; j <= margin; j++)
                    {
                        for (int i = -margin; i <= margin; i++)
                        {
                            Color pixel = bitmap.GetPixel(x + i, y + j);
                            int grayValue = (int)(pixel.R * 0.3 + pixel.G * 0.59 + pixel.B * 0.11);

                            gradientX += kernelX[j + margin, i + margin] * grayValue;
                            gradientY += kernelY[j + margin, i + margin] * grayValue;
                        }
                    }
                    int magnitude = (int)Math.Sqrt(gradientX * gradientX + gradientY * gradientY);
                    magnitude = Math.Min(255, Math.Max(0, magnitude));
                    Color edgeColor = Color.FromArgb(magnitude, magnitude, magnitude);
                    edgeBitmap.SetPixel(x, y, edgeColor);
                }
            }
            return edgeBitmap;
        }

        /*********************Tema 10*********************/

        // Funcție pentru redimensionarea unei imagini la dimensiuni specifice
        private static Bitmap ResizeBitmap(Bitmap bitmap, int width, int height)
        {
            Bitmap resizedBitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(resizedBitmap))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bitmap, 0, 0, width, height);
            }
            return resizedBitmap;
        }

        // Funcție pentru a rotunji un număr la cea mai apropiată putere a lui 2
        private static int RoundUpToPowerOfTwo(int x)
        {
            int result = 1;
            while (result < x)
                result *= 2;
            return result;
        }

        // Filtru ideal de tip "trece jos" in domeniulul frecvential
        public static Bitmap SmoothingImageFreq(Bitmap bitmap)
        {
            int newWidth = RoundUpToPowerOfTwo(bitmap.Width);
            int newHeight = RoundUpToPowerOfTwo(bitmap.Height);
            Bitmap resizedBitmap = ResizeBitmap(bitmap, newWidth, newHeight);
            Bitmap filteredBitmap = (Bitmap)resizedBitmap.Clone();
            Grayscale filterGray = new Grayscale(0.2125, 0.7154, 0.0721);
            filteredBitmap = filterGray.Apply(filteredBitmap);
            ComplexImage complexImage = ComplexImage.FromBitmap(filteredBitmap);
            complexImage.ForwardFourierTransform();
            int radius = 50;
            int width = complexImage.Width;
            int height = complexImage.Height;
            int centerX = width / 2;
            int centerY = height / 2;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double distance = Math.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
                    if (distance > radius)
                        complexImage.Data[y, x] = new Complex(0, 0);
                }
            }
            complexImage.BackwardFourierTransform();
            filteredBitmap = complexImage.ToBitmap();
            filteredBitmap = ResizeBitmap(filteredBitmap, bitmap.Width, bitmap.Height);
            return filteredBitmap;
        }

        /*********************Tema 11*********************/

        // Restaurarea unei imagini folosind un algo cu un nucleu gaussian
        public static (Bitmap, double) RestorationGauss(Bitmap bitmap)
        {
            DateTime startTime = DateTime.Now;
            Bitmap filteredBitmap = (Bitmap)bitmap.Clone();
            GaussianBlur filterGaussian = new GaussianBlur(5, 3);
            filteredBitmap = filterGaussian.Apply(filteredBitmap);
            TimeSpan elapsedTime = DateTime.Now - startTime;
            return (filteredBitmap, elapsedTime.TotalMilliseconds);
        }

        // Restaurarea unei imagini folosing un alog cu un nucleu bidimensional
        public static (Bitmap, double) RestorationBi(Bitmap bitmap)
        {
            DateTime startTime = DateTime.Now;
            Bitmap filteredBitmap = (Bitmap)bitmap.Clone();
            int[,] kernel = {
                { 1, 2, 1 },
                { 2, 4, 2 },
                { 1, 2, 1 }
            };
            Convolution filterConvolution = new Convolution(kernel);
            filteredBitmap = filterConvolution.Apply(filteredBitmap);
            TimeSpan elapsedTime = DateTime.Now - startTime;
            return (filteredBitmap, elapsedTime.TotalMilliseconds); ;
        }

        /*********************Tema 12*********************/

        // Binarizare adaptiva a punctelor de muchie
        public static (Bitmap,Mat) AdaptiveEdgeThresholding(Bitmap inputImage)
        {
            Mat inputMat = BitmapToMat(inputImage);
            Mat grayMat = new Mat();
            Cv2.CvtColor(inputMat, grayMat, ColorConversionCodes.BGR2GRAY);
            Mat edgeMat = new Mat();
            Cv2.Canny(grayMat, edgeMat, 100, 200);
            Mat adaptiveThresholdMat = new Mat();
            Cv2.AdaptiveThreshold(edgeMat, adaptiveThresholdMat, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, 9, 2);
            return (MatToBitmap(adaptiveThresholdMat), adaptiveThresholdMat);
        }

        // Prelungire a muchiilor prin histereza
        public static Bitmap EdgeExtensionThresholding(Mat inputImage)
        {
            // Aplică histereză direct pe imaginea de intrare binarizată adaptiv
            Mat hysteresisMat = new Mat();
            Cv2.Dilate(inputImage, hysteresisMat, new Mat(), null, 1);
            Cv2.Erode(hysteresisMat, hysteresisMat, new Mat(), null, 1);
            return MatToBitmap(hysteresisMat);
        }
    }
}