
using UnityEngine;

namespace Nullspace
{
    public class PositionValueStack : ValueStackGeneric<Vector3>
    {
        protected override Vector3 Add(Vector3 l, Vector3 r)
        {
            return l + r;
        }

        protected override void ResetDelta()
        {
            Delta = Value - mLastValue;
        }
    }
}
