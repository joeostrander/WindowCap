using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

namespace ScreenShot
{
    public class ScreenCapture
    {
        /// Creates an Image object containing a screen shot of the entire desktop
        public Image CaptureScreen()
        {
            return CaptureWindow(User32.GetDesktopWindow());
        }
        //CaptureScreen
        /// Creates an Image object containing a screen shot of a specific window
        public Image CaptureWindow(IntPtr handle)
        {
            int SRCCOPY = 0xcc0020;
            // get te hDC of the target window
            IntPtr hdcSrc = User32.GetWindowDC(handle);
            // get the size
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(handle, ref windowRect);
            int width = windowRect.right - windowRect.left;
            int height = windowRect.bottom - windowRect.top;
            // create a device context we can copy to
            IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
            // select the bitmap object
            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
            // bitblt over
            GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, SRCCOPY);
            // restore selection
            GDI32.SelectObject(hdcDest, hOld);
            // clean up 
            GDI32.DeleteDC(hdcDest);
            User32.ReleaseDC(handle, hdcSrc);

            // get a .NET image object for it
            Image img = Image.FromHbitmap(hBitmap);
            // free up the Bitmap object
            GDI32.DeleteObject(hBitmap);

            return img;
        }
        //CaptureWindow
        /// Captures a screen shot of a specific window, and saves it to a file
        public void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format)
        {
            Image img = CaptureWindow(handle);
            img.Save(filename, format);
        }
        //CaptureWindowToFile
        /// Captures a screen shot of the entire desktop, and saves it to a file
        public void CaptureScreenToFile(string filename, ImageFormat format)
        {
            Image img = CaptureScreen();
            img.Save(filename, format);
        }
        //CaptureScreenToFile
        public Bitmap CaptureDeskTopRectangle(Rectangle CapRect, int CapRectWidth, int CapRectHeight)
        {
            /// Returns BitMap of the region of the desktop, similar to CaptureWindow, but can be used to 
            /// create a snapshot of the desktop when no handle is present, by passing in a rectangle 
            /// Grabs snapshot of entire desktop, then crops it using the passed in rectangle's coordinates
            ScreenShot.ScreenCapture SC = new ScreenShot.ScreenCapture();
            Bitmap bmpImage = new Bitmap(SC.CaptureScreen());
            Bitmap bmpCrop = new Bitmap(CapRectWidth, CapRectHeight, bmpImage.PixelFormat);
            Rectangle recCrop = new Rectangle(CapRect.X, CapRect.Y, CapRectWidth, CapRectHeight);
            Graphics gphCrop = Graphics.FromImage(bmpCrop);
            Rectangle recDest = new Rectangle(0, 0, CapRectWidth, CapRectHeight);
            gphCrop.DrawImage(bmpImage, recDest, recCrop.X, recCrop.Y, recCrop.Width, recCrop.Height, GraphicsUnit.Pixel);
            return bmpCrop;
        }
        /// Helper class containing Gdi32 API functions
        private class GDI32
        {
            public int SRCCOPY = 0xcc0020;
            [DllImport("gdi32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            // BitBlt dwRop parameter
            public static extern Int32 BitBlt(IntPtr hDestDC, Int32 x, Int32 y, Int32 nWidth, Int32 nHeight, IntPtr hSrcDC, Int32 xSrc, Int32 ySrc, Int32 dwRop);
            [DllImport("gdi32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

            public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, Int32 nWidth, Int32 nHeight);
            [DllImport("gdi32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

            public static extern IntPtr CreateCompatibleDC(IntPtr hdc);
            [DllImport("gdi32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

            public static extern Int32 DeleteDC(IntPtr hdc);
            [DllImport("gdi32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

            public static extern Int32 DeleteObject(IntPtr hObject);
            [DllImport("gdi32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

            public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);
        }
        //GDI32
        /// Helper class containing User32 API functions
        public class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }
            [DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            //RECT

            public static extern IntPtr GetDesktopWindow();
            [DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

            public static extern IntPtr GetWindowDC(IntPtr hwnd);
            [DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

            public static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);
            [DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

            public static extern Int32 GetWindowRect(IntPtr hwnd, ref RECT lpRect);

        }
        //User32
    }
    //ScreenCapture 

}
