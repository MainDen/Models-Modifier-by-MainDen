using Core_by_MainDen.Models;
using System;
using System.Drawing;

namespace Core_by_MainDen.Modifiers
{
    public class DecreaseQualityModifier : AbstractModifier
    {
        private static readonly string argsInfo = "float(0,1] | Quality";

        public override AbstractModel ApplyTo(AbstractModel model, params object[] args)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));
            if (!(model is BitmapModel source))
                throw new ArgumentException("Expected BitmapModel.");
            if (args.Length != 1 || args[0] as float? is null)
                throw new ArgumentException("Invalid args.");

            float quality = (float)args[0];
            if (quality <= 0 || quality > 1)
                throw new ArgumentException("Invalid args.");

            using (model.Access.GetAccess())
            {
                Bitmap sourceBitmap = source.Bitmap;
                if (sourceBitmap is null || sourceBitmap.Width == 0 || sourceBitmap.Height == 0)
                    throw new ArgumentException("Invalid model.");

                int width = (int)(sourceBitmap.Width * quality);
                int height = (int)(sourceBitmap.Height * quality);
                return new BitmapModel(new Bitmap(sourceBitmap, width == 0 ? 1 : width, height == 0 ? 1 : height));
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
