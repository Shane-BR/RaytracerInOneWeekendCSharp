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