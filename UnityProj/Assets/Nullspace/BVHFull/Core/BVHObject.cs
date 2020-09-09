
using UnityEngine;

namespace Nullspace
{
    public abstract class BVHObject
    {
        public abstract bool GetIntersection(ref BVHRay ray, ref BVHIntersectionInfo intersection);
        public abstract Vector3 GetNormal(ref BVHIntersectionInfo i);
        public abstract BVHBox GetBBox();
        public abstract Vector3 GetCentroid();

    }
}
