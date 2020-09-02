using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nullspace
{
    public class ResourceRef
    {
        protected int mRefCount;

        public ResourceRef()
        {
            mRefCount = 0;
        }

        public virtual void AddRef()
        {
            mRefCount++;
        }

        public virtual void DelRef()
        {
            mRefCount--;
            if (mRefCount <= 0)
            {
                Destroy();
            }
        }

        public virtual void Destroy()
        {

        }
    }

    public class ResourceItem : ResourceRef
    {
        protected AssetBundle mAb;
        protected List<ResourceItem> mDependencies;

        public ResourceItem(AssetBundle mAb)
        {
            mAb = null;
            mDependencies = new List<ResourceItem>();
        }

        public override void AddRef()
        {
            base.AddRef();
            foreach (ResourceItem item in mDependencies)
            {
                item.AddRef();
            }
        }

        public override void DelRef()
        {
            base.DelRef();
            foreach (ResourceItem item in mDependencies)
            {
                item.DelRef();
            }
        }

        public override void Destroy()
        {
            if (mAb != null)
            {
                mAb.Unload(false);
                mAb = null;
                Resources.UnloadUnusedAssets();
            }
        }
    }
}
