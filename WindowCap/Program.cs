using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowCap
{
    class Program
    {

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindow(IntPtr hWnd);

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        static String strWindowTitle;
        static String strWindowClass=null;
        static String strFilePath = "window.jpg";
        static String output = "";
        static bool boolShowUsage = false;
        static bool boolSilent = false;


        static void Main(string[] args)
        {
            

            if (args.Length > 0)
            {

                if (args.Contains("/?"))
                {
                    boolShowUsage = true;
                }
                else
                {
                    strWindowTitle = args[0];

                    if (args.Length > 1)
                    {
                        if (System.IO.Directory.Exists(args[1]))
                        {
                            strFilePath = args[1] + "\\" + strFilePath;
                        }
                        else
                        {
                            if (args[1] != "/silent")
                            {
                                output += "Directory not found:  " + args[1] + "\n\n";
                                boolShowUsage = true;
                            }
                        }
                    }

                }

                if (args.Contains("/silent"))
                {
                    boolSilent = true;
                }

            }
            else
            {
                boolShowUsage = true;
            }

            if (boolShowUsage == false)
            {
                IntPtr handle = FindWindow(strWindowClass, strWindowTitle);
                if (!IsWindow(handle))
                {
                    output += "Could not find window titled:  " + strWindowTitle;
                }
                else
                {
                    CaptureWindow(handle);
                }
            }

            if (boolSilent)
                Environment.Exit(0);


            AllocConsole();
            //AttachConsole(ATTACH_PARENT_PROCESS);

            if (boolShowUsage)
            {
                output += "Joe Ostrander\n";
                output += "2017.03.10\n";
                output += "Take a screenshot of a specific window\n\n";
                output += "Usage:  \n"; ;
                output += System.Windows.Forms.Application.ProductName + " <window name>\n";
                output += "or\n";
                output += System.Windows.Forms.Application.ProductName + " <window name> <directory name> /silent\n\n";
                output += "Press ENTER to continue...";

                if (boolSilent == false)
                {
                    Console.WriteLine();
                    Console.WriteLine(output);
                    Console.ReadLine();
                }

            }
            else
            {
                if (boolSilent == false)
                {
                    Console.WriteLine();
                    Console.WriteLine(output);
                    Console.WriteLine();
                    System.Threading.Thread.Sleep(2000);
                }

            }

        }


        private static void CaptureWindow(IntPtr handle)
        {
            ScreenShot.ScreenCapture SC = new ScreenShot.ScreenCapture();
            
            try
            {
                Image bmTemp = SC.CaptureWindow(handle);
                bmTemp.Save(strFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);

                output += "Window captured to:  " + strFilePath + "\n";
            }
            catch (Exception ex)
            {
                output += ex.Message + "\n";
            }


        }
    }
}
