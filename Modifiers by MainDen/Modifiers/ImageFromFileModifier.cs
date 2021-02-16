using System;
using System.Drawing;

namespace Modifiers_by_MainDen.Modifiers
{
    public class ImageFromFileModifier : AbstractModifier
    {
        private static readonly string argsInfo = "string | Path";

        public override string Name => "Image from File";

        public override object ApplyTo(object model, params object[] args)
        {
            if (args.Length != 1 || args[0] as string is null)
                throw new ArgumentException("Invalid args.");
            
            string path = (string)args[0];
            try
            {
                return (Bitmap)Image.FromFile(path);
            }
            catch
            {
                throw new ArgumentException("Invalid args.");
            }
        }

        public override bool CanBeAppliedTo(Type modelType)
        {
            return modelType == null;
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
