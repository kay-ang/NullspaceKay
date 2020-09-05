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
        public static BundleManager Instance = new BundleManager();

        private StringBuilder mStringBuilder = new StringBuilder();
        private BundleManager() { }
        private bool mIsInitialized = false;
        private string mABDir;
        private Dictionary<string, Bundle> mBundleCaches;
        private Dictionary<string, AssetBundleCreateRequest> mBundleAsyncCaches;
        private AssetBundleManifest mManifest;

        private string Format(string format, params object[] paramObjs)
        {
            mStringBuilder.Length = 0;
            mStringBuilder.AppendFormat(format, paramObjs);
            return mStringBuilder.ToString();
        }

        public void Initialize(string abDir, string manifestBundleName)
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
        
        public void UnloadBundle(string bundleName)
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

        public void LoadBundleAsync(string bundleName, Callback callback)
        {
            LoadBundle(bundleName, true);
        }

        public Bundle LoadBundleSync(string bundleName)
        {
            return LoadBundle(bundleName, false);
        }

        protected void SyncInterruptAsync(Bundle bundle)
        {
            if (bundle != null)
            {
                // 先异步(异步未加载完)，再同步加载 处理
                if (bundle.IsAsyncLoading)
                {
                    // 同步打断异步
                    AssetBundleCreateRequest asyncOperation = GetAssetBundleCreateRequest(bundle.BundleName);
                    if (asyncOperation != null)
                    {
                        bundle.Ab = asyncOperation.assetBundle;
                        asyncOperation.assetBundle.LoadAsset<_EmptyAsset>("");
                        // RemoveAsyncOperation(bundleName);
                    }
                }
            }
        }

        /// <summary>
        /// 直接加载 
        /// </summary>
        /// <param name="bundleName">ab包名</param>
        /// <param name="isAsync">是否异步</param>
        /// <returns></returns>
        protected Bundle LoadBundle(string bundleName, bool isAsync)
        {
            Bundle rootBundle = GetBundle(bundleName);
            if (rootBundle == null || IsDependence(rootBundle))  // 目前不存在 或者 之前通过被依赖而加载
            {
                if (rootBundle == null)  // 目前不存在
                {
                    rootBundle = CreateBundle(bundleName, false, isAsync); // 创建 bundle
                }

                SetIsDependence(bundleName, false);
                rootBundle.AddRef(); // 引用 + 1
                string[] bundles = GetAllDependencies(bundleName); // 获取所有依赖ab
                foreach (string bundle in bundles)
                {
                    Bundle dependence = GetBundle(bundle);
                    if (dependence == null) // 不存在
                    {
                        dependence = CreateBundle(bundle, true, isAsync); // 加载 依赖
                        rootBundle.AddDependece(dependence); // 添加 依赖
                    }
                    dependence.AddRef(); // 引用 + 1
                }
            }
            DebugUtils.Assert(IsContain(bundleName) && !IsDependence(bundleName), string.Format("{0} wrong", bundleName));
            if (!isAsync) // 同步时，才检查是否存在异步加载的AB
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

        private Bundle CreateBundle(string bundleName, bool isDependence, bool isAsync)
        {
            DebugUtils.Assert(!IsContain(bundleName), "");
            Bundle bundle = null;
            if (isAsync)
            {
                AssetBundleCreateRequest createRequest = AssetBundle.LoadFromFileAsync(FormatAbPath(bundleName));
                bundle = new Bundle(bundleName, null);
                bundle.IsAsyncLoading = true;
                createRequest.completed += (asyncOperation) => 
                                        {
                                            DebugUtils.Assert(asyncOperation.isDone, "wrong " + bundleName);
                                            RemoveAsyncOperation(bundleName);
                                            // 可能此时已被销毁，重新获取一下
                                            Bundle cacheBundle = GetBundle(bundleName);
                                            if (cacheBundle != null)
                                            {
                                                cacheBundle.Ab = createRequest.assetBundle;
                                                bundle.IsAsyncLoading = false;
                                            }
                                            else
                                            {
                                                // 卸载
                                                createRequest.assetBundle.Unload(true);
                                            }
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
