
using UnityEngine;

namespace Nullspace
{
    public class Plane
    {
        public Vector3 mNormal;
        public float mD;
        public Vector3 mAxisX;
        public Vector3 mAxisZ;

        private Matrix4x4 mMat;
        private Matrix4x4 mInvMat;
        private Vector3 mPoint;

        public Plane(Vector3 normal, float d)
        {
            mNormal = normal;
            mD = d;
        }

        /// p1 p2 p3 can not be on the same line
        public Plane(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            mAxisX = p2 - p1;
            mAxisX.Normalize();
            mAxisZ = p3 - p1;
            mAxisZ.Normalize();
            mNormal = Vector3.Cross(mAxisX, mAxisZ);
            mNormal.Normalize();
            mPoint = p1;
            mD = -Vector3.Dot(mNormal, p1);
            mAxisX = Vector3.Cross(mAxisZ, mNormal);
            mAxisX.Normalize();
            Quaternion q = Quaternion.FromToRotation(Vector3.up, mNormal);
            mMat = Matrix4x4.TRS(mPoint, q, Vector3.one);
            mInvMat = mMat.inverse;
        }

        // must 左手坐标系
        public Plane(Vector3 axisX, Vector3 axisY, Vector3 axisZ, Vector3 p)
        {
            mAxisX = axisX;
            mNormal = axisY;
            mAxisZ = axisZ;
            mPoint = p;
            Quaternion q = Quaternion.FromToRotation(Vector3.up, mNormal);
            Vector3 tmp = q * Vector3.right;
            q = Quaternion.FromToRotation(tmp, mAxisX) * q;
            mMat = Matrix4x4.TRS(mPoint, q, Vector3.one);
            mInvMat = mMat.inverse;
        }

        public void Normalize()
        {
            float len = mNormal.magnitude;
            mNormal.Normalize();
            mD = mD / len;
        }

        public void ReverseDirection()
        {
            mNormal = -mNormal;
            mD = -mD;
        }

        public Vector3 TransformToLocal(Vector3 p)
        {
            return mInvMat.MultiplyVector(p);
        }

        public Vector3 TransformToGlobal(Vector3 p)
        {
            return mMat.MultiplyPoint(p);
        }

        public void TransformToGlobal(Vector3 p, out Vector2 p2)
        {
            Vector3 pt = mMat.MultiplyPoint(p);
            p2 = new Vector2(pt.x, pt.z);
        }


    }
}
