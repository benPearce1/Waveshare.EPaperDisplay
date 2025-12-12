#region Copyright
// --------------------------------------------------------------------------------------------------------------------
// MIT License
// Copyright(c) 2019 Andre Wehrli

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// --------------------------------------------------------------------------------------------------------------------
#endregion Copyright

#region Usings

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Waveshare.Devices;

#endregion Usings

namespace Waveshare.Example
{
    /// <summary>
    /// Example for the Waveshare E-Paper Library
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Application Main
        /// </summary>
        /// <param name="args">Commandline arguments</param>
        public static void Main(string[] args)
        {
            var timer = new Timer("Initializing E-Paper Display...");
            
            using var ePaperDisplay = EPaperDisplay.Create(EPaperDisplayType.WaveShare7In5_V2);
            if (ePaperDisplay == null)
            {
                return;
            }
            timer.Dispose();
            
            using var bitmap = LoadBitmap(args, ePaperDisplay.Width, ePaperDisplay.Height);
            if (bitmap == null)
            {
                return;
            }

            using (new Timer("Waiting for E-Paper Display..."))
            {
                ePaperDisplay.Clear();
                ePaperDisplay.WaitUntilReady();
            }

            using (new Timer("Sending Image to E-Paper Display..."))
            {
                ePaperDisplay.DisplayImage(bitmap, true);
            }

            Console.WriteLine("Done");
        }

        /// <summary>
        /// Load Bitmap from arguments or get the default bitmap
        /// </summary>
        /// <param name="args"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private static SkiaSharp.SKBitmap LoadBitmap(string[] args, int width, int height)
        {
            string bitmapFilePath;

            if (args == null || args.Length == 0)
            {
                var fileName = $"like_a_sir_{width}x{height}-mono.bmp";
                bitmapFilePath = Path.Combine(ExecutingAssemblyPath, fileName);
            }
            else
            {
                bitmapFilePath = args.First();
            }

            if (!File.Exists(bitmapFilePath))
            {
                Console.WriteLine($"Can not find Bitmap file: '{bitmapFilePath}'!");
                return null;
            }

            Console.WriteLine($"Loading Bitmap from '{bitmapFilePath}'...");
            
            return LoadSKBitmapFromFile(bitmapFilePath);
        }

        /// <summary>
        /// Load a SKBitmap from a file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static SkiaSharp.SKBitmap LoadSKBitmapFromFile(string filePath)
        {
            SkiaSharp.SKBitmap bitmap;
            using var stream = File.OpenRead(filePath);
            bitmap = SkiaSharp.SKBitmap.Decode(stream);

            if (bitmap == null)
            {
                throw new Exception($"Can not load Bitmap from '{filePath}'!");
            }
            
            Console.WriteLine($"Bitmap loaded: {bitmap.Width}x{bitmap.Height}");
            
            return bitmap;
        }

        /// <summary>
        /// Return the path of the executing assembly
        /// </summary>
        private static string ExecutingAssemblyPath
        {
            get
            {
                var path = Assembly.GetExecutingAssembly().Location;
                return Path.GetDirectoryName(path);
            }
        }
    }
    
    
    public class Timer : IDisposable
    {
        private Stopwatch sw;
        public Timer(string message)
        {
            Console.Write(message);
            sw = Stopwatch.StartNew();
            
        }
        public void Dispose()
        {
            sw.Stop();
            Console.WriteLine($" [Done {sw.ElapsedMilliseconds} ms]");
        }
    }
}