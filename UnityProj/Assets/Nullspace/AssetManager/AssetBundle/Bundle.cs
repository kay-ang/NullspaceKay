﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nullspace
{

    public class Bundle : BundleRef
    {
        // 对应的AB包
        public AssetBundle Ab;
        // 是否通过 被依赖项 被加载：true，可能自身依赖的AB未被加载；false，依赖的AB必定被加载进来
        public bool IsLoadedByDpendenced;
        // 全部依赖的ab包，包括间接ab包
        protected List<Bundle> mDependencies;

        public Bundle()
        {
            Ab = null;
            IsLoadedByDpendenced = true;
            mDependencies = new List<Bundle>();
        }

        public Bundle(AssetBundle ab) : this()
        {
            Ab = ab;
        }

        public void AddDependece(Bundle dependence)
        {
            mDependencies.Add(dependence);
        }

        public override int AddRef()
        {
            int cnt = base.AddRef();
            foreach (Bundle item in mDependencies)
            {
                item.AddRef();
            }
            return cnt;
        }

        public override bool DelRef()
        {
            bool isDestroy = base.DelRef();
            foreach (Bundle item in mDependencies)
            {
                item.DelRef();
            }
            return isDestroy;
        }

        protected void DestroyAb()
        {
            if (Ab != null)
            {
                Ab.Unload(true);
                Ab = null;
                Resources.UnloadUnusedAssets();
            }
        }

        public override void Destroy()
        {
            DestroyAb();
            mDependencies.Clear();
            base.Destroy();
        }
    }
}
