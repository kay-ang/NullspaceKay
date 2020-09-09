using UnityEngine;

namespace Nullspace
{
    public class BVHBox
    {
        public Vector3 Min = Vector3.zero;
        public Vector3 Max = Vector3.zero;
        public Vector3 ExtentSize = Vector3.zero;

        public BVHBox()
        {

        }

        public BVHBox(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
            ExtentSize = Max - Min;
        }

        public BVHBox(Vector3 p)
        {
            Min = p;
            Max = p;
            ExtentSize = Max - Min;
        }

        public bool Intersect(BVHRay ray, ref float dnear, ref float dfar)
        {
            // Intermediate calculation variables.
            float tmin = 0.0f;
            float tmax = 0.0f;
            // X direction.
            float div = ray.InvDirection.x;
            if (div >= 0.0f)
            {
                tmin = (Min.x - ray.Origin.x) * div;
                tmax = (Max.x - ray.Origin.x) * div;
            }
            else
            {
                tmin = (Max.x - ray.Origin.x) * div;
                tmax = (Min.x - ray.Origin.x) * div;
            }
            dnear = tmin;
            dfar = tmax;
    
            // Check if the ray misses the box.
            if (dnear > dfar || dfar < 0.0f)
            {
                return false;
            }
            // Y direction.
            div = ray.InvDirection.y;
            if (div >= 0.0f)
            {
                tmin = (Min.y - ray.Origin.y) * div;
                tmax = (Max.y - ray.Origin.y) * div;
            }
            else
            {
                tmin = (Max.y - ray.Origin.y) * div;
                tmax = (Min.y - ray.Origin.y) * div;
            }
            // Update the near and far intersection distances.
            if (tmin > dnear)
            {
                dnear = tmin;
            }
            if (tmax < dfar)
            {
                dfar = tmax;
            }
            // Check if the ray misses the box.
            if (dnear > dfar || dfar < 0.0f)
            {
		        return false;
            }
            // Z direction.
            div = ray.InvDirection.z;
            if (div >= 0.0f)
            {
                tmin = (Min.z - ray.Origin.z) * div;
                tmax = (Max.z - ray.Origin.z) * div;
            }
            else
            {
                tmin = (Max.z - ray.Origin.z) * div;
                tmax = (Min.z - ray.Origin.z) * div;
            }
            // Update the near and far intersection distances.
            if (tmin > dnear)
            {
                dnear = tmin;
            }
            if (tmax < dfar)
            {
                dfar = tmax;
            }
            // Check if the ray misses the box.
            if (dnear > dfar || dfar < 0.0f)
            {
                return false;
            }
            return true;
        }

        public void ExpandToInclude(Vector3 p)
        {
            Min = Vector3.Min(Min, p);
            Max = Vector3.Max(Max, p);
            ExtentSize = Max - Min;
        }

        public void ExpandToInclude(BVHBox b)
        {
            Min = Vector3.Min(Min, b.Min);
            Max = Vector3.Max(Max, b.Max);
            ExtentSize = Max - Min;
        }

        public int MaxDimension()
        {
            int result = 0;
            if (ExtentSize.y > ExtentSize.x)
            {
                result = 1;
                if (ExtentSize.z > ExtentSize.y)
                {
                    result = 2;
                }
            }
            else
            {
                if (ExtentSize.z > ExtentSize.x)
                {
                    result = 2;
                }
            }
            return result;
        }

        public float SurfaceArea()
        {
            return 2.0f * (ExtentSize.x * ExtentSize.z + ExtentSize.x * ExtentSize.y + ExtentSize.y * ExtentSize.z);
        }
    }
}
