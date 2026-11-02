using System;
using System.IO;
using Raytracer.Utils;

namespace Raytracer.Core;

class Program 
{
    public static void Main()
    {
        string filename = "output.ppm";
        string path = AppContext.BaseDirectory + filename;
        int imageHeight = 500;
        int imageWidth = 500;

        StreamWriter writer = new(path);

        writer.WriteLine("P3");
        writer.WriteLine("#" + filename);
        writer.WriteLine(imageHeight + " " + imageWidth);
        writer.WriteLine("255");
        
        for (int y = 0; y < imageHeight; y++)
        {
            Console.Write("\rScanlines Remaining: " + (imageHeight-y) + "      ");
            for (int x = 0; x < imageWidth; x++)
            {
                Color color = new(
                (double)y / (imageHeight-1),
                0.0,
                (double)x / (imageWidth-1));

                WritePixel(ref writer, color);
            }
            writer.WriteLine();
        }
        Console.WriteLine("\rDone.                           ");
        writer.Dispose();
    }

    public static void WritePixel(ref StreamWriter writer, Color pixelColor)
    {
        int r = (int)(255 * pixelColor.R());
        int g = (int)(255 * pixelColor.G());
        int b = (int)(255 * pixelColor.B());

        writer.Write(r + " " + g + " " + b + "  ");
    }
}