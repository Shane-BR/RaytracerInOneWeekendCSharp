using System;

namespace Raytracer.Utils;

using Point3 = Vector3;

class Vector3(double x, double y, double z)
{
    private readonly double[] elems = [x, y, z];

    public double X() { return elems[0]; }
    public double Y() { return elems[1]; }
    public double Z() { return elems[2]; }

    public double LengthSquared()
    {
        return X()*X() + Y()*Y() + Z()*Z();
    }

    public double Length()
    {
        return Math.Sqrt(LengthSquared());
    }

    public static Vector3 operator +(in Vector3 a, in Vector3 b)
    {
        return new Vector3(a.X() + b.X(), a.Y() + b.Y(), a.Z() + b.Z());
    }

    public static Vector3 operator -(in Vector3 a, in Vector3 b)
    {
        return new Vector3(a.X() - b.X(), a.Y() - b.Y(), a.Z() - b.Z());
    }

    public static Vector3 operator *(in Vector3 a, double scalar)
    {
        return new Vector3(a.X()*scalar, a.Y()*scalar, a.Z()*scalar);
    }

    public static Vector3 operator *(double scalar, in Vector3 a)
    {
        return a * scalar;
    }

    public static Vector3 operator /(in Vector3 a, double scalar)
    {
        return new Vector3(a.X()/scalar, a.Y()/scalar, a.Z()/scalar);
    }

    public static double Dot(in Vector3 a, in Vector3 b)
    {
        return a.X()*b.X() + a.Y()*b.Y() + a.Z()*b.Z();
    }

    /*
        Returns a vector that is perpendicular (orthogonal) to both a and b.
    */
    public static Vector3 Cross(in Vector3 a, in Vector3 b)
    {
        return new Vector3(
                            a.Y()*b.Z() - a.Z()*b.Y(),
                            a.Z()*b.X() - a.X()*b.Z(),
                            a.X()*b.Y() - a.Y()*b.X());
    }

    public static Vector3 UnitVector(in Vector3 a)
    {
        return a / a.Length();
    }
}