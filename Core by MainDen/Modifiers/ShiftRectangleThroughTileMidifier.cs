using Core_by_MainDen.Models;
using System;
using System.Drawing;

namespace Core_by_MainDen.Modifiers
{
    public class ShiftRectangleThroughTileModifier : AbstractModifier
    {
        private static readonly string argsInfo = "int|X;int|Y;int(0,MAX)|Width;int(0,MAX)|Height";

        public override string Name => "Shift Rectangle Through Tile";

        public override AbstractModel ApplyTo(AbstractModel model, params object[] args)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));
            if (!(model is BitmapModel source))
                throw new ArgumentException("Expected IBitmapModel.");
            if (args.Length != 4 ||
                args[0] as int? is null ||
                args[1] as int? is null ||
                args[2] as int? is null ||
                args[3] as int? is null)
                throw new ArgumentException("Invalid args.");

            using (model.Access.GetAccess())
            {
                Bitmap sourceBitmap = source.Bitmap;
                if (sourceBitmap is null || sourceBitmap.Width == 0 || sourceBitmap.Height == 0)
                    throw new ArgumentException("Invalid model.");

                int x = (((int)args[0] % sourceBitmap.Width) + sourceBitmap.Width) % sourceBitmap.Width;
                int y = (((int)args[1] % sourceBitmap.Height) + sourceBitmap.Height) % sourceBitmap.Height;
                int width = (int)args[2];
                int height = (int)args[3];
                
                if (width <= 0 || height <= 0)
                    throw new ArgumentException("Invalid args.");

                Bitmap result = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(result))
                    for (int i = -y; i < height; i += sourceBitmap.Height)
                        for (int j = -x; j < width; j += sourceBitmap.Width)
                            g.DrawImage(sourceBitmap, j, i);
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
    }
}
