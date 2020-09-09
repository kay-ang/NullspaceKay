
using UnityEngine;

namespace Nullspace
{
    public class BVHAABBObject : BVHObject
    {
        public BVHBox AABB;
        public BVHAABBObject(Vector3 min, Vector3 max)
        {
            AABB = new BVHBox(min, max);
        }
        
        public override bool GetIntersection(ref BVHRay ray, ref BVHIntersectionInfo intersection)
        {
            float near = 0.0f;
            float far = 0.0f;
            bool isect = AABB.Intersect(ray, ref near, ref far);
            if (isect)
            {
                intersection.Object = this;
                intersection.HitPoint = ray.Origin + ray.Direction * near;
                intersection.Length = near;
            }
            return isect;
        }

        // here not debug test
        
        public override Vector3 GetNormal(ref BVHIntersectionInfo i)
        {
            Vector3 v = i.HitPoint - AABB.Min;
            if (v.x == 0.0f || v.y == 0.0f || v.z == 0.0f)
            {
                if (v.x == 0.0f)
                {
                    return Vector3.left;
                }
                else if (v.y == 0.0f)
                {
                    return Vector3.down;
                }
                else if (v.z == 0.0f)
                {
                    return Vector3.back;
                }
            }
            else
            {
                if (v.x == 0.0f)
                {
                    return Vector3.right;
                }
                else if (v.y == 0.0f)
                {
                    return Vector3.up;
                }
                else if (v.z == 0.0f)
                {
                    return Vector3.forward;
                }
            }
            // won't be exist
            return Vector3.zero;
        }

        
        public override BVHBox GetBBox()
        {
            return AABB;
        }

        
        public override Vector3 GetCentroid()
        {
            return (AABB.Min + AABB.Max) * 0.5f;
        }
    }
}
