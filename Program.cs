using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using AForge.Imaging;
using System.Windows.Input;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace cms21_autoscrap
{
    internal class Program
    {
        const byte VK_SPACE = 0x20;
        private const int KEYEVENTF_KEYDOWN = 0x0001;
        private const int KEYEVENTF_KEYUP = 0x0002;

        static void Main(string[] args)
        {
            Bitmap patternImage = new Bitmap("example.png");
            Bitmap patternImage2 = new Bitmap("example2.png");
            patternImage = RemoveAlphaChannel(patternImage);
            patternImage2 = RemoveAlphaChannel(patternImage2);
            while (true)
            {
                var match = LocateImageWithinRegion(new Bitmap[] { patternImage, patternImage2}, 660, 523, 617, 153);
                if (match != null)
                {
                    Console.WriteLine(match);
                    
                }
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void keybd_event(uint bVk, uint bScan, uint dwFlags, UIntPtr dwExtraInfo);

        public static TemplateMatch? LocateImageWithinRegion(Bitmap[] patternImages, int x, int y, int width, int height)
        {
            var img = new Bitmap(ScreenCapture.CaptureDesktop());
            img = RemoveAlphaChannel(img);
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0.9f);
            foreach(var patternImage in patternImages)
            {
                var matches = tm.ProcessImage(img, patternImage, new Rectangle(x, y, width, height));
                if (matches.Length > 0)
                {
                    keybd_event(VK_SPACE, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    keybd_event(VK_SPACE, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                    return matches[0];
                }
            }
            return null;
        }
        public static Bitmap RemoveAlphaChannel(Bitmap bitmap)
        {
            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            return bitmap.Clone(rect, PixelFormat.Format24bppRgb);
        }
    }
}
