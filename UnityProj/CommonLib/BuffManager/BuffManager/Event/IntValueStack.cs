
namespace Nullspace
{
    public class IntValueStack : ValueStackGeneric<int>
    {
        protected override int Add(int l, int r)
        {
            return l + r;
        }

        protected override void ResetDelta()
        {
            Delta = Value - mLastValue;
        }
    }
}
