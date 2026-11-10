using Raytracer.Utils;

namespace Raytracer.Core;

class Camera
{
    private Point3 eyePoint = new(0, 0, 0);
    private double focalLength = 1.0;
    private readonly int samplesPerPixel = 10;
    private readonly double pixelSampleScale;

    private readonly double viewportHeight = 2.0;
    private readonly double viewportWidth;

    private readonly Vector3 viewportXDir;
    private readonly Vector3 viewportYDir;
    private readonly Vector3 pixelDeltaX;
    private readonly Vector3 pixelDeltaY;
    private readonly Vector3 pixel00Loc;

    public Camera(int imageWidth, int imageHeight)
    {
        pixelSampleScale = 1.0 / samplesPerPixel;

        viewportHeight = 2.0;
        viewportWidth = viewportHeight * (double)imageWidth/imageHeight;

        viewportXDir = new(viewportWidth, 0, 0);
        viewportYDir = new(0, -viewportHeight, 0);
        pixelDeltaX = viewportXDir / imageWidth;
        pixelDeltaY = viewportYDir / imageHeight;

        Vector3 viewportUpperLeft = eyePoint
                                        - new Vector3(0, 0, focalLength)
                                        - viewportXDir/2 - viewportYDir/2;
        pixel00Loc = viewportUpperLeft + 0.5 * (pixelDeltaX + pixelDeltaY);
    }

    private Color RayColor(Ray ray, Hittable world)
    {
        if (world.Hit(ray, new DoubleInterval(0, double.PositiveInfinity), out HitRecord record))
        {
            return (new Color(  0.5*(record.normal.X+1), 
                                0.5*(record.normal.Y+1), 
                                0.5*(record.normal.Z+1)));
        }

        Vector3 unitDir = Vector3.UnitVector(ray.Direction);
        double a = 0.5*(unitDir.Y + 1.0);
        Color color = (1.0-a) * (new Color(1.0, 1.0, 1.0)) + a*(new Color(0.5, 0.7, 1.0));
        return color;
    }

    private Vector3 RandSampleSquare()
    {
        Random rand = new();
        return new Vector3(rand.NextDouble() - 0.5, rand.NextDouble() - 0.5, 0);
    }

    private Ray GetRay(int x, int y)
    {
        Vector3 offset = RandSampleSquare();
        Point3 pixelSample = pixel00Loc 
                                + (x + offset.X) * pixelDeltaX
                                + (y + offset.Y) * pixelDeltaY;
        
        Point3 rayOrig = eyePoint;
        Vector3 rayDir = pixelSample - rayOrig;

        return new Ray(rayOrig, rayDir);
    }

    public Vector3 EyePoint
    {
        get { return eyePoint;}
        set { eyePoint = value;}
    }

    public double FocalLength
    {
        get {return focalLength;}
        set {focalLength = value;}
    }

    public void Render(Hittable world, ImageWriter writer)
    {
        for (int y = 0; y < writer.ImageHeight; y++)
        {
            Console.Write("\rScanlines Remaining: " + (writer.ImageHeight-y) + "      ");
            for (int x = 0; x < writer.ImageWidth; x++)
            {
                Color pixelColor = new(0, 0, 0);
                for (int sample = 0; sample < samplesPerPixel; sample++)
                {
                    Ray ray = GetRay(x, y);
                    pixelColor += RayColor(ray, world) * pixelSampleScale;
                }
                writer.WritePixel(x, pixelColor);
            }
        }
        Console.WriteLine("\rDone.                           ");
    }
}