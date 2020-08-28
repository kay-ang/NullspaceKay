
namespace Nullspace
{
    public class BoolAndValueStack : ValueStackGeneric<bool>
    {
        protected override bool Add(bool l, bool r)
        {
            return l && r;
        }

        protected override void ResetDelta()
        {
            // nothing
        }
    }
}