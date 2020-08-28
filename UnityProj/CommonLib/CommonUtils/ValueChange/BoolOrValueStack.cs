
namespace Nullspace
{
    public class BoolOrValueStack : ValueStackGeneric<bool>
    {
        protected override bool Add(bool l, bool r)
        {
            return l || r;
        }

        protected override void ResetDelta()
        {
            // nothing
        }
    }
}