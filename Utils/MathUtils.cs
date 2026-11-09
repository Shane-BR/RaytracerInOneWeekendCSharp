global using Point3 = Raytracer.Utils.Vector3;

using System;

namespace Raytracer.Utils;

class Vector3(double x, double y, double z)
{
    private readonly double[] elems = [x, y, z];

    public double X { get {return elems[0];} }
    public double Y { get {return elems[1];} }
    public double Z { get {return elems[2];} }

    public double GetLengthSquared()
    {
        return X*X + Y*Y + Z*Z;
    }

    public double GetLength()
    {
        return Math.Sqrt(GetLengthSquared());
    }

    public static Vector3 operator +(Vector3 a, Vector3 b)
    {
        return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    public static Vector3 operator -(Vector3 a, Vector3 b)
    {
        return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }

    public static Vector3 operator -(Vector3 a)
    {
        return new Vector3(-a.X, -a.Y, -a.Z);
    }

    public static Vector3 operator *(Vector3 a, double scalar)
    {
        return new Vector3(a.X*scalar, a.Y*scalar, a.Z*scalar);
    }

    public static Vector3 operator *(double scalar, in Vector3 a)
    {
        return a * scalar;
    }

    public static Vector3 operator /(Vector3 a, double scalar)
    {
        return new Vector3(a.X/scalar, a.Y/scalar, a.Z/scalar);
    }

    public static double Dot(Vector3 a, Vector3 b)
    {
        return a.X*b.X + a.Y*b.Y + a.Z*b.Z;
    }

    /*
        Returns a vector that is perpendicular (orthogonal) to both a and b.
    */
    public static Vector3 Cross(Vector3 a, Vector3 b)
    {
        return new Vector3(
                            a.Y*b.Z - a.Z*b.Y,
                            a.Z*b.X - a.X*b.Z,
                            a.X*b.Y - a.Y*b.X);
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

    public Point3 Origin { get {return orig;} }

    public Vector3 Direction { get { return dir; } }

    public Point3 At(double scalar)
    {
        return orig + scalar*dir;
    }
}

class DoubleInterval(double _min, double _max)
{
    private double min = _min, max = _max;

    public double Min 
    { 
        get { return min; }
        set { min = value; }
    }

    public double Max
    { 
        get { return max; }
        set { max = value; }
    }

    public double Size()
    {
        return max - min;
    }

    public bool Contains(double x)
    {
        return min <= x && x <= max;
    }

    public bool Surrounds(double x)
    {
        return min < x && x < max;
    }

    public static readonly DoubleInterval Empty = 
        new(double.PositiveInfinity, double.NegativeInfinity);

    public static readonly DoubleInterval universe = 
        new(double.NegativeInfinity, double.PositiveInfinity);
}