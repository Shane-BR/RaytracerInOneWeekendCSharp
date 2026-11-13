using Raytracer.Utils;

namespace Raytracer.Core;

abstract class Material
{
    public abstract bool Scatter(Ray ray, HitRecord record, out Color attenuation, out Ray scattered);
}

class Lambertian(Color _albedo) : Material
{
    private readonly Color albedo = _albedo;

    public override bool Scatter(Ray ray, HitRecord record, out Color attenuation, out Ray scattered)
    {
        Vector3 scatterDir = record.normal + Vector3.RandomUnitVector();

        // Catch degenerate scatter direction
        if (scatterDir.NearZero())
        {
            scatterDir = record.normal;
        }

        scattered = new Ray(record.at, scatterDir);
        attenuation = albedo;
        return true;
    }
}

class Metal(Color _albedo, double _fuzz) : Material
{
    private readonly Color albedo = _albedo;
    private readonly double fuzz = _fuzz < 1 ? _fuzz : 1;

    public override bool Scatter(Ray ray, HitRecord record, out Color attenuation, out Ray scattered)
    {
        Vector3 reflected = Vector3.Reflect(ray.Direction, record.normal);
        reflected = Vector3.UnitVector(reflected) + (fuzz * Vector3.RandomUnitVector());
        scattered = new Ray(record.at, reflected);
        attenuation = albedo;
        return Vector3.Dot(scattered.Direction, record.normal) > 0;
    }
}

class Dielectric(double _refractionIndex) : Material
{
    private double refractionIndex = _refractionIndex;

    public override bool Scatter(Ray ray, HitRecord record, out Color attenuation, out Ray scattered)
    {
        attenuation = Color.White();
        double ri = record.frontFace ? (1.0/refractionIndex) : refractionIndex;

        Vector3 unitDir = Vector3.UnitVector(ray.Direction);
        double cosTheta = Math.Min(Vector3.Dot(-unitDir, record.normal), 1.0);
        double sinTheta = Math.Sqrt(1.0 - cosTheta*cosTheta);

        bool cannotRefract = ri * sinTheta > 1.0;

        Vector3 direction;
        if (cannotRefract || Reflectance(cosTheta, ri) > RandomDouble.Get())
        {
            direction = Vector3.Reflect(unitDir, record.normal);
        }
        else
        {
            direction = Vector3.Refract(unitDir, record.normal, ri);
        }

        scattered = new Ray(record.at, direction);

        return true;
    }

    private static double Reflectance(double cosine, double refractIndex)
    {
        double r0 = (1 - refractIndex) / (1 + refractIndex);
        r0 *= r0;
        return r0 + (1-r0)*Math.Pow((1 - cosine),5);
    }
}