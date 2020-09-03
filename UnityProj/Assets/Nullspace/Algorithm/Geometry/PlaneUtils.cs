
using System.Collections.Generic;
using UnityEngine;

namespace Nullspace
{
    public class PlaneUtils
    {
        public static Plane CreateFromTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return new Plane(p1, p2, p3);
        }

        public static Plane CreateFromRectangle(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            Vector3 ax = (p2 - p1).normalized;
            Vector3 az = (p3 - p1).normalized;
            Vector3 ay = Vector3.Cross(ax, az);
            ay.Normalize();
            ay = -ay;
            return new Plane(ax, ay, az, p1);
        }

        public static Vector3 CalculateNormal(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Vector3 p12 = p2 - p1;
            Vector3 p13 = p3 - p1;
            return Vector3.Cross(p12, p13);
        }

        public static float PointDistanceToPlane(Vector3 normal, float d, Vector3 p)
        {
            // float dot = Vector3.Dot(plane.mNormal, point - plane.mPoint);
            // return dot; 
            // or
            float dot = Vector3.Dot(normal, p);
            return dot + d;
        }

        public static float PointClosestToPlaneAbs(Vector3 normal, float d, Vector3 p, ref Vector3 close)
        {
            float dot = PointDistanceToPlane(normal, d, p);
            close = p - dot * normal;
            return Mathf.Abs(dot);
        }

        public static float PointDistanceToPlaneAbs(Vector3 normal, float d, Vector3 p)
        {
            float dot = Vector3.Dot(normal, p);
            return Mathf.Abs(dot + d);
        }

        public static float PointDistanceToPlane(Plane plane, Vector3 p)
        {
            return PointDistanceToPlane(plane.mNormal, plane.mD, p);
        }

        public static bool IsPointOnPlane(Vector3 normal, float d, Vector3 p)
        {
            float dist = PointDistanceToPlane(normal, d, p);
            return dist > -1e-5f && dist < 1e-5f;
        }

        public static bool IsPointInPositiveHalf(Vector3 normal, float d, Vector3 p)
        {
            float d1 = PointDistanceToPlane(normal, d, p);
            return d1 > 1e-5f;
        }

        public static bool IsPointInNegativeHalf(Vector3 normal, float d, Vector3 p)
        {
            float d1 = PointDistanceToPlane(normal, d, p);
            return d1 < -1e-5f;
        }

        public static bool IsPointInPositiveHalf(Plane plane, Vector3 p)
        {
            float d = PointDistanceToPlane(plane, p);
            return d > 1e-5f;
        }

        public static Vector3 GetPointFromPlane(Vector3 normal, float d)
        {
            Vector3 point = new Vector3(0, 0, 0);
            int axis = 0;
            if (normal[0] != 0)
                axis = 0;
            if (normal[1] != 0)
                axis = 1;
            if (normal[2] != 0)
                axis = 2;
            point[axis] = d / normal[axis];
            return point;
        }

        public static void PlaneTransformLocalTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Plane plane, out Vector2 p11, out Vector2 p21, out Vector2 p31)
        {
            Vector3 p1t = plane.TransformToLocal(p1);
            Vector3 p2t = plane.TransformToLocal(p2);
            Vector3 p3t = plane.TransformToLocal(p3);
            p11 = new Vector2(p1t.x, p1t.z);
            p21 = new Vector2(p2t.x, p2t.z);
            p31 = new Vector2(p3t.x, p3t.z);
        }
        public static void PlaneTransformGlobaleTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Plane plane, out Vector3 p11, out Vector3 p21, out Vector3 p31)
        {
            p11 = plane.TransformToGlobal(p1);
            p21 = plane.TransformToGlobal(p2);
            p31 = plane.TransformToGlobal(p3);
        }

        public static void PlaneTransformGlobaleTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Plane plane, out Vector2 p11, out Vector2 p21, out Vector2 p31)
        {
            Vector3 p1t = plane.TransformToGlobal(p1);
            Vector3 p2t = plane.TransformToGlobal(p2);
            Vector3 p3t = plane.TransformToGlobal(p3);
            p11 = new Vector2(p1t.x, p1t.z);
            p21 = new Vector2(p2t.x, p2t.z);
            p31 = new Vector2(p3t.x, p3t.z);
        }

        public static List<Vector3> PlaneTransformGlobal(List<Vector3> ps, Plane plane)
        {
            List<Vector3> tmp = new List<Vector3>();
            foreach (Vector3 p in ps)
            {
                Vector3 v = new Vector3(p.x, 0.0f, p.y);
                tmp.Add(plane.TransformToGlobal(v));
            }
            return tmp;
        }

        public static List<Vector3> PlaneTransformGlobal(List<Vector2> ps, Plane plane)
        {
            List<Vector3> tmp = new List<Vector3>();
            foreach (Vector3 p in ps)
            {
                Vector3 v = new Vector3(p.x, 0.0f, p.y);
                tmp.Add(plane.TransformToGlobal(v));
            }
            return tmp;
        }

        public static List<Vector2> PlaneTransformLocal(List<Vector3> ps, Plane plane)
        {
            List<Vector2> tmp = new List<Vector2>();
            foreach (Vector3 p in ps)
            {
                Vector3 p1 = plane.TransformToLocal(p);
                tmp.Add(new Vector2(p1.x, p1.z));
            }
            return tmp;
        }
    }
}
