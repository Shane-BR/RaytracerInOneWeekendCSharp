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

        using StreamWriter writer = new(path);

        writer.WriteLine("P3");
        writer.WriteLine("#" + filename);
        writer.WriteLine(imageHeight + " " + imageWidth);
        writer.WriteLine("255");
        
        for (int y = 0; y < imageHeight; y++)
        {
            Console.Write("\rScanlines Remaining: " + (imageHeight-y) + "      ");
            for (int x = 0; x < imageWidth; x++)
            {
                int r = 255 * y / (imageHeight-1);
                int g = 0;
                int b = 255 * x / (imageWidth-1);

                writer.Write(r + " " + g + " " + b + "  ");
            }
            writer.WriteLine();
        }
        Console.WriteLine("\rDone.                           ");
    }
}