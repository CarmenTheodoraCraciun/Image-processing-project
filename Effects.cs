using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

using OpenCvSharp;

using Point = System.Drawing.Point;
using System.Drawing.Imaging;
using Emgu.CV.Reg;


namespace PI_Project
{
    public static class Effects
    {
        // Creaza o imagine pornind de la o matrice de puncte
        public static Bitmap DrawImage(List<List<System.Drawing.Point>> pointsLists)
        {
            // Determina dimensiunile imaginii in functie de dimensiunile punctelor
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

            // Creeaza o imagine cu dimensiunile corespunzatoare
            Bitmap graphBitmap = new Bitmap(maxX + 1, maxY + 1);
            using (Graphics g = Graphics.FromImage(graphBitmap))
            {
                g.Clear(Color.White);

                // Deseneaza graficul pe imaginea noua
                foreach (List<Point> points in pointsLists)
                {
                    foreach (Point point in points)
                    {
                        // Seteaza simbolul in functe de existenta punctului
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
            // Parcurgem pixel cu pixel imaginea
            for (int i = 0; i < originalImage.Width; i++)
            {
                for (int j = 0; j < originalImage.Height; j++)
                {
                    // Luam culoare de pe pozitia ij
                    Color pixelColor = originalImage.GetPixel(i, j);
                    int grayValue = CalculateWeightedValue(pixelColor);
                    // Cream culoare pe scara gri
                    Color grayColor = Color.FromArgb(grayValue, grayValue, grayValue);
                    // Setam pixelul pe pozitia i, j a noi imagini
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
                default: // case 5:
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
                    // Converteste RGB la HSV
                    float h, s, v;
                    RGBToHSV(rgbColor.R, rgbColor.G, rgbColor.B, out h, out s, out v);
                    // Converteste HSV înapoi la RGB pentru afiaare.
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
                    // Aplica pragul
                    Color binaryColor = (grayValue < threshold) ? Color.Black : Color.White;
                    binaryImage.SetPixel(i, j, binaryColor);
                }
            }
            return binaryImage;
        }

        /*********************Tema 3*********************/

        // Generarea histogramei color
        // Histograma este organizata pe trei canale de culoare: rosu, verde si albastru.
        public static int[][] GenerateColorHistogram(Bitmap image)
        {
            int[][] histograms = new int[3][];
            histograms[0] = new int[256]; // Red
            histograms[1] = new int[256]; // Green
            histograms[2] = new int[256]; // Blue

            // Parcurgerea pixel cu pixel a imaginii
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    // Obtinerea culorii pixelului la poziția (i, j)
                    Color pixelColor = image.GetPixel(i, j);

                    // Incrementarea corespunzatoare a frecventei culorii în histogramele separate
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
            // Initializare vector pentru histograma grayscale cu 256 de nivele de intensitate
            int[] histogram = new int[256];

            // Parcurgerea pixel cu pixel a imaginii grayscale
            for (int x = 0; x < grayscaleImage.Width; x++)
            {
                for (int y = 0; y < grayscaleImage.Height; y++)
                {
                    // Obtinerea culorii pixelului la poziția (x, y)
                    Color pixelColor = grayscaleImage.GetPixel(x, y);

                    // Calcularea intensitatii grayscale utilizand formula ponderata
                    int intensity = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);

                    // Incrementarea corespunzatoare a frecventei nivelului de intensitate in histograma
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
                {
                    localMaxima.Add(i);
                }
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
                // Identifica varfurile locale in histograma canalului de culoare
                List<int> localMaxima = FindLocalMaxima(histograms[channel]);

                // Alege pragul drept varful maxim local
                thresholds[channel] = localMaxima.Max();
            }

            return thresholds;
        }

        // Determinarea pragurilor multiple pentru o imagine gri
        public static int[] FindMultipleThresholdsForGrayscale(int[] histogram)
        {
            // Identifica varfurile locale in histograma de intensitati
            List<int> localMaxima = FindLocalMaxima(histogram);
            // Alegerea maximului
            int[] thresholds = localMaxima.ToArray();

            return thresholds;
        }

        // Verifica la ce interval apartine valoarea de pixel
        private static int FindSegmentForGrayscale(int value, int[] thresholds)
        {
            for (int i = 0; i < thresholds.Length; i++)
            {
                if (value <= thresholds[i])
                {
                    return i;
                }
            }

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

                    // Verifica la ce interval apartine valoarea de pixel
                    int segment = FindSegmentForGrayscale(grayValue, thresholds);

                    // Seteaza culoarea pixelului in functie de segment
                    int newColor = (int)(255 / (thresholds.Length + 1) * (segment + 1));
                    segmentedGrayImage.SetPixel(i, j, Color.FromArgb(newColor, newColor, newColor));
                }
            }

            return segmentedGrayImage;
        }

        // Aplica difuzia erorii pentru un pixel specific în timpul algoritmului Floyd-Steinberg
        private static void DiffuseError(Bitmap image, int x, int y, int error, double factor)
        {
            // Obtinerea culorii curente a pixelului la poziția (x, y)
            Color currentColor = image.GetPixel(x, y);

            // Calcularea noilor componente de culoare cu aplicarea difuziei erorii
            int newR = (int)Math.Max(0, Math.Min(255, currentColor.R + error * factor));
            int newG = (int)Math.Max(0, Math.Min(255, currentColor.G + error * factor));
            int newB = (int)Math.Max(0, Math.Min(255, currentColor.B + error * factor));

            // Crearea unei noi culori cu componentele calculate și actualizarea pixelului în imagine
            Color newColor = Color.FromArgb(newR, newG, newB);
            image.SetPixel(x, y, newColor);
        }

        // Algoritmul Floyd-Steinberg pentru corectie asupra imaginii în tonuri de gri
        // Algoritmul reduce numărul de nivele de intensitate și difuzează eroarea către pixelii învecinați.
        public static Bitmap ApplyFloydSteinbergDithering(Bitmap originalImage)
        {
            // Initializarea unei noi imagini pentru rezultatul algoritmului
            Bitmap ditheredImage = new Bitmap(originalImage);

            // Parcurgerea pixel cu pixel a imaginii originale
            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    // Obtinerea culorii pixelului la poziția (x, y) în imaginea originala
                    Color oldColor = originalImage.GetPixel(x, y);

                    // Calcularea valorii grayscale folosind formula ponderata
                    int oldGrayValue = CalculateWeightedValue(oldColor);

                    // Alegerea noii valori de pixel in functie de prag (0 sau 255)
                    int newGrayValue = (oldGrayValue < 128) ? 0 : 255;

                    // Crearea unei noi culori grayscale si actualizarea pixelului in imaginea rezultata
                    Color newColor = Color.FromArgb(newGrayValue, newGrayValue, newGrayValue);
                    ditheredImage.SetPixel(x, y, newColor);

                    // Calcularea erorii dintre vechea și noua valoare de pixel
                    int error = oldGrayValue - newGrayValue;

                    // Aplicarea difuziei erorii la pixelii învecinați conform coeficienților Floyd-Steinberg
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
        public static List<List<Point>> BFS(Bitmap bitmap)
        {
            List<List<Point>> objects = new List<List<Point>>();
            bool[,] visited = new bool[bitmap.Width, bitmap.Height];

            // Iterăm prin fiecare pixel în imagine
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    // Obținem valoarea de luminanță a pixelului curent
                    Color pixelColor = bitmap.GetPixel(x, y);
                    int luminance = (int)(0.299 * pixelColor.R + 0.587 * pixelColor.G + 0.114 * pixelColor.B);

                    // Verificăm dacă pixelul este negru și nevizitat
                    if (luminance < 128 && !visited[x, y])
                    {
                        // Inițializăm o nouă listă pentru a stoca punctele obiectului curent
                        List<Point> obj = new List<Point>();

                        // Inițializăm o coadă pentru a efectua traversarea în lățime
                        Queue<Point> queue = new Queue<Point>();

                        // Adăugăm punctul inițial în coadă și îl marcam ca vizitat
                        queue.Enqueue(new Point(x, y));
                        visited[x, y] = true;

                        // Începem traversarea în lățime
                        while (queue.Count > 0)
                        {
                            Point current = queue.Dequeue();

                            // Adăugăm punctul curent la obiectul curent
                            obj.Add(current);

                            // Definim direcțiile posibile de deplasare
                            int[] dx = { 1, -1, 0, 0 };
                            int[] dy = { 0, 0, 1, -1 };

                            // Parcurgem direcțiile posibile
                            for (int i = 0; i < 4; i++)
                            {
                                int newX = current.X + dx[i];
                                int newY = current.Y + dy[i];

                                // Verificăm dacă noua poziție este validă și nevizitată
                                if (newX >= 0 && newX < bitmap.Width && newY >= 0 && newY < bitmap.Height &&
                                    !visited[newX, newY])
                                {
                                    Color newPixelColor = bitmap.GetPixel(newX, newY);
                                    int newLuminance = (int)(0.299 * newPixelColor.R + 0.587 * newPixelColor.G + 0.114 * newPixelColor.B);

                                    // Verificăm dacă noua poziție este un pixel negru
                                    if (newLuminance < 128)
                                    {
                                        // Adăugăm noua poziție în coadă și o marcam ca vizitată
                                        queue.Enqueue(new Point(newX, newY));
                                        visited[newX, newY] = true;
                                    }
                                }
                            }
                        }

                        // Adăugăm obiectul curent la lista de obiecte
                        objects.Add(obj);
                    }
                }
            }

            return objects;
        }

        // Algoritmul Rosenfeld
        public static Bitmap ApplyRosenfeld(Bitmap inputImage)
        {
            // Creează o copie a imaginii de intrare pentru a nu modifica imaginea originală
            Bitmap outputImage = new Bitmap(inputImage.Width, inputImage.Height);

            // Parcurge fiecare pixel al imaginii de intrare
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    // Obține culoarea pixelului
                    Color pixelColor = inputImage.GetPixel(x, y);

                    // Calculează valoarea medie a componentelor de culoare pentru a determina
                    // dacă pixelul este negru sau alb
                    int averageColor = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;

                    // Setează culoarea corespunzătoare în imaginea rezultat
                    if (averageColor < 128)
                    {
                        // Pixel negru
                        outputImage.SetPixel(x, y, Color.Black);
                    }
                    else
                    {
                        // Pixel alb
                        outputImage.SetPixel(x, y, Color.White);
                    }
                }
            }

            return outputImage;
        }

        /*********************Tema 5*********************/

        // Calculul ariilor obiectelor dintr-o imagine data ca matrice de puncte
        public static List<double> CalculateAreas(List<List<Point>> objects)
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
        public static List<double> CalculatePerimeters(List<List<Point>> objects)
        {
            List<double> perimeters = new List<double>();

            foreach (var obj in objects)
            {
                double perimeter = 0;

                for (int i = 0; i < obj.Count; i++)
                {
                    Point currentPoint = obj[i];
                    Point nextPoint = obj[(i + 1) % obj.Count];

                    double distance = Math.Sqrt(Math.Pow(nextPoint.X - currentPoint.X, 2) + Math.Pow(nextPoint.Y - currentPoint.Y, 2));

                    perimeter += distance;
                }

                perimeters.Add(perimeter);
            }

            return perimeters;
        }

        // Calculul centrele de greutate obiectelor dintr-o imagine data ca matrice de puncte
        public static List<Point> CalculateCentroids(List<List<Point>> objects)
        {
            List<Point> centroids = new List<Point>();

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

                centroids.Add(new Point(centerX, centerY));
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
        public static List<List<Point>> FindContours(Bitmap binaryImage)
        {
            // Coversie Bitmat la OpenCV.Mat
            Mat image = BitmapToMat(binaryImage);
            Cv2.CvtColor(image, image, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(image, image, 127, 255, ThresholdTypes.Binary);

            // Gasi contururile
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(image, out contours, out hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);

            // Convertim contururile la matrice de pointeri
            List<List<System.Drawing.Point>> contoursList = new List<List<System.Drawing.Point>>();
            foreach (var contour in contours)
            {
                List<System.Drawing.Point> contourList = new List<System.Drawing.Point>();
                foreach (var point in contour)
                {
                    contourList.Add(new System.Drawing.Point(point.X, point.Y));
                }
                contoursList.Add(contourList);
            }

            return contoursList;
        }

        // Extragerea codurilor inlantuite pentru obiectele din imagine
        public static List<int> extractChainCode(List<List<Point>> contours)
        {
            // Verificare dacă s-au găsit contururi
            if (contours.Count == 0)
            {
                Console.WriteLine("Nu s-au putut găsi contururi.");
                return new List<int>(); // Returnează o listă goală în caz de eșec
            }

            // Inițializarea lanțului de coduri
            var chainCode = new List<int>();

            // Parcurgerea fiecărui contur și extragerea lanțului de coduri
            foreach (var contour in contours)
            {
                for (int i = 0; i < contour.Count - 1; i++)
                {
                    var point1 = contour[i];
                    var point2 = contour[i + 1];
                    var deltaX = point2.X - point1.X;
                    var deltaY = point2.Y - point1.Y;

                    // Calculul direcției între punctele consecutive
                    int direction = -1; // Valoare invalidă

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

                    // Verificare pentru a asigura că direction a fost atribuită înainte de adăugare la lanțul de coduri
                    if (direction != -1)
                    {
                        // Adăugarea direcției la lanțul de coduri
                        chainCode.Add(direction);
                    }
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
                // Verificați dacă Mat este valid
                if (mat == null || mat.Width == 0 || mat.Height == 0)
                    return null;

                // Verificați tipul de date al Mat și construiți bitmap-ul corespunzător
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
                    {
                        bitmap = new Bitmap(mat.Width, mat.Height, mat.Width * mat.Channels(), PixelFormat.Format24bppRgb, mat.Data);
                    }
                    else if (mat.Channels() == 4)
                    {
                        bitmap = new Bitmap(mat.Width, mat.Height, mat.Width * mat.Channels(), PixelFormat.Format32bppArgb, mat.Data);
                    }
                }

                return bitmap;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error converting Mat to Bitmap: " + ex.Message);
                return null;
            }
        }

        // Convertire a unui Bitmap la OpenCvSharp.Mat
        private static Mat BitmapToMatOpenCV(Bitmap bitmap)
        {
            // Verificăm dacă bitmapul este valid
            if (bitmap == null)
                return null;

            // Obținem dimensiunile bitmapului
            int width = bitmap.Width;
            int height = bitmap.Height;

            // Convertim bitmapul într-un obiect de tip Mat
            Mat mat = new Mat(height, width, MatType.CV_8UC3);
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            try
            {
                // Copiem datele din Bitmap în Mat
                unsafe
                {
                    byte* bmpPtr = (byte*)bmpData.Scan0.ToPointer();
                    for (int y = 0; y < height; y++)
                    {
                        byte* matPtr = (byte*)mat.Ptr(y).ToPointer();
                        for (int x = 0; x < width; x++)
                        {
                            // Copiem valorile de la Bitmap la Mat (în ordinea BGR)
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
                // Eliberăm resursele de blocare
                bitmap.UnlockBits(bmpData);
            }

            return mat;
        }

        // Dilatarea unei imagini binare
        public static Bitmap DilateBinaryImage(Bitmap binary, int kernelSize = 3)
        {
            Mat binaryImage = BitmapToMatOpenCV(binary);
            // Definirea kernelului pentru operația de dilatare
            Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(kernelSize, kernelSize));

            // Aplicarea operației de dilatare pe imaginea binară
            Mat dilatedImage = new Mat();
            Cv2.Dilate(binaryImage, dilatedImage, kernel);

            return MatToBitmap(dilatedImage);
        }

        // Erodarea unei imagini binare
        public static Bitmap ErodeBinaryImage(Bitmap binary, int kernelSize = 3)
        {
            // Convertiți imaginea binară într-un obiect Mat
            Mat binaryImage = BitmapToMat(binary);

            // Definiți kernelul pentru operația de eroziune
            Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(kernelSize, kernelSize));

            // Aplicați operația de eroziune pe imaginea binară
            Mat erodedImage = new Mat();
            Cv2.Erode(binaryImage, erodedImage, kernel);

            // Convertiți imaginea rezultată înapoi într-un obiect Bitmap
            Bitmap resultBitmap = MatToBitmap(erodedImage);

            return resultBitmap;
        }

        /*********************Tema 8*********************/

        // Calculeaza probabilitatea pentru fiecare nivel de intensitate grayscale in functie de histograma imaginii
        private static double[] CalculateProbabilities(int[] histogram, int totalPixels)
        {
            double[] probabilities = new double[256];
            for (int i = 0; i < 256; i++)
            {
                probabilities[i] = (double)histogram[i] / totalPixels;
            }
            return probabilities;
        }

        // Calculeaza probabilitatile cumulative pentru fiecare nivel de intensitate
        // Probabilitatea cumulativa pentru un anumit nivel de intensitate este suma probabilitatilor tuturor nivelurilor de intensitate de la 0 la acel nivel.
        private static double[] CalculateCumulativeProbabilities(double[] probabilities)
        {
            double[] cumulativeProbabilities = new double[256];
            cumulativeProbabilities[0] = probabilities[0];
            for (int i = 1; i < 256; i++)
            {
                cumulativeProbabilities[i] = cumulativeProbabilities[i - 1] + probabilities[i];
            }
            return cumulativeProbabilities;
        }

        // Calculeaza media globala a intensitatilor grayscale.
        private static double CalculateGlobalMean(double[] probabilities)
        {
            double globalMean = 0;
            for (int i = 0; i < 256; i++)
            {
                globalMean += i * probabilities[i];
            }
            return globalMean;
        }

        // Gaseste pragul optim de binarizare folosind metoda Otsu
        // Pragul optim este acel nivel de intensitate care maximizează variatia interclaselor intre clasele de pixeli (obiect si fundal).
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
                {
                    backgroundMean += i * probabilities[i] / backgroundProbability;
                }

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

            // Aplica binarizarea folosind pragul optim gasit
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
            // Creează o nouă imagine cu aceleași dimensiuni ca imaginea originală
            Bitmap invertedImage = new Bitmap(originalImage.Width, originalImage.Height);

            // Parcurge fiecare pixel al imaginii originale
            for (int x = 0; x < originalImage.Width; x++)
            {
                for (int y = 0; y < originalImage.Height; y++)
                {
                    // Obține culoarea pixelului la poziția (x, y)
                    Color originalColor = originalImage.GetPixel(x, y);

                    // Inversează culorile pixelului
                    Color invertedColor = Color.FromArgb(
                        255 - originalColor.R,   // Inversarea componentei roșii
                        255 - originalColor.G,   // Inversarea componentei verzi
                        255 - originalColor.B    // Inversarea componentei albastre
                    );

                    // Setează culoarea pixelului în imaginea inversată
                    invertedImage.SetPixel(x, y, invertedColor);
                }
            }

            return invertedImage;
        }

        // Schimbarea contrastului
        public static Bitmap AdjustContrast(Bitmap originalImage, float contrastLevel = 2.5f)
        {
            Bitmap adjustedImage = new Bitmap(originalImage.Width, originalImage.Height);

            // Iterăm prin fiecare pixel al imaginii
            for (int x = 0; x < originalImage.Width; x++)
            {
                for (int y = 0; y < originalImage.Height; y++)
                {
                    // Obținem pixelul original
                    Color originalPixel = originalImage.GetPixel(x, y);

                    // Convertim pixelul la intensitatea grii (valoare de la 0 la 255)
                    int grayIntensity = (int)(originalPixel.R * 0.3 + originalPixel.G * 0.59 + originalPixel.B * 0.11);

                    // Aplicăm tranziția de contrast la intensitatea grii
                    int adjustedIntensity = (int)(contrastLevel * (grayIntensity - 128) + 128);

                    // Asigurăm că intensitatea rămâne în intervalul valid [0, 255]
                    adjustedIntensity = Math.Max(0, Math.Min(255, adjustedIntensity));

                    // Creăm un nou pixel cu valoarea de contrast ajustată
                    Color adjustedPixel = Color.FromArgb(originalPixel.A, adjustedIntensity, adjustedIntensity, adjustedIntensity);

                    // Actualizăm pixelul în noua imagine
                    adjustedImage.SetPixel(x, y, adjustedPixel);
                }
            }

            return adjustedImage;
        }

        // Aplicarea corectiei gamma
        public static Bitmap ApplyGammaCorrection(Bitmap originalImage, double gamma = 1.5f)
        {
            // Creează o copie a imaginii originale pentru a evita modificarea acesteia
            Bitmap adjustedImage = (Bitmap)originalImage.Clone();

            // Parcurge fiecare pixel al imaginii și aplică corecția gamma
            for (int x = 0; x < adjustedImage.Width; x++)
            {
                for (int y = 0; y < adjustedImage.Height; y++)
                {
                    Color originalColor = adjustedImage.GetPixel(x, y);

                    // Extrage valorile R, G și B ale pixelului original
                    double red = originalColor.R / 255.0;
                    double green = originalColor.G / 255.0;
                    double blue = originalColor.B / 255.0;

                    // Aplică corecția gamma pentru fiecare componentă de culoare
                    double correctedRed = Math.Pow(red, 1.0 / gamma);
                    double correctedGreen = Math.Pow(green, 1.0 / gamma);
                    double correctedBlue = Math.Pow(blue, 1.0 / gamma);

                    // Asigură că valorile corectate sunt în intervalul [0, 1]
                    correctedRed = Math.Max(0.0, Math.Min(1.0, correctedRed));
                    correctedGreen = Math.Max(0.0, Math.Min(1.0, correctedGreen));
                    correctedBlue = Math.Max(0.0, Math.Min(1.0, correctedBlue));

                    // Creează un nou color cu valorile corectate
                    Color correctedColor = Color.FromArgb(
                        (int)(correctedRed * 255),
                        (int)(correctedGreen * 255),
                        (int)(correctedBlue * 255)
                    );

                    // Setează pixelul corectat în imaginea rezultată
                    adjustedImage.SetPixel(x, y, correctedColor);
                }
            }

            return adjustedImage;
        }

        // Schimbarea luminozitatii
        public static Bitmap AdjustBrightness(Bitmap originalImage, int brightnessLevel = 70)
        {
            Bitmap adjustedImage = new Bitmap(originalImage.Width, originalImage.Height);

            // Parcurgem fiecare pixel al imaginii originale
            for (int x = 0; x < originalImage.Width; x++)
            {
                for (int y = 0; y < originalImage.Height; y++)
                {
                    // Obținem culoarea pixelului
                    Color originalColor = originalImage.GetPixel(x, y);

                    // Calculăm noua valoare a culorii pentru luminozitatea modificată
                    int newRed = originalColor.R + brightnessLevel;
                    int newGreen = originalColor.G + brightnessLevel;
                    int newBlue = originalColor.B + brightnessLevel;

                    // Asigurăm că valorile noilor culori sunt între 0 și 255
                    newRed = Math.Max(0, Math.Min(255, newRed));
                    newGreen = Math.Max(0, Math.Min(255, newGreen));
                    newBlue = Math.Max(0, Math.Min(255, newBlue));

                    // Construim noua culoare cu luminozitatea modificată
                    Color adjustedColor = Color.FromArgb(newRed, newGreen, newBlue);

                    // Setăm culoarea pixelului în imaginea ajustată
                    adjustedImage.SetPixel(x, y, adjustedColor);
                }
            }

            return adjustedImage;
        }

        // Algoritmul de egalizare al histogramei
        public static Bitmap EqualizeHistogram(Bitmap grayscaleImage)
        {
            // Convert the image to grayscale if necessary
            if (grayscaleImage.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                grayscaleImage = Effects.ConvertToGrayscale(grayscaleImage);
            }

            // Calculate histogram
            int[] histogram = Effects.GenerateGrayscaleHistogram(grayscaleImage);

            // Calculate probabilities
            double[] probabilities = Effects.CalculateProbabilities(histogram, grayscaleImage.Width * grayscaleImage.Height);

            // Calculate cumulative probabilities
            double[] cumulativeProbabilities = Effects.CalculateCumulativeProbabilities(probabilities);

            // Calculate transformation mapping
            int[] transformationMap = new int[256];
            for (int i = 0; i < 256; i++)
            {
                transformationMap[i] = (int)Math.Round(255 * cumulativeProbabilities[i]);
            }

            // Apply histogram equalization transformation
            Bitmap equalizedImage = new Bitmap(grayscaleImage.Width, grayscaleImage.Height);
            for (int x = 0; x < grayscaleImage.Width; x++)
            {
                for (int y = 0; y < grayscaleImage.Height; y++)
                {
                    Color pixelColor = grayscaleImage.GetPixel(x, y);
                    int intensity = pixelColor.R; // Assuming the image is grayscale
                    int newIntensity = transformationMap[intensity];
                    Color newPixelColor = Color.FromArgb(newIntensity, newIntensity, newIntensity);
                    equalizedImage.SetPixel(x, y, newPixelColor);
                }
            }

            return equalizedImage;
        }

        /*********************Tema 9*********************/

        // Filtru "trece jos" (de netezire a imaginilor, de eliminare a zgomotelor), 
        public static Bitmap SmoothingImage(Bitmap bitmap)
        {
            // Convertim imaginea Bitmap într-o matrice OpenCV
            Mat inputImage = BitmapToMat(bitmap);

            // Aici poți aplica filtrele necesare folosind funcțiile din OpenCV
            Cv2.GaussianBlur(inputImage, inputImage, new OpenCvSharp.Size(3, 3), 0);

            // Convertim matricea rezultată înapoi într-o imagine Bitma
            return MatToBitmap(inputImage);
        }

        public static Bitmap DetectEdges(Bitmap bitmap)
        {
            // Convertim imaginea Bitmap într-o matrice OpenCV
            Mat inputImage = BitmapToMat(bitmap);
            // Convertim imaginea într-o imagine gri pentru a facilita detectarea marginilor
            Mat grayImage = new Mat();
            Cv2.CvtColor(inputImage, grayImage, ColorConversionCodes.BGR2GRAY);

            // Aplicăm filtrul Canny pentru detectarea marginilor
            Mat edgesImage = new Mat();
            Cv2.Canny(grayImage, edgesImage, 50, 150);

            return MatToBitmap(inputImage);
        }

    }

}