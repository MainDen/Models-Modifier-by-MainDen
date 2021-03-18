using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using C = System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MainDen.ModelsModifier.ViewModels
{
    public class ModelViewModel : AbstractViewModel
    {
        private object model = null;
        public object Model
        {
            get => model;
            set
            {
                model = value;
                OnPropertyChanged(nameof(View));
            }
        }

        public FrameworkElement View
        {
            get
            {
                if (model is null)
                    return new FrameworkElement();
                Bitmap bitmap = model as Bitmap;
                if (!(bitmap is null))
                {
                    C.Image image = new C.Image();
                    image.Source = BitmapToImageSource(model as Bitmap);
                    C.ToolTip toolTip = new C.ToolTip();
                    toolTip.Content = $"Width: {bitmap.Width}\nHeight: {bitmap.Height}";
                    image.ToolTip = toolTip;
                    C.ContextMenu contextMenu = new C.ContextMenu();
                    C.MenuItem saveItem = new C.MenuItem();
                    saveItem.Header = "Save as";
                    saveItem.Click += delegate
                    {
                        Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                        if (saveFileDialog.ShowDialog() == true)
                            bitmap.Save(saveFileDialog.FileName);
                    };
                    contextMenu.Items.Add(saveItem);
                    image.ContextMenu = contextMenu;
                    return image;
                }
                return new FrameworkElement();
            }
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
