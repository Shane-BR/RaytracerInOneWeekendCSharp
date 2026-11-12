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

    public bool NearZero()
    {
        double threshold = 1e-8;
        return  (Math.Abs(X) < threshold) && 
                (Math.Abs(Y) < threshold) && 
                (Math.Abs(Z) < threshold);
    }

    public static Vector3 Random()
    {
        return new Vector3(
            RandomDouble.Get(), 
            RandomDouble.Get(), 
            RandomDouble.Get());
    }

    public static Vector3 Random(double min, double max)
    {
        return new Vector3(
            RandomDouble.Get(min, max), 
            RandomDouble.Get(min, max), 
            RandomDouble.Get(min, max));
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

    public static Vector3 RandomUnitVector()
    {
        while(true) // This raytracing stuff is no joke, huh?
        {
            Point3 randPoint = Point3.Random(-1, 1);
            double lenSqr = randPoint.GetLengthSquared();
            // Extremely small values of lenSqr may underflow to 0 when squared.
            // The norm vector will produce a nonsensical result if this happens.
            // So, exclude these values.
            if (1e-160 < lenSqr && lenSqr <= 1)
            {
                return randPoint / Math.Sqrt(lenSqr);
            }
        }
    }

    public static Vector3 RandomOnHemisphere(Vector3 normal)
    {
        Vector3 vec = RandomUnitVector();
        if (Dot(vec, normal) > 0)
        {
            return vec;
        }
        else
        {
            return -vec;
        }
    }

    public static Vector3 Reflect(Vector3 vec, Vector3 surfaceNormal)
    {
        // Reversing 2 times the direction of the projection of vec onto surfaceNormal
        return vec - 2*Dot(vec,surfaceNormal)*surfaceNormal;
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

    public double Clamp(double x)
    {
        if (x < min) return min;
        if (x > max) return max;
        return x;
    }

    public static readonly DoubleInterval Empty = 
        new(double.PositiveInfinity, double.NegativeInfinity);

    public static readonly DoubleInterval universe = 
        new(double.NegativeInfinity, double.PositiveInfinity);
}

class RandomDouble
{
    private static readonly Random rand = new();

    public static double Get()
    {
        return rand.NextDouble();
    }

    public static double Get(double min, double max)
    {
        return min + (max - min) * rand.NextDouble();
    }
}