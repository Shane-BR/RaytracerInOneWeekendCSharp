using System;


namespace Raytracer.Utils;

class Color
{
    private readonly double[] elems = new double[3];

    public Color(double r, double g, double b)
    {
        if (r > 1.0 || g > 1.0 || b > 1.0 )
        {
            DoubleInterval interval = new(0.0, 1.0);
            r = interval.Clamp(r);
            g = interval.Clamp(g);
            b = interval.Clamp(b);
        }

        elems[0] = r;
        elems[1] = g;
        elems[2] = b;
    }

    public double R { get {return elems[0];} }
    public double G { get {return elems[1];} }
    public double B { get {return elems[2];} }

    public static Color operator +(in Color a, in Color b)
    {
        return new Color(a.R + b.R, a.G + b.G, a.B + b.B);
    }

    public static Color operator -(in Color a, in Color b)
    {
        return new Color(a.R - b.R, a.G - b.G, a.B - b.B);
    }

    public static Color operator *(in Color a, double scalar)
    {
        return new Color(a.R*scalar, a.G*scalar, a.B*scalar);
    }

    public static Color operator *(double scalar, in Color a)
    {
        return a * scalar;
    }

    public static Color operator /(in Color a, double scalar)
    {
        return new Color(a.R/scalar, a.G/scalar, a.B/scalar);
    }
}