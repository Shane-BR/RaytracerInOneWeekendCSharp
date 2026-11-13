using System;


namespace Raytracer.Utils;

internal struct Color
{
    private double r, g, b;

    public Color(double _r, double _g, double _b)
    {
        if (_r > 1.0 || _g > 1.0 || _b > 1.0 )
        {
            DoubleInterval interval = new(0.0, 1.0);
            _r = interval.Clamp(_r);
            _g = interval.Clamp(_g);
            _b = interval.Clamp(_b);
        }

        r = _r;
        g = _g;
        b = _b;
    }

    public readonly double R { get {return r;} }
    public readonly double G { get {return g;} }
    public readonly double B { get {return b;} }

    public void SetColor(double _r, double _b, double _g)
    {
        r = _r;
        g = _g;
        b = _b;
    }

    public static Color Random()
    {
        return new Color(RandomDouble.Get(), RandomDouble.Get(), RandomDouble.Get());
    }

    public static Color Random(double max, double min)
    {
        max = Math.Clamp(max, 0, 1);
        min = Math.Clamp(min, 0, 1);

        return new Color(
                        RandomDouble.Get(min, max), 
                        RandomDouble.Get(min, max), 
                        RandomDouble.Get(min, max));
    }

    public static Color Black()
    {
        return new Color(0, 0, 0);
    }

    public static Color White()
    {
        return new Color(1, 1, 1);
    }

    public static Color operator +(Color a, Color b)
    {
        return new Color(a.R + b.R, a.G + b.G, a.B + b.B);
    }

    public static Color operator -(Color a, Color b)
    {
        return new Color(a.R - b.R, a.G - b.G, a.B - b.B);
    }

    public static Color operator *(Color a, double scalar)
    {
        return new Color(a.R*scalar, a.G*scalar, a.B*scalar);
    }

    public static Color operator *(double scalar, Color a)
    {
        return a * scalar;
    }

    public static Color operator *(Color a, Color b)
    {
        return new Color(a.R * b.R, a.G * b.G, a.B * b.B);
    }

    public static Color operator /(Color a, double scalar)
    {
        return new Color(a.R/scalar, a.G/scalar, a.B/scalar);
    }
}