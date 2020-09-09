using UnityEngine;

namespace Nullspace
{
    public class BVHRay
    {
        public Vector3 Origin; 
        public Vector3 Direction;
        public Vector3 InvDirection;

        public BVHRay(Vector3 o, Vector3 d)
        {
            Origin = o;
            Direction = d;
            InvDirection = new Vector3(1 / d[0], 1 / d[1], 1 / d[2]);
        }
    }
}
