using System.Diagnostics;
using Raytracer.Utils;

namespace Raytracer.Core;

class Camera
{
    private Point3 centre;
    private readonly int samplesPerPixel;
    private readonly double pixelSampleScale;
    private readonly int imageWidth, imageHeight;
    private readonly double aspectRatio;
    private readonly double vfov;
    private readonly int bounceDepth;

    private readonly Point3 lookFrom = new(0, 0, 0);
    private readonly Point3 lookAt = new(0, 0, -1);
    private readonly Vector3 vecUp = new(0, 1, 0);
    private readonly Vector3 basisU, basisV, basisW;

    private readonly Vector3 pixelDeltaX;
    private readonly Vector3 pixelDeltaY;
    private readonly Vector3 pixel00Loc;

    public Camera(Point3 _lookFrom, Point3 _lookAt, int _imageWidth, double _aspectRatio, double _vfov)
    {
        lookFrom = _lookFrom;
        lookAt = _lookAt;
        centre = lookFrom;
        samplesPerPixel = 20;
        pixelSampleScale = 1.0 / samplesPerPixel;
        imageWidth = _imageWidth;
        aspectRatio = _aspectRatio;
        vfov = _vfov;
        bounceDepth = 10;
        imageHeight = 
            ((int)(imageWidth / aspectRatio) < 1) ? 1 
            : (int)(imageWidth / aspectRatio);
        
        double focalLength = (lookFrom - lookAt).GetLength();
        double theta = (Math.PI * vfov) / 180.0;
        double h = Math.Tan(theta/2);
        double viewportHeight = 2.0 * h * focalLength;
        double viewportWidth = viewportHeight * (double)imageWidth/imageHeight;

        basisW = Vector3.UnitVector(lookFrom - lookAt);
        basisU = Vector3.UnitVector(Vector3.Cross(vecUp, basisW));
        basisV = Vector3.Cross(basisW, basisU);

        Vector3 viewportXDir = viewportWidth * basisU;
        Vector3 viewportYDir = viewportHeight * -basisV;
        pixelDeltaX = viewportXDir / imageWidth;
        pixelDeltaY = viewportYDir / imageHeight;

        Vector3 viewportUpperLeft = centre
                                        - (focalLength * basisW)
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

        /* 
            Floating point imprecision makes it possible that the origin of the bouncing
            ray is slightly different from the point of intersection with the surface.
            This may cause the resulting bounce ray to originate from below the surface,
            and subsequently hit it again at a very close distance.
            
            Use 0.001 min distance to avoid hitting the same surface on bounce.
        */ 
        if (world.Hit(ray, new DoubleInterval(0.001, double.PositiveInfinity), out HitRecord record))
        {
            if (record.material.Scatter(ray, record, out Color attenuation, out Ray scattered))
            {
                return attenuation * RayColor(scattered, world, bDepth - 1);
            }

            return Color.Black();
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

    public Vector3 Centre
    {
        get { return centre;}
        set { centre = value;}
    }

    public void Render(Hittable world, ImageWriter writer)
    {
        // Timer
        Stopwatch stopWatch = new();
        stopWatch.Start();

        for (int y = 0; y < imageHeight; y++)
        {
            double percentRendered = (int)(((double)y/imageHeight) * 100);
            Console.Write("\rRendering: " + percentRendered + "%      ");
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
        stopWatch.Stop();
        TimeSpan ts = stopWatch.Elapsed;

        string elapsedTime = 
            String.Format("{0:00}:{1:00}:{2:00}.{3:00}s",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);        
        
        Console.WriteLine("\rDone.                           ");
        Console.WriteLine("Time to Render Scene: " + elapsedTime);
    }
}