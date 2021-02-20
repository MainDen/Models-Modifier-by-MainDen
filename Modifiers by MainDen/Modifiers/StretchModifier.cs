using System;
using System.Drawing;

namespace Modifiers_by_MainDen.Modifiers
{
    public class StretchModifier : AbstractModifier
    {
        private static string name = "Stretch";
        private static string[] argNames = new string[] { "Width", "Height" };
        private static string[] argHints = new string[] { "Any positive integer.", "Any positive integer." };
        private static string[] argDefaults = new string[] { "100", "100" };

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

            int width, height;
            try
            {
                string[] states = ArgStates;
                width = int.Parse(states[0]);
                height = int.Parse(states[1]);
            }
            catch
            {
                throw new Exception("Invalid args.");
            }
            if (width <= 0 || height <= 0)
                throw new Exception("Invalid args.");

            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
                g.DrawImage(source, 0, 0, width, height);
            return result;
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
