using Raytracer.Utils;

namespace Raytracer.Core
{
    abstract class Hittable
    {
        public abstract bool Hit(Ray ray, double rayScalarMin, double rayScalarMax, out HitRecord? record);
    }

    class HitRecord
    {
        public Point3? at;
        public Vector3? normal;
        public double scalar;
    }

    class Sphere(Point3 _centre, double _radius) : Hittable
    {

        private readonly Point3 centre = _centre;
        private readonly double radius = _radius > 0 ? _radius : 0;

        public override bool Hit(Ray ray, double rayScalarMin, double rayScalarMax, out HitRecord? record)
        {
            Vector3 oc = centre - ray.GetOrigin();
            double a = Vector3.Dot(ray.GetDirection(), ray.GetDirection());
            double h = Vector3.Dot(ray.GetDirection(), oc);
            double c = oc.GetLengthSquared() - radius*radius; // Dot(A, A) = |A|^2
            double discriminant = h*h - 4*a*c;
        
            record = null;

            if (discriminant < 0)
                return false;

            // Find nearest root in given range
            double sqrtd = Math.Sqrt(discriminant);
            double root = (h - sqrtd) / a;
            if (root <= rayScalarMin || root >= rayScalarMax)
            {
                root = (h + sqrtd) / a;
                if (root <= rayScalarMin || root >= rayScalarMax)
                    return false;
            }

            record = new HitRecord();
            
            record.scalar = root;
            record.at = ray.At(record.scalar);
            record.normal = (record.at - centre) / radius;

            return true;
        }
    }
}