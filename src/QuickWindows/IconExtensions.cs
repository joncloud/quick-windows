using System;
using System.Drawing;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace QuickWindows
{
    static class IconExtensions
    {
        static readonly BitmapSizeOptions _emptyOptions = BitmapSizeOptions.FromEmptyOptions();
        public static ImageSource ToImageSource(this Icon icon)
        {
            using var _ = icon;
            using var bitmap = icon.ToBitmap();

            var hBitmap = bitmap.GetHbitmap();

            try
            {
                var wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    _emptyOptions
                );

                return wpfBitmap;
            }
            finally
            {
                if (!NativeMethods.DeleteObject(hBitmap))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
        }

    }
}
