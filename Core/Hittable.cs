using Raytracer.Utils;

namespace Raytracer.Core;

abstract class Hittable
{
    public abstract bool Hit(Ray ray, DoubleInterval rayScalarRange, out HitRecord? record);
}

class HitRecord
{
    public Point3 at;
    public Vector3 normal;
    public Material? material;
    public double scalar;
    public bool frontFace;

    public void SetFaceNormal(Ray ray, Vector3 outwardNormal)
    {
        frontFace = Vector3.Dot(ray.Direction, outwardNormal) < 0;
        normal = frontFace ? outwardNormal : -outwardNormal;
    }
}

class Sphere(Point3 _centre, double _radius, Material _material) : Hittable
{

    private readonly Point3 centre = _centre;
    private readonly double radius = _radius > 0 ? _radius : 0;
    private readonly Material material = _material;

    public override bool Hit(Ray ray, DoubleInterval rayScalarRange, out HitRecord? record)
    {
        Vector3 oc = centre - ray.Origin;
        double a = ray.Direction.GetLengthSquared(); // Dot(A, A) = |A|^2
        double h = Vector3.Dot(ray.Direction, oc);
        double c = oc.GetLengthSquared() - radius*radius;
        double discriminant = h*h - a*c;
    
        record = null;

        if (discriminant < 0)
            return false;

        // Find nearest root in given range
        double sqrtd = Math.Sqrt(discriminant);
        double root = (h - sqrtd) / a;
        if (!rayScalarRange.Surrounds(root))
        {
            root = (h + sqrtd) / a;
            if (!rayScalarRange.Surrounds(root))
                return false;
        }
        
        record = new();
        
        record.scalar = root;
        record.at = ray.At(record.scalar);
        record.SetFaceNormal(ray, (record.at - centre) / radius);
        record.material = material;

        return true;
    }
}

class HittableList : Hittable
{
    private readonly List<Hittable> objects = [];

    public void Add(Hittable obj)
    {
        objects.Add(obj);
    }

    public void Clear()
    {
        objects.Clear();
    }

    public override bool Hit(Ray ray, DoubleInterval rayScalarRange, out HitRecord? record)
    {
        bool hitAnything = false;
        double closestSoFar = rayScalarRange.Max;

        record = null;

        foreach (Hittable obj in objects)
        {
            if (obj.Hit(ray, new DoubleInterval(rayScalarRange.Min, closestSoFar), out HitRecord? temp))
            {
                if (temp == null)
                    continue;

                hitAnything = true;
                closestSoFar = temp.scalar;
                record = temp;
            }
        }

        return hitAnything;
    }
}