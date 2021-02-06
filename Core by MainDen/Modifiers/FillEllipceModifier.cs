using Core_by_MainDen.Models;
using System;
using System.Drawing;

namespace Core_by_MainDen.Modifiers
{
    public class FillEllipceModifier : AbstractModifier
    {
        private static readonly string argsInfo = "";

        public override AbstractModel ApplyTo(AbstractModel model, params object[] args)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));
            if (!(model is BitmapModel source))
                throw new ArgumentException("Expected BitmapModel.");

            using (model.Access.GetAccess())
            {
                Bitmap sourceBitmap = source.Bitmap;
                if (sourceBitmap is null || sourceBitmap.Width == 0 || sourceBitmap.Height == 0)
                    throw new ArgumentException("Invalid model.");

                Bitmap result = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);
                double a = result.Width / 2.0;
                double b = result.Height / 2.0;
                EllipseMath math = new EllipseMath(a, b, a, b);
                for (int i = 0; i < result.Height; i++)
                    for (int j = 0; j < result.Width; j++)
                    {
                        math.SetLocalXInEllipse(j);
                        math.SetLocalYInEllipse(i);
                        if (math.ContainsIn())
                            result.SetPixel(j, i, sourceBitmap.GetPixel(
                                Math.Max(0, Math.Min(result.Width - 1, j)),
                                Math.Max(0, Math.Min(result.Height - 1, (int)math.GetYInBorder()))
                                ));
                    }
                return new BitmapModel(result);
            }
        }

        public override bool CanBeAppliedTo(AbstractModel model)
        {
            return !(model as BitmapModel is null);
        }

        public override string GetArgsInfo(AbstractModel model)
        {
            return argsInfo;
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
                return x / Math.Sqrt(xx / aa + yy * bb);
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
