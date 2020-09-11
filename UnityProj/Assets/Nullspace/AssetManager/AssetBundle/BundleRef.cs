
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

        public virtual bool DelRef(bool unloadLoadedAssets)
        {
            mRefCount--;
            if (mRefCount <= 0)
            {
                mIsDestroyed = true;
                Destroy(unloadLoadedAssets);
            }
            return mIsDestroyed;
        }

        public virtual void Destroy(bool unloadLoadedAssets)
        {
            
        }
    }
}
