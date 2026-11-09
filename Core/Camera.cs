using Raytracer.Utils;

namespace Raytracer.Core;

class Camera
{
    private Point3 eyePoint = new(0, 0, 0);
    private double focalLength = 1.0;

    public Vector3 GetEyePoint()
    {
        return eyePoint;
    }

    public double GetFocalLength()
    {
        return focalLength;
    }
}