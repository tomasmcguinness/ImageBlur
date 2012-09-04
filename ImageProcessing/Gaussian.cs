using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing
{
    public class Gaussian
    {
        public static double[,] Calculate1DSampleKernel(double deviation, int size)
        {
            double[,] ret = new double[size, 1];
            double sum = 0;
            int half = size / 2;
            for (int i = 0; i < size; i++)
            {
                ret[i, 0] = 1 / (Math.Sqrt(2 * Math.PI) * deviation) * Math.Exp(-(i - half) * (i - half) / (2 * deviation * deviation));
                sum += ret[i, 0];
            }
            return ret;
        }

        public static double[,] Calculate1DSampleKernel(double deviation)
        {
            int size = (int)Math.Ceiling(deviation * 3) * 2 + 1;
            return Calculate1DSampleKernel(deviation, size);
        }

        public static double[,] CalculateNormalized1DSampleKernel(double deviation)
        {
            return NormalizeMatrix(Calculate1DSampleKernel(deviation));
        }

        public static double[,] NormalizeMatrix(double[,] matrix)
        {
            double[,] ret = new double[matrix.GetLength(0), matrix.GetLength(1)];
            double sum = 0;
            for (int i = 0; i < ret.GetLength(0); i++)
            {
                for (int j = 0; j < ret.GetLength(1); j++)
                    sum += matrix[i, j];
            }
            if (sum != 0)
            {
                for (int i = 0; i < ret.GetLength(0); i++)
                {
                    for (int j = 0; j < ret.GetLength(1); j++)
                        ret[i, j] = matrix[i, j] / sum;
                }
            }
            return ret;
        }

        public static double[,] GaussianConvolution(double[,] matrix, double deviation)
        {
            double[,] kernel = CalculateNormalized1DSampleKernel(deviation);
            double[,] res1 = new double[matrix.GetLength(0), matrix.GetLength(1)];
            double[,] res2 = new double[matrix.GetLength(0), matrix.GetLength(1)];
            //x-direction
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                    res1[i, j] = processPoint(matrix, i, j, kernel, 0);
            }
            //y-direction
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                    res2[i, j] = processPoint(res1, i, j, kernel, 1);
            }
            return res2;
        }

        private static double processPoint(double[,] matrix, int x, int y, double[,] kernel, int direction)
        {
            double res = 0;
            int half = kernel.GetLength(0) / 2;
            for (int i = 0; i < kernel.GetLength(0); i++)
            {
                int cox = direction == 0 ? x + i - half : x;
                int coy = direction == 1 ? y + i - half : y;
                if (cox >= 0 && cox < matrix.GetLength(0) && coy >= 0 && coy < matrix.GetLength(1))
                {
                    res += matrix[cox, coy] * kernel[i, 0];
                }
            }
            return res;
        }

        private Color grayscale(Color cr)
        {
            return Color.FromArgb(cr.A, (int)(cr.R * .3 + cr.G * .59 + cr.B * 0.11),
               (int)(cr.R * .3 + cr.G * .59 + cr.B * 0.11),
              (int)(cr.R * .3 + cr.G * .59 + cr.B * 0.11));
        }

        public Bitmap FilterProcessImage(double d, Bitmap image, int x, int x2, int y, int y2)
        {
            Bitmap ret = new Bitmap(image.Width, image.Height);
            double[,] redMatrix = new double[image.Width, image.Height];
            double[,] greenMatrix = new double[image.Width, image.Height];
            double[,] blueMatrix = new double[image.Width, image.Height];

            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    redMatrix[i, j] = image.GetPixel(i, j).R;
                    greenMatrix[i, j] = image.GetPixel(i, j).G;
                    blueMatrix[i, j] = image.GetPixel(i, j).B;
                }
            }

            redMatrix = Gaussian.GaussianConvolution(redMatrix, d);
            greenMatrix = Gaussian.GaussianConvolution(greenMatrix, d);
            blueMatrix = Gaussian.GaussianConvolution(blueMatrix, d);

            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    if (i > x && i < x2 && j > y && j < y2)
                    {
                        int redVal = (int)Math.Min(255, redMatrix[i, j]);
                        int greenVal = (int)Math.Min(255, greenMatrix[i, j]);
                        int blueVal = (int)Math.Min(255, blueMatrix[i, j]);
                        ret.SetPixel(i, j, Color.FromArgb(255, redVal, greenVal, blueVal));
                    }
                    else
                    {
                        ret.SetPixel(i, j, Color.FromArgb(image.GetPixel(i, j).ToArgb()));
                    }
                }
            }

            //for (int i = 0; i < image.Width; i++)
            //{
            //    for (int j = 0; j < image.Height; j++)
            //    {
            //        matrix[i, j] = grayscale(image.GetPixel(i, j)).R;
            //    }
            //}

            //matrix = Gaussian.GaussianConvolution(matrix, d);

            //for (int i = 0; i < image.Width; i++)
            //{
            //    for (int j = 0; j < image.Height; j++)
            //    {
            //        int val = (int)Math.Min(255, matrix[i, j]);
            //        ret.SetPixel(i, j, Color.FromArgb(255, val, val, val));
            //    }
            //}

            return ret;
        }
    }
}
