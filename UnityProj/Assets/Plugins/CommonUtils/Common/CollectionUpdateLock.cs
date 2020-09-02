
namespace Nullspace
{
    public class CollectionUpdateLock
    {
        protected bool mUpdateLocker = false;

        protected void LockUpdate()
        {
            mUpdateLocker = true;
        }

        protected void UnLockUpdate()
        {
            mUpdateLocker = false;
        }

        protected bool IsLockUpdate()
        {
            return mUpdateLocker;
        }
    }
}
