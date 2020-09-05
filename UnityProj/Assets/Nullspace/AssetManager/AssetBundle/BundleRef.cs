
namespace Nullspace
{
    public class BundleRef
    {
        protected int mRefCount;
        protected bool mIsDestroyed;
        public BundleRef()
        {
            mRefCount = 0;
            mIsDestroyed = false;
        }

        public virtual int AddRef()
        {
            mRefCount++;
            return mRefCount;
        }

        public virtual bool DelRef()
        {
            mRefCount--;
            if (mRefCount <= 0)
            {
                mIsDestroyed = true;
                Destroy();
            }
            return mIsDestroyed;
        }

        public virtual void Destroy()
        {
            
        }
    }
}
