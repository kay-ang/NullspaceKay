using UnityEngine;

namespace Nullspace
{
    public class BVHSphereObject : BVHObject
    {
        public Vector3 Center;
        public float Radius;
        public float Radius2;

        public BVHSphereObject(Vector3 center, float r)
        {
            Center = center;
            Radius = r;
            Radius2 = Radius * Radius;
        }

        
        public override bool GetIntersection(ref BVHRay ray, ref BVHIntersectionInfo intersection)
        {
            Vector3 s = Center - ray.Origin;
            float sd = Vector3.Dot(s, ray.Direction);
            float ss = s.magnitude * s.magnitude;
            float disc = sd * sd + Radius2 - ss;
            if (disc < 0.0f)
            {
                return false;
            }
            intersection.Object = this;
            intersection.Length = sd - Mathf.Sqrt(disc);
            return true;
        }


        public override Vector3 GetNormal(ref BVHIntersectionInfo i)
        {
            Vector3 nor = i.HitPoint - Center;
            nor.Normalize();
            return nor;
        }

        
        public override BVHBox GetBBox()
        {
            return new BVHBox(Center - new Vector3(Radius, Radius, Radius), Center + new Vector3(Radius, Radius, Radius)); ;
        }

        
        public override Vector3 GetCentroid()
        {
            return Center;
        }
    }
}
