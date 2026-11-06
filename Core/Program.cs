using System;
using System.IO;
using System.Net.NetworkInformation;
using Raytracer.Utils;

namespace Raytracer.Core;

class Program 
{

    public const int imageWidth = 960;
    public const double aspectRatio = 16/9;
    public const int imageHeight = 
        ((int)(imageWidth / aspectRatio) < 1) ? 1 : (int)(imageWidth / aspectRatio);
    
    public const double viewportHeight = 2.0;
    public const double viewportWidth = viewportHeight * (double)imageWidth/imageHeight;

    public static Camera cam = new();

    private static readonly Vector3 viewportXDir = new(viewportWidth, 0, 0);
    private static readonly Vector3 viewportYDir = new(0, -viewportWidth, 0);
    private static Vector3 pixelDeltaX = viewportXDir / imageWidth;
    private static Vector3 pixelDeltaY = viewportYDir / imageHeight;

    public static void Main()
    {
        string filename = "output.ppm";
        string path = AppContext.BaseDirectory + filename;

        Vector3 viewportUpperLeft = cam.GetEyePoint()
                                        - new Vector3(0, 0, cam.GetFocalLength())
                                        - viewportXDir/2 - viewportYDir/2;
        Point3 pixel00Loc = viewportUpperLeft + 0.5 * (pixelDeltaX + pixelDeltaY);

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
                Point3 pixelCenter = pixel00Loc + (x * pixelDeltaX)
                                                + (y * pixelDeltaY);

                Vector3 rayDir = pixelCenter - cam.GetEyePoint();
                Ray ray = new(cam.GetEyePoint(), rayDir);
                Color color = RayColor(ray);

                WritePixel(writer, color);
            }
            writer.WriteLine();
        }
        Console.WriteLine("\rDone.                           ");
    }

    public static void WritePixel(StreamWriter writer, Color pixelColor)
    {
        int r = (int)(255 * pixelColor.R());
        int g = (int)(255 * pixelColor.G());
        int b = (int)(255 * pixelColor.B());

        writer.Write(r + " " + g + " " + b + "  ");
    }

    public static Color RayColor(Ray ray)
    {
        if (HitSphere(new Point3(0, 0, -1), 0.2, ray))
        {
            return new Color(1, 0, 0);
        }

        Vector3 unitDir = Vector3.UnitVector(ray.GetDirection());
        double a = 0.5*(unitDir.GetY() + 1.0);
        Color color = (1.0-a) * (new Color(1.0, 1.0, 1.0)) + a*(new Color(0.5, 0.7, 1.0));
        return color;
    }

    public static bool HitSphere(Point3 center, double radius, Ray ray)
    {
        Vector3 oc = center - ray.GetOrigin();
        double a = Vector3.Dot(ray.GetDirection(), ray.GetDirection());
        double b = -2.0 * Vector3.Dot(ray.GetDirection(), oc);
        double c = Vector3.Dot(oc, oc) - radius*radius;
        double discriminant = b*b - 4*a*c;
        return discriminant >= 0;
    }
}