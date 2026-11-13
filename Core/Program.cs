using System;
using System.IO;
using Raytracer.Utils;

namespace Raytracer.Core;

class Program 
{
    private static readonly HittableList world = new();

    public static void Main()
    {
        string filename = "output.ppm";
        string path = AppContext.BaseDirectory + filename;

        Material groundMat = new Lambertian(new Color(0.5, 0.5, 0.5));
        world.Add(new Sphere(new Point3(0, -1000, -1), 1000, groundMat));

        for (int a = -11; a < 11; a++)
        {
            for (int b = -11; b < 11; b++)
            {
                double chooseMat = RandomDouble.Get();
                Point3 centre = new(a + 0.9*RandomDouble.Get(), 0.2, b + 0.9*RandomDouble.Get());

                if ((centre - new Point3(4, 0.2, 0)).GetLength() > 0.9)
                {
                    Material sphereMat;

                    if (chooseMat < 0.8)
                    {
                        // Diffuse
                        Color albedo = Color.Random() * Color.Random();
                        sphereMat = new Lambertian(albedo);
                    }
                    else if (chooseMat < 0.95)
                    {
                        // Metal
                        Color albedo = Color.Random(0.5, 1.0);
                        double fuzz = RandomDouble.Get();
                        sphereMat = new Metal(albedo, fuzz);
                    }
                    else
                    {
                        // Glass
                        sphereMat = new Dielectric(1.5);
                    }
                    world.Add(new Sphere(centre, 0.2, sphereMat));
                }
            }
        }

        Material mat1 = new Dielectric(1.5);
        world.Add(new Sphere(new Point3(0, 1, 0), 1.0, mat1));

        Material mat2 = new Lambertian(new Color(0.4, 0.2, 0.1));
        world.Add(new Sphere(new Point3(-4, 1, 0), 1.0, mat2));

        Material mat3 = new Metal(new Color(0.7, 0.6, 0.5), 0.0);
        world.Add(new Sphere(new Point3(4, 1, 0), 1.0, mat3));

        Camera cam = 
        new(
            16.0 / 9.0,
            400,
            100,
            50,
            20.0,
            new Point3(13, 2, 3),
            new Point3(0, 0, 0),
            0.6,
            10.0
        );

        using ImageWriter writer = new(path, cam.ImageWidth, cam.ImageHeight);
        cam.Render(world, writer);
    }
}