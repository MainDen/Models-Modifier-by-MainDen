using Modifiers_Core_by_MainDen.Modifiers;
using System;
using System.Drawing;

namespace Standart_Modifiers_for_Bitmap_by_MainDen.Modifiers
{
    public class DecreaseQualityModifier : AbstractModifier
    {
        private static string name = "Decrease Quality";
        private static string[] argNames = new string[] { "Quality" };
        private static string[] argHints = new string[] { "Value: 0 < value <= 1." };
        private static string[] argDefaults = new string[] { "0,5" };

        public override string Name => name;
        public override string[] ArgNames => argNames;
        public override string[] ArgHints => argHints;
        public override string[] ArgDefaults => argDefaults;

        public override object ApplyTo(object model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));
            if (!(model is Bitmap source))
                throw new ArgumentException("Unexpected model.");
            if (source.Width == 0 || source.Height == 0)
                throw new ArgumentException("Invalid model.");

            if (ContainsNullArgStates())
                throw new Exception("Invalid args.", new MethodAccessException("ArgStates is not initialized."));
            
            float quality;
            if (!float.TryParse(ArgStates[0], out quality))
                throw new Exception("Invalid args.");
            if (quality <= 0 || quality > 1)
                throw new Exception("Invalid args.");

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
    }
}
