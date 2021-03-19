using MainDen.ModifiersCore.Modifiers;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace MainDen.StandardBitmapModifiersPack.Modifiers
{
    public class FillIntoEllipseModifier : AbstractModifier
    {
        private static string name = "Fill into Ellipce";

        public override string Name => name;

        public override object ApplyTo(object model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));
            if (!(model is Bitmap source))
                throw new ArgumentException("Unexpected model.");
            if (source.Width == 0 || source.Height == 0)
                throw new ArgumentException("Invalid model.");

            Bitmap result = new Bitmap(source.Width, source.Height);
            int w = source.Width;
            int h = source.Height;
            int wmax = w - 1;
            int hmax = h - 1;
            double a = wmax / 2.0;
            double b = hmax / 2.0;
            EllipseMath math = new EllipseMath(a, b, a, b);
            unsafe
            {
                byte[,,] src = BitmapToByteARGBQ(source);
                byte[,,] res = new byte[h, w, 4];
                fixed (byte* _src = src)
                fixed (byte* _res = res)
                {
                    uint* _s = (uint*)_src;
                    uint* _r = (uint*)_res;
                    for (int i = 0; i < h; i++)
                    {
                        for (int j = 0; j < w; j++)
                        {
                            math.SetLocalXInEllipse(j);
                            math.SetLocalYInEllipse(i);
                            if (math.ContainsIn())
                                *_r = *(_s + Limit(math.GetYInBorder(), 0, hmax) * w + Limit(math.GetXInBorder(), 0, wmax));
                            _r++;
                        }
                    }
                }
                return ByteARGBQToBitmap(res);
            }
        }

        public override bool CanBeAppliedTo(Type modelType)
        {
            return modelType == typeof(Bitmap);
        }

        public override Type ResultType(Type modelType)
        {
            return typeof(Bitmap);
        }

        private unsafe static byte[,,] BitmapToByteARGBQ(Bitmap source)
        {
            int width = source.Width;
            int height = source.Height;
            int len = height * width * 4;
            byte[,,] result = new byte[height, width, 4];
            BitmapData bd = source.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                void* src = (void*)bd.Scan0;
                fixed (void* dst = result)
                    Buffer.MemoryCopy(src, dst, len, len);
            }
            finally
            {
                source.UnlockBits(bd);
            }
            return result;
        }

        private unsafe static Bitmap ByteARGBQToBitmap(byte[,,] source)
        {
            int width = source.GetLength(1);
            int height = source.GetLength(0);
            int len = height * width * 4;
            Bitmap result = new Bitmap(width, height);
            BitmapData bd = result.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            try
            {
                void* dst = (void*)bd.Scan0;
                fixed (void* src = source)
                    Buffer.MemoryCopy(src, dst, len, len);
            }
            finally
            {
                result.UnlockBits(bd);
            }
            return result;
        }

        private static int Limit(double value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return (int)value;
        }

        private class EllipseMath
        {
            private double cx;
            private double cy;
            private double a;
            private double b;
            private double aa;
            private double bb;
            private double x = 0;
            private double y = 0;
            private double xx = 0;
            private double yy = 0;

            public EllipseMath(double A, double B, double CENTERX = 0F, double CENTERY = 0F)
            {
                a = A;
                b = B;
                aa = a * a;
                bb = b * b;
                cx = CENTERX;
                cy = CENTERY;
            }

            public bool Contains()
            {
                if (a == 0 && b == 0)
                    return x == 0 && y == 0;
                if (a == 0)
                    return x == 0 && yy / bb <= 1.0;
                if (b == 0)
                    return y == 0 && xx / aa <= 1.0;
                return xx / aa + yy / bb <= 1.0;
            }
            public bool ContainsOn()
            {
                if (a == 0 && b == 0)
                    return x == 0 && y == 0;
                if (a == 0)
                    return x == 0 && yy / bb <= 1.0;
                if (b == 0)
                    return y == 0 && xx / aa <= 1.0;
                return xx / aa + yy / bb == 1.0;
            }
            public bool ContainsIn()
            {
                if (a == 0 || b == 0)
                    return false;
                return xx / aa + yy / bb < 1.0;
            }

            public double SetLocalXInEllipse(double X)
            {
                x = X - cx;
                xx = x * x;
                return x;
            }
            public double SetLocalYInEllipse(double Y)
            {
                y = Y - cy;
                yy = y * y;
                return y;
            }

            public double GetLocalXOnEllipse()
            {
                if (x == 0 && y == 0)
                    return 0;
                if (a == 0 || b == 0 || y == 0)
                    if (x > 0)
                        return a;
                    else
                        return -a;
                return x / Math.Sqrt(xx / aa + yy / bb);
            }
            public double GetLocalYOnEllipse()
            {
                if (x == 0 && y == 0)
                    return 0;
                if (a == 0 || b == 0 || x == 0)
                    if (y > 0)
                        return b;
                    else
                        return -b;
                return y / Math.Sqrt(xx / aa + yy / bb);
            }

            public double GetLocalXOnBorder()
            {
                if (a == 0 || x == 0)
                    return 0;
                if (b == 0)
                    if (x > a)
                        return a;
                    else if (x < -a)
                        return -a;
                    else
                        return x;
                if (y == 0 || Math.Abs(x * b) >= Math.Abs(y * a))
                    if (x > 0)
                        return a;
                    else
                        return -a;
                else
                    return x / Math.Abs(y) * b;
            }
            public double GetLocalYOnBorder()
            {
                if (b == 0 || y == 0)
                    return 0;
                if (a == 0)
                    if (y > b)
                        return b;
                    else if (y < -b)
                        return -b;
                    else
                        return y;
                if (x == 0 || Math.Abs(y * a) >= Math.Abs(x * b))
                    if (y > 0)
                        return b;
                    else
                        return -b;
                else
                    return y / Math.Abs(x) * a;
            }

            public double GetLocalRadiusToXYInEllipse()
            {
                if (a == 0 || b == 0 || (x == 0 && y == 0))
                    return 0;
                return Math.Sqrt(xx + yy);
            }
            public double GetLocalRadiusToXYOnEllipse()
            {
                if (a == 0 || b == 0 || (x == 0 && y == 0))
                    return 0;
                double rx = GetLocalXOnEllipse();
                double ry = GetLocalYOnEllipse();
                return Math.Sqrt(rx * rx + ry * ry);
            }

            public double GetLocalRadiusToXYOnBorder()
            {
                if (a == 0 || b == 0 || (x == 0 && y == 0))
                    return 0;
                double rx = GetLocalXOnBorder();
                double ry = GetLocalYOnBorder();
                return Math.Sqrt(rx * rx + ry * ry);
            }
            public double GetLocalRadiusToXYInBorder()
            {
                double rInE = GetLocalRadiusToXYInEllipse();
                double rOnB = GetLocalRadiusToXYOnBorder();
                double rOnE = GetLocalRadiusToXYOnEllipse();
                if (rInE == 0 || rOnB == 0 || rOnE == 0)
                    return 0;
                return rInE * rOnB / rOnE;
            }

            public double GetLocalXInBorder()
            {
                if (a == 0 || b == 0 || (x == 0 && y == 0))
                    return 0;
                double xOnB = GetLocalXOnBorder();
                double xOnE = GetLocalXOnEllipse();
                if (xOnB == 0 || xOnE == 0)
                    return 0;
                return x / xOnE * xOnB;
            }
            public double GetLocalYInBorder()
            {
                if (a == 0 || b == 0 || (x == 0 && y == 0))
                    return 0;
                double yOnB = GetLocalYOnBorder();
                double yOnE = GetLocalYOnEllipse();
                if (yOnB == 0 || yOnE == 0)
                    return 0;
                return y / yOnE * yOnB;
            }

            public double GetXInBorder()
            {
                return GetLocalXInBorder() + cx;
            }
            public double GetYInBorder()
            {
                return GetLocalYInBorder() + cy;
            }
        }
    }
}
