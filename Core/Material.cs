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

class Metal(Color _albedo) : Material
{
    private readonly Color albedo = _albedo;

    public override bool Scatter(Ray ray, HitRecord record, out Color attenuation, out Ray scattered)
    {
        Vector3 reflected = Vector3.Reflect(ray.Direction, record.normal);
        scattered = new Ray(record.at, reflected);
        attenuation = albedo;
        return true;
    }
}