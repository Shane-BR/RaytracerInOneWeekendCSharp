using System;
using System.IO;
using System.Net.NetworkInformation;
using Raytracer.Utils;

namespace Raytracer.Core;

class Program 
{
    public static readonly Camera cam = new(new Point3(0, 0, 0), 1280, 16.0/9.0);

    private static readonly HittableList world = new();

    public static void Main()
    {
        string filename = "output.ppm";
        string path = AppContext.BaseDirectory + filename;

        world.Add(new Sphere(new Point3(0, 0, -1), 0.5));
        world.Add(new Sphere(new Point3(0, -100.5, -1), 100));

        using ImageWriter writer = new(path, cam.ImageWidth, cam.ImageHeight);
        cam.Render(world, writer);
    }
}