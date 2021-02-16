using System;
using System.Drawing;

namespace Modifiers_by_MainDen.Modifiers
{
    public class ShiftRectangleThroughTileModifier : AbstractModifier
    {
        private static readonly string argsInfo = "int|X;int|Y;int(0,MAX)|Width;int(0,MAX)|Height";

        public override string Name => "Shift Rectangle through Tile";

        public override object ApplyTo(object model, params object[] args)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));
            if (!(model is Bitmap source))
                throw new ArgumentException("Expected Bitmap.");
            if (source.Width == 0 || source.Height == 0)
                throw new ArgumentException("Invalid model.");

            if (args.Length != 4 ||
                args[0] as int? is null ||
                args[1] as int? is null ||
                args[2] as int? is null ||
                args[3] as int? is null)
                throw new ArgumentException("Invalid args.");
            int x = (((int)args[0] % source.Width) + source.Width) % source.Width;
            int y = (((int)args[1] % source.Height) + source.Height) % source.Height;
            int width = (int)args[2];
            int height = (int)args[3];
            if (width <= 0 || height <= 0)
                throw new ArgumentException("Invalid args.");

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

        public override string GetArgsInfo(object model)
        {
            return argsInfo;
        }
    }
}
