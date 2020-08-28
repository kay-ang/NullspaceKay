
using UnityEngine;

namespace Nullspace
{
    public class QuaternionValueStack : ValueStackGeneric<Quaternion>
    {
        protected override Quaternion Add(Quaternion l, Quaternion r)
        {
            return l * r;
        }

        protected override void ResetDelta()
        {
            Delta = Value * Quaternion.Inverse(mLastValue);
        }
    }
}
