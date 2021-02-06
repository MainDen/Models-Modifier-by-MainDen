using System;
using System.Drawing;

namespace Core_by_MainDen.Models
{
    public class BitmapModel : AbstractModel
    {
        private Bitmap bitmap;

        public Bitmap Bitmap
        {
            get
            {
                using (Access.GetAccess())
                    return bitmap;
            }
            set
            {
                if (value is null)
                    throw new ArgumentNullException(nameof(value));
                using (Access.GetAccess())
                    bitmap = value;
            }
        }

        public BitmapModel(Bitmap bitmap)
        {
            if (bitmap is null)
                throw new ArgumentNullException(nameof(bitmap));
            this.bitmap = bitmap;
        }
    }
}
