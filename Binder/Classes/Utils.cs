using System;
using System.Drawing;
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

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern UInt32 GetWindowThreadProcessId(IntPtr hwnd, ref Int32 pid);
    }
}
