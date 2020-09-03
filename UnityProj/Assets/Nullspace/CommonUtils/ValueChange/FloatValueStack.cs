
namespace Nullspace
{
    public class FloatValueStack : ValueStackGeneric<float>
    {
        protected override float Add(float l, float r)
        {
            return l + r;
        }

        protected override void ResetDelta()
        {
            Delta = Value - mLastValue;
        }
    }
}
