using System;
using System.IO;
using System.Net.NetworkInformation;
using Raytracer.Utils;

namespace Raytracer.Core;

class Program 
{

    public const int imageWidth = 1280;
    public const double aspectRatio = 16.0/9.0;
    public const int imageHeight = 
        ((int)(imageWidth / aspectRatio) < 1) ? 1 : (int)(imageWidth / aspectRatio);

    public static readonly Camera cam = new(imageWidth, imageHeight);

    private static readonly HittableList world = new();

    public static void Main()
    {
        string filename = "output.ppm";
        string path = AppContext.BaseDirectory + filename;

        world.Add(new Sphere(new Point3(0, 0, -1), 0.5));
        world.Add(new Sphere(new Point3(0, -100.5, -1), 100));

        using ImageWriter writer = new(path, imageWidth, imageHeight);
        cam.Render(world, writer);
    }

    // Solve for scalar of ray that intersects with sphere
    public static double HitSphere(Point3 center, double radius, Ray ray)
    {
        Vector3 oc = center - ray.Origin;
        double a = Vector3.Dot(ray.Direction, ray.Direction);
        double b = -2.0 * Vector3.Dot(ray.Direction, oc);
        double c = Vector3.Dot(oc, oc) - radius*radius;
        double discriminant = b*b - 4*a*c;
        
        if (discriminant < 0)
        {
            return -1.0;
        }
        else 
        {
            // For now, return one root
            return (-b - Math.Sqrt(discriminant)) / (2.0 * a);
        }
    }
}