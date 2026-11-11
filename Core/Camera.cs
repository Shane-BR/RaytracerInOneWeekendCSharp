using Raytracer.Utils;

namespace Raytracer.Core;

class Camera
{
    private Point3 centre;
    private double focalLength;
    private readonly int samplesPerPixel;
    private readonly double pixelSampleScale;
    private readonly int imageWidth, imageHeight;
    private readonly double aspectRatio;
    private readonly int bounceDepth;

    private readonly double viewportHeight;
    private readonly double viewportWidth;

    private readonly Vector3 viewportXDir;
    private readonly Vector3 viewportYDir;
    private readonly Vector3 pixelDeltaX;
    private readonly Vector3 pixelDeltaY;
    private readonly Vector3 pixel00Loc;

    public Camera(Point3 _centre, int _imageWidth, double _aspectRatio)
    {
        centre = _centre;
        focalLength = 1.0;
        samplesPerPixel = 15;
        pixelSampleScale = 1.0 / samplesPerPixel;
        imageWidth = _imageWidth;
        aspectRatio = _aspectRatio;
        bounceDepth = 10;
        imageHeight = 
            ((int)(imageWidth / aspectRatio) < 1) ? 1 
            : (int)(imageWidth / aspectRatio);
        
        viewportHeight = 2.0;
        viewportWidth = viewportHeight * (double)imageWidth/imageHeight;

        viewportXDir = new(viewportWidth, 0, 0);
        viewportYDir = new(0, -viewportHeight, 0);
        pixelDeltaX = viewportXDir / imageWidth;
        pixelDeltaY = viewportYDir / imageHeight;

        Vector3 viewportUpperLeft = centre
                                        - new Vector3(0, 0, focalLength)
                                        - viewportXDir/2 - viewportYDir/2;
        pixel00Loc = viewportUpperLeft + 0.5 * (pixelDeltaX + pixelDeltaY);
    }

    public int ImageWidth { get{return imageWidth;} }
    public int ImageHeight { get{return imageHeight;} }

    private Color RayColor(Ray ray, Hittable world, int bDepth)
    {
        if (bDepth <= 0)
        {
            return Color.Black();
        }

        if (world.Hit(ray, new DoubleInterval(0, double.PositiveInfinity), out HitRecord record))
        {
            Vector3 direction = Vector3.RandomOnHemisphere(record.normal);
            return 0.5 * RayColor(new Ray(record.at, direction), world, bDepth-1);
        }

        Vector3 unitDir = Vector3.UnitVector(ray.Direction);
        double a = 0.5*(unitDir.Y + 1.0);
        Color color = (1.0-a) * (new Color(1.0, 1.0, 1.0)) + a*(new Color(0.5, 0.7, 1.0));
        return color;
    }

    /*
        I have never looked at other anti-aliasing algorithms, but this seems
        stupid. Why not use a more deterministic algorithm?
    */
    private Vector3 RandSampleSquare()
    {
        return new Vector3(RandomDouble.Get() - 0.5, RandomDouble.Get() - 0.5, 0);
    }

    private Ray GetRay(int x, int y)
    {
        Vector3 offset = RandSampleSquare();
        Point3 pixelSample = pixel00Loc 
                                + (x + offset.X) * pixelDeltaX
                                + (y + offset.Y) * pixelDeltaY;
        
        Point3 rayOrig = centre;
        Vector3 rayDir = pixelSample - rayOrig;

        return new Ray(rayOrig, rayDir);
    }

    public Vector3 EyePoint
    {
        get { return centre;}
        set { centre = value;}
    }

    public double FocalLength
    {
        get {return focalLength;}
        set {focalLength = value;}
    }

    public void Render(Hittable world, ImageWriter writer)
    {
        for (int y = 0; y < imageHeight; y++)
        {
            Console.Write("\rScanlines Remaining: " + (imageHeight-y) + "      ");
            for (int x = 0; x < imageWidth; x++)
            {
                Color pixelColor = new(0, 0, 0);
                for (int sample = 0; sample < samplesPerPixel; sample++)
                {
                    Ray ray = GetRay(x, y);
                    pixelColor += RayColor(ray, world, bounceDepth) * pixelSampleScale;
                }
                writer.WritePixel(x, pixelColor);
            }
        }
        Console.WriteLine("\rDone.                           ");
    }
}