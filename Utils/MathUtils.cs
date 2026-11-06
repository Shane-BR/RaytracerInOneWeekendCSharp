global using Point3 = Raytracer.Utils.Vector3;

using System;

namespace Raytracer.Utils;

class Vector3(double x, double y, double z)
{
    private readonly double[] elems = [x, y, z];

    public double GetX() { return elems[0]; }
    public double GetY() { return elems[1]; }
    public double GetZ() { return elems[2]; }

    public double GetLengthSquared()
    {
        return GetX()*GetX() + GetY()*GetY() + GetZ()*GetZ();
    }

    public double GetLength()
    {
        return Math.Sqrt(GetLengthSquared());
    }

    public static Vector3 operator +(Vector3 a, Vector3 b)
    {
        return new Vector3(a.GetX() + b.GetX(), a.GetY() + b.GetY(), a.GetZ() + b.GetZ());
    }

    public static Vector3 operator -(Vector3 a, Vector3 b)
    {
        return new Vector3(a.GetX() - b.GetX(), a.GetY() - b.GetY(), a.GetZ() - b.GetZ());
    }

    public static Vector3 operator *(Vector3 a, double scalar)
    {
        return new Vector3(a.GetX()*scalar, a.GetY()*scalar, a.GetZ()*scalar);
    }

    public static Vector3 operator *(double scalar, in Vector3 a)
    {
        return a * scalar;
    }

    public static Vector3 operator /(Vector3 a, double scalar)
    {
        return new Vector3(a.GetX()/scalar, a.GetY()/scalar, a.GetZ()/scalar);
    }

    public static double Dot(Vector3 a, Vector3 b)
    {
        return a.GetX()*b.GetX() + a.GetY()*b.GetY() + a.GetZ()*b.GetZ();
    }

    /*
        Returns a vector that is perpendicular (orthogonal) to both a and b.
    */
    public static Vector3 Cross(Vector3 a, Vector3 b)
    {
        return new Vector3(
                            a.GetY()*b.GetZ() - a.GetZ()*b.GetY(),
                            a.GetZ()*b.GetX() - a.GetX()*b.GetZ(),
                            a.GetX()*b.GetY() - a.GetY()*b.GetX());
    }

    public static Vector3 UnitVector(Vector3 a)
    {
        return a / a.GetLength();
    }
}

class Ray(Point3 origin, Vector3 direction)
{
    private readonly Point3 orig = origin;
    private readonly Vector3 dir = direction;

    public Point3 GetOrigin()
    {
        return orig;
    }

    public Vector3 GetDirection()
    {
        return dir;
    }
}