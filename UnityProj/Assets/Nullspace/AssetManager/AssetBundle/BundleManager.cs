using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nullspace
{
    public class BundleManager
    {
        private StringBuilder mStringBuilder = new StringBuilder();
        
        private bool mIsInitialized = false;
        private string mABDir;
        private Dictionary<string, Bundle> mBundleCaches;
        private Dictionary<string, AssetBundleCreateRequest> mBundleAsyncCaches;
        private AssetBundleManifest mManifest;

        public BundleManager() { }

        internal string Format(string format, params object[] paramObjs)
        {
            mStringBuilder.Length = 0;
            mStringBuilder.AppendFormat(format, paramObjs);
            return mStringBuilder.ToString();
        }

        internal void Initialize(string abDir, string manifestBundleName)
        {
            mABDir = abDir;
            if (mManifest != null)
            {
                Resources.UnloadAsset(mManifest);
                mManifest = null;
            }
            if (mBundleCaches != null)
            {
                foreach (Bundle bundle in mBundleCaches.Values)
                {
                    bundle.Destroy();
                }
            }
            mBundleAsyncCaches = new Dictionary<string, AssetBundleCreateRequest>();
            mBundleCaches = new Dictionary<string, Bundle>();
            AssetBundle ab = AssetBundle.LoadFromFile(FormatAbPath(manifestBundleName));
            mManifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            // 卸载 AssetBundle，不卸载 AssetBundleManifest
            ab.Unload(false);
            mIsInitialized = true;
        }

        internal void UnloadBundle(string bundleName)
        {
            if (mIsInitialized)
            {
                if (IsContain(bundleName))
                {
                    bool isDestroy = GetBundle(bundleName).DelRef();
                    if (isDestroy)
                    {
                        DestroyBundle(bundleName);
                    }
                }
            }
        }

        internal void LoadBundleAsync(string bundleName, Action<Bundle> callback)
        {
            Bundle bundle = LoadBundle(bundleName, true);
            // 异步加载完毕 ：有可能依赖未加载完毕
            if (bundle != null)
            {
                bundle.AsyncLoadCallback += callback;
                bundle.InvokeAsync();
            }
        }

        internal Bundle LoadBundleSync(string bundleName)
        {
            return LoadBundle(bundleName, false);
        }

        /// <summary>
        /// 只能是同步加载时，才会调到这里
        /// </summary>
        /// <param name="bundle"></param>
        protected void SyncInterruptAsync(Bundle bundle)
        {
            if (bundle != null)
            {
                // 先异步(异步未加载完)，再同步加载 处理
                if (bundle.IsAsyncLoading)
                {
                    DebugUtils.Log(InfoType.Info, "SyncInterruptAsync {0}", bundle.BundleName);
                    bundle.IsAsyncLoading = false;
                    // 同步打断异步
                    AssetBundleCreateRequest asyncOperation = GetAssetBundleCreateRequest(bundle.BundleName);
                    if (asyncOperation != null)
                    {
                        // asyncOperation.assetBundle 直接转同步加载
                        bundle.Ab = asyncOperation.assetBundle;
                    }
                }
            }
        }

        /// <summary>
        /// 0. 加载：引用计数设计
        ///     0. 区分 主包加载 和 依赖包加载。LoadBundleAsync和LoadBundleSync均是主包加载
        ///     1. 主包加载：所有依赖的包(A,B,C,...) 引用只加1，不区分某些依赖包A还依赖了其他的依赖包(B...)
        ///     2. 之前主包加载时，某个依赖包A被加载，此时的计数为1
        ///     3. 现在 A 作为主包加载，A所有依赖的包 引用只加1。此时 A 计数为 2，A依赖的B也为2
        ///     4. 再次 A 作为主包加载，直接返回 A 包，不再做引用计数
        ///     5. 注意：依赖的层次不要深，最好都是小范围依赖。打包时，要做好规划。
        ///     
        /// 1. 异步加载：回调设计
        ///     1. 参数： 主包 总依赖数量 和 已经加载依赖的数量
        ///     2. 依赖包未加载，依赖包加载的接口注册complete：完成时要让 主包 依赖加载数量剩余完成 -1
        ///     3. 依赖包已加载，直接 让主包 依赖加载数量剩余完成 -1
        ///     4. 主包已加载：上层接口直接回调，此时有可能依赖未加载完。Bundle内部检测是否执行回调
        ///     5. 主包完成数量等于总数量，则主包执行给上层的 加载完成 回调
        ///     
        /// 2. 同步加载
        ///     1. 同步加载和异步加载区别在：CreateBundle
        ///     2. 如果之前存在异步加载，现在同步打断异步：rootBundle.Sync(SyncInterruptAsync);
        ///         1. 所有异步加载的依赖包需转同步
        ///         
        /// 3. 只有同步打断异步，不存在异步打断同步
        /// 
        /// </summary>
        /// <param name="rootBundleName">主包：ab包名</param>
        /// <param name="isAsync">是否异步</param>
        /// <returns></returns>
        protected Bundle LoadBundle(string rootBundleName, bool isAsync)
        {
            Bundle rootBundle = GetBundle(rootBundleName);
            // 目前不存在 或者 之前通过被依赖而加载
            if (rootBundle == null || IsDependence(rootBundle))  
            {
                // 目前不存在
                if (rootBundle == null) 
                {
                    // 创建 bundle
                    rootBundle = CreateBundle(rootBundleName, false, isAsync);
                }
                // 设置为主包，而非 依赖包
                SetIsDependence(rootBundleName, false);
                // 引用 + 1
                rootBundle.AddRef();
                // 获取所有依赖ab
                string[] bundles = GetAllDependencies(rootBundleName);
                // 设置依赖包剩余加载数
                rootBundle.LeftDependenceCount = bundles.Length;
                foreach (string dependenceBundleName in bundles)
                {
                    Bundle dependenceBundle = GetBundle(dependenceBundleName);
                    // 不存在
                    if (dependenceBundle == null) 
                    {
                        // 加载 依赖
                        dependenceBundle = CreateBundle(dependenceBundleName, true, isAsync);
                        // 添加 依赖
                        rootBundle.AddDependece(dependenceBundle); 
                    }
                    // 引用 + 1
                    dependenceBundle.AddRef(); 
                    // 依赖异步加载中
                    if (dependenceBundle.IsAsyncLoading)
                    {
                        AssetBundleCreateRequest req = GetAssetBundleCreateRequest(dependenceBundleName);
                        req.completed += (asyncOperation) => 
                            {
                                // 依赖已经加载好了， root 剩余数量-1
                                rootBundle.OneDependenceLoaded();
                            };
                    }
                    else
                    {
                        // 依赖已经加载好了， root 剩余数量-1
                        rootBundle.OneDependenceLoaded();
                    }
                }
            }
            DebugUtils.Assert(IsContain(rootBundleName) && !IsDependence(rootBundleName), string.Format("{0} wrong", rootBundleName));
            // 同步时，才检查是否存在异步加载的AB
            if (!isAsync) 
            {
                rootBundle.Sync(SyncInterruptAsync);
            }
            return rootBundle;
        }

        private string[] GetAllDependencies(string bundleName)
        {
            return mManifest.GetAllDependencies(bundleName);
        }

        private bool IsContain(string bundleName)
        {
            return mBundleCaches.ContainsKey(bundleName);
        }

        private Bundle GetBundle(string bundleName)
        {
            if (IsContain(bundleName))
            {
                return mBundleCaches[bundleName];
            }
            return null;
        }

        private void RemoveAsyncOperation(string bundleName)
        {
            DebugUtils.Log(InfoType.Info, "RemoveAsyncOperation {0}", bundleName);
            if (mBundleAsyncCaches.ContainsKey(bundleName))
            {
                mBundleAsyncCaches.Remove(bundleName);
            }
        }

        private void AddAsyncOperation(string bundleName, AssetBundleCreateRequest asyncOperation)
        {
            DebugUtils.Log(InfoType.Info, "AddAsyncOperation {0}", bundleName);
            if (!mBundleAsyncCaches.ContainsKey(bundleName))
            {
                mBundleAsyncCaches.Add(bundleName, asyncOperation);
            }
        }

        private AssetBundleCreateRequest GetAssetBundleCreateRequest(string bundleName)
        {
            if (IsAsyncLoading(bundleName))
            {
                return mBundleAsyncCaches[bundleName];
            }
            return null;
        }

        private bool IsAsyncLoading(string bundleName)
        {
            return mBundleAsyncCaches.ContainsKey(bundleName);
        }

        private void DestroyBundle(string bundleName)
        {
            if (mBundleCaches.ContainsKey(bundleName))
            {
                mBundleCaches.Remove(bundleName);
                DebugUtils.Log(InfoType.Info, "DestroyBundle {0}", bundleName);
            }
        }

        private string FormatAbPath(string bundleName)
        {
            return Format("{0}/{1}", mABDir, bundleName);
        }

        /// <summary>
        /// 加载一个AB资源
        /// </summary>
        /// <param name="bundleName">报名</param>
        /// <param name="isDependence">主包还是依赖包</param>
        /// <param name="isAsync">异步还是同步</param>
        /// <returns></returns>
        private Bundle CreateBundle(string bundleName, bool isDependence, bool isAsync)
        {
            DebugUtils.Assert(!IsContain(bundleName), "");
            Bundle bundle = null;
            if (isAsync)
            {
                bundle = new Bundle(bundleName, null);
                bundle.IsAsyncLoading = true;
                AssetBundleCreateRequest createRequest = AssetBundle.LoadFromFileAsync(FormatAbPath(bundleName));
                createRequest.completed += (asyncOperation) => 
                                        {
                                            DebugUtils.Assert(asyncOperation.isDone, "wrong " + bundleName);
                                            // 可能此时已被销毁，重新获取一下
                                            Bundle cacheBundle = GetBundle(bundleName);
                                            if (cacheBundle != null)
                                            {
                                                if (cacheBundle.Ab == null)
                                                {
                                                    cacheBundle.Ab = createRequest.assetBundle;
                                                }
                                                bundle.IsAsyncLoading = false;
                                            }
                                            else
                                            {
                                                // 卸载
                                                createRequest.assetBundle.Unload(true);
                                            }
                                            RemoveAsyncOperation(bundleName);
                                        };
                AddAsyncOperation(bundleName, createRequest);
            }
            else
            {
                AssetBundle ab = AssetBundle.LoadFromFile(FormatAbPath(bundleName));
                bundle = new Bundle(bundleName, ab);
                bundle.IsAsyncLoading = false;
            }
            bundle.IsLoadedByDpendenced = isDependence;
            mBundleCaches.Add(bundleName, bundle);
            return bundle;
        }

        private void SetIsDependence(string bundleName, bool isDependence)
        {
            DebugUtils.Assert(IsContain(bundleName), "");
            mBundleCaches[bundleName].IsLoadedByDpendenced = isDependence;
        }

        private bool IsDependence(string bundleName)
        {
            DebugUtils.Assert(IsContain(bundleName), "");
            return IsDependence(mBundleCaches[bundleName]);
        }

        private bool IsDependence(Bundle bundle)
        {
            return bundle.IsLoadedByDpendenced;
        }

        private void AddRef(string bundleName)
        {
            DebugUtils.Assert(IsContain(bundleName), "");
            mBundleCaches[bundleName].AddRef();
        }

    }
}
