﻿using MainDen.ModifiersCore.Modifiers;
using System;
using System.Drawing;

namespace MainDen.StandardBitmapModifiersPack.Modifiers
{
    public class ImageFromFileModifier : AbstractModifier
    {
        private static string name = "Image from File";
        private static string[] argNames = new string[] { "Path" };
        private static string[] argHints = new string[] { "Path to image file." };
        private static string[] argFormats = new string[] { "openfilepath|Image files (*.bmp, *.gif, *.jpg, *.png)|*.bmp;*.gif;*.jpg;*.png" };
        private static string[] argDefaults = new string[] { "" };

        public override string Name => name;
        public override string[] ArgNames => argNames;
        public override string[] ArgHints => argHints;
        public override string[] ArgFormats => argFormats;
        public override string[] ArgDefaults => argDefaults;

        public override object ApplyTo(object model)
        {
            if (!(model is null))
                throw new ArgumentException("Unexpected model.");

            if (ContainsNullArgStates())
                throw new Exception("Invalid args.", new MethodAccessException("ArgStates is not initialized."));

            string path = ArgStates[0];
            try
            {
                return (Bitmap)Image.FromFile(path.Replace("\"", ""));
            }
            catch (Exception e)
            {
                throw new Exception("Invalid args.", e);
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
    }
}
