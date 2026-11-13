global using Point3 = Raytracer.Utils.Vector3;

using System;

namespace Raytracer.Utils;

internal readonly struct Vector3(double _x, double _y, double _z)
{
    private readonly double x = _x, y = _y, z = _z;

    public double X { get {return x;} }
    public double Y { get {return y;} }
    public double Z { get {return z;} }

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

    private static Vector3 Random()
    {
        return new Vector3(
            RandomDouble.Get(), 
            RandomDouble.Get(), 
            RandomDouble.Get());
    }

    private static Vector3 Random(double min, double max)
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

    public static Vector3 RandomInUnitDisk()
    {
        while (true)
        {
            Vector3 v = new(RandomDouble.Get(-1, 1), RandomDouble.Get(-1, 1), 0);
            if (v.GetLengthSquared() < 1)
                return v;
        }
    }

    public static Vector3 Reflect(Vector3 vec, Vector3 surfaceNormal)
    {
        // Reversing 2 times the direction of the projection of vec onto surfaceNormal
        return vec - 2*Dot(vec,surfaceNormal)*surfaceNormal;
    }

    public static Vector3 Refract(Vector3 vec, Vector3 surfaceNormal, double etaiOverEtat)
    {
        double cosTheta = Math.Min(Dot(-vec, surfaceNormal), 1.0);
        Vector3 rOutPerp = etaiOverEtat * (vec + cosTheta*surfaceNormal);
        Vector3 rOutParallel = -Math.Sqrt(Math.Abs(1.0 - rOutPerp.GetLengthSquared())) * surfaceNormal;
        return rOutPerp + rOutParallel;
    }
}

internal readonly struct Ray(Point3 origin, Vector3 direction)
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

internal struct DoubleInterval(double _min, double _max)
{
    private double max = _max;

    public double Min { get; set; } = _min;

    public double Max
    { 
        get { return max; }
        set { max = value; }
    }

    public readonly double Size()
    {
        return max - Min;
    }

    public readonly bool Contains(double x)
    {
        return Min <= x && x <= max;
    }

    public readonly bool Surrounds(double x)
    {
        return Min < x && x < max;
    }

    public readonly double Clamp(double x)
    {
        if (x < Min) return Min;
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