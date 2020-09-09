
using UnityEngine;

namespace Nullspace
{
    public class BVHTriangleObject : BVHObject
    {
        public Vector3 P1;
        public Vector3 P2;
        public Vector3 P3;
        public Vector3 Center;
        public BVHBox Box;
        public Vector3 Normal;
        public BVHTriangleObject(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
            Center = (P1 + P2 + P3) * 0.33333f;
            Vector3 min = Vector3.Min(Vector3.Min(P1, P2), P3);
            Vector3 max = Vector3.Max(Vector3.Max(P1, P2), P3);
            Box = new BVHBox(min, max);
            Normal = Vector3.Cross(P2 - P1, P3 - P1);
            Normal.Normalize();
        }

        
        public override bool GetIntersection(ref BVHRay ray, ref BVHIntersectionInfo intersection)
        {
            Vector3 edge1, edge2, tvec, pvec, qvec;  
            double det, inv_det;
            double t;
            //find vectors for two edges sharing vert0  
            edge1 = P2 - P1;
            edge2 = P3 - P1;
            // begin calculating determinant - also used to calculate U parameter  
            pvec = Vector3.Cross(ray.Direction, edge2);
            // if determinant is near zero, ray lies in plane of triangle  
            det = Vector3.Dot(edge1, pvec);  
  
#if TEST_CULL // define TEST_CULL if culling is desired  
            if (det < 1e-5f)  
                return false;  
            tvec = ray.mOrigin - mP1;
            // calculate U parameter and test bounds  
            double u = Vector3.Dot(tvec, pvec); 
            if (u < 0.0 || u > det)  
                return false;  
  
            // prepare to test V parameter  
            qvec = Vector3.Cross(tvec, edge1);  
            // calculate V parameter and test bounds  
            double v = Vector3.Dot(ray.mDirection, qvec);  
            if (v < 0.0 || u +v > det)  
                return false;  
            // calculate t, scale parameters, ray intersects triangle  
            t = Vector3.Dot(edge2, qvec);
            inv_det = 1.0 / det;  
            t *= inv_det;  
            u *= inv_det;  
            v *= inv_det;  
#else           // the non-culling branch  
            if (det > -1e-5f && det < 1e-5f)  
                return false;  
            inv_det = 1.0 / det;  
  
            // calculate distance from vert0 to ray origin   
            tvec = ray.Origin - P1;
            // calculate U parameter and test bounds  
            double u = Vector3.Dot(tvec, pvec) * inv_det;  
            if (u < 0.0 || u > 1.0)  
                return false;  
  
            // prepare to test V parameters  
            qvec = Vector3.Cross(tvec, edge1);  
  
            // calculate V paremeter and test bounds  
            double v = Vector3.Dot(ray.Direction, qvec) * inv_det;  
            if (v < 0.0 || u + v > 1.0)  
                return false;  
  
            //calculate t, ray intersects triangle  
            t = Vector3.Dot(edge2, qvec) * inv_det;  
#endif  
            intersection.Length = (float)t;
            intersection.Object = this;
            return true;
        }
         
        
        public override Vector3 GetNormal(ref BVHIntersectionInfo i)
        {
            return Normal;
        }

        
        public override BVHBox GetBBox()
        {
            return Box;
        }

        
        public override Vector3 GetCentroid()
        {
            return Center;
        }
    }
}
