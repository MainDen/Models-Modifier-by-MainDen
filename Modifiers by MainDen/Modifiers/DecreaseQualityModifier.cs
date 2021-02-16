using System;
using System.Drawing;

namespace Modifiers_by_MainDen.Modifiers
{
    public class DecreaseQualityModifier : AbstractModifier
    {
        private static readonly string argsInfo = "float(0,1] | Quality";

        public override string Name => "Decrease Quality";

        public override object ApplyTo(object model, params object[] args)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));
            if (!(model is Bitmap source))
                throw new ArgumentException("Expected Bitmap.");
            if (source.Width == 0 || source.Height == 0)
                throw new ArgumentException("Invalid model.");
            
            if (args.Length != 1 || args[0] as float? is null)
                throw new ArgumentException("Invalid args.");
            float quality = (float)args[0];
            if (quality <= 0 || quality > 1)
                throw new ArgumentException("Invalid args.");

            int width = (int)(source.Width * quality);
            int height = (int)(source.Height * quality);
            return new Bitmap(source, width == 0 ? 1 : width, height == 0 ? 1 : height);
        }

        public override bool CanBeAppliedTo(Type modelType)
        {
            return modelType == typeof(Bitmap);
        }

        public override Type ResultType(Type modelType)
        {
            return typeof(Bitmap);
        }

        public override string GetArgsInfo(object model)
        {
            return argsInfo;
        }
    }
}
