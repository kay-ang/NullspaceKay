
using UnityEngine;

namespace Nullspace
{
    public class ColorValueStack : ValueStackGeneric<Color>
    {
        protected override Color Add(Color l, Color r)
        {
            return l + r;
        }

        protected override void ResetDelta()
        {
            Delta = Value - mLastValue;
        }
    }
}
