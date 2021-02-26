using MainDen.ModifiersCore.Modifiers;
using System;
using System.Drawing;

namespace MainDen.StandardBitmapModifiersPack.Modifiers
{
    public class ShiftRectangleThroughTileModifier : AbstractModifier
    {
        private static string name = "Shift Rectangle through Tile";
        private static string[] argNames = new string[] { "X", "Y", "Width", "Height" };
        private static string[] argHints = new string[] { "Any integer.", "Any integer.", "Any positive integer.", "Any positive integer." };
        private static string[] argDefaults = new string[] { "0", "0", "100", "100" };

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

            int x, y, width, height;
            try
            {
                string[] states = ArgStates;
                x = ((int.Parse(states[0]) % source.Width) + source.Width) % source.Width;
                y = ((int.Parse(states[1]) % source.Height) + source.Height) % source.Height;
                width = int.Parse(states[2]);
                height = int.Parse(states[3]);
            }
            catch
            {
                throw new Exception("Invalid args.");
            }
            if (width <= 0 || height <= 0)
                throw new Exception("Invalid args.");

            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
                for (int i = -y; i < height; i += source.Height)
                    for (int j = -x; j < width; j += source.Width)
                        g.DrawImage(source, j, i);
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
