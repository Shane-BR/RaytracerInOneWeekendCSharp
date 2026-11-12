using System;
using System.IO;
using System.Net.NetworkInformation;
using Raytracer.Utils;

namespace Raytracer.Core;

class Program 
{
    public static readonly Camera cam = new(new Point3(0, 0, 0), 240, 16.0/9.0);

    private static readonly HittableList world = new();

    public static void Main()
    {
        string filename = "output.ppm";
        string path = AppContext.BaseDirectory + filename;

        Material ground = new Lambertian(new Color(0.8, 0.8, 0.0));
        Material metal = new Metal(new Color(0.8, 0.8, 0.8));
        Material ball = new Lambertian(new Color(0.1, 0.2, 0.5));

        world.Add(new Sphere(new Point3(0, 1, -2), 0.5, ball));
        world.Add(new Sphere(new Point3(2, 0, -2.2), 0.5, ball));
        world.Add(new Sphere(new Point3(-2.1, 0, -2), 0.5, metal));
        world.Add(new Sphere(new Point3(0, -100.5, -1), 100, ground));

        using ImageWriter writer = new(path, cam.ImageWidth, cam.ImageHeight);
        cam.Render(world, writer);
    }
}