using Binder.Environment;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace Binder
{
    static class Utils
    {
        public static Size ScreenResolution => System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;
        public static Point ScreenCenter => new Point(ScreenResolution.Width / 2, ScreenResolution.Height / 2);
        public static Random Random = new Random();
        

        public static string NameWithoutExtension(this FileInfo file)
        {
            return file.Name.Replace(file.Extension, string.Empty);
        }
        public static double FixBetween(double originValue, double min, double max)
        {
            return Math.Min(Math.Max(originValue, min), max);
        }
        public static Size ScreenResolutionWithoutTaskbar
        {
            get
            {
                if (Taskbar.Position == TaskbarPosition.Bottom || Taskbar.Position == TaskbarPosition.Top) return new Size(ScreenResolution.Width, ScreenResolution.Height - Taskbar.CurrentBounds.Height);
                else if (Taskbar.Position == TaskbarPosition.Left || Taskbar.Position == TaskbarPosition.Right) return new Size(ScreenResolution.Width - Taskbar.CurrentBounds.Width, ScreenResolution.Height);
                else return ScreenResolution;
            }
        }
        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }

        public static Bitmap ResizeBitmap(this Bitmap bitmap, Size newSize)
        {
            double ratio = 0d;
            double myThumbWidth = 0d;
            double myThumbHeight = 0d;

            Bitmap resultBitmap;

            if ((bitmap.Width / Convert.ToDouble(newSize.Width)) > (bitmap.Height /
            Convert.ToDouble(newSize.Height)))
                ratio = Convert.ToDouble(bitmap.Width) / Convert.ToDouble(newSize.Width);
            else
                ratio = Convert.ToDouble(bitmap.Height) / Convert.ToDouble(newSize.Height);
            myThumbHeight = Math.Ceiling(bitmap.Height / ratio);
            myThumbWidth = Math.Ceiling(bitmap.Width / ratio);

            Size thumbSize = new Size((int)newSize.Width, (int)newSize.Height);
            resultBitmap = new Bitmap(newSize.Width, newSize.Height);
            int x = (newSize.Width - thumbSize.Width) / 2;
            int y = (newSize.Height - thumbSize.Height);
            // Had to add System.Drawing class in front of Graphics ---
            Graphics graphics = Graphics.FromImage(resultBitmap);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            Rectangle rect = new Rectangle(x, y, thumbSize.Width, thumbSize.Height);
            graphics.DrawImage(bitmap, rect, 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel);

            return resultBitmap;
        }

        public static Bitmap GetBitmapFromFile(string path)
        {
            if (!File.Exists(path)) throw new IOException("File path doest not exists");
            using (MemoryStream stream = new MemoryStream(File.ReadAllBytes(path)))
            {
                return new Bitmap(stream);
            }
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern UInt32 GetWindowThreadProcessId(IntPtr hwnd, ref Int32 pid);
    }
}
