using System;

namespace Nullspace
{
    internal class ResourceAbLoader : ResourceLoader
    {
        protected BundleManager mBundleManager;

        public ResourceAbLoader(string abDir, string manifestBundleName)
        {
            Initialize(abDir, manifestBundleName);
        }

        private string FormatAbName(string path)
        {
            return path.Replace("/", "_").ToLower();
        }

        protected void Initialize(string abDir, string manifestBundleName)
        {
            mBundleManager = new BundleManager();
            mBundleManager.Initialize(abDir, manifestBundleName);
        }

        internal override T LoadAsset<T>(string path, string name)
        {
            // path 变成 AB 名
            Bundle bundle = mBundleManager.LoadBundleSync(FormatAbName(path));
            T asset = bundle.LoadAsset<T>(name);
            return asset;
        }

        internal override void LoadAssetAsync<T>(string path, string name, Action<T> callback)
        {
            Action<Bundle> load = (bundle) =>
            {
                T asset = bundle.LoadAsset<T>(name);
                callback(asset);
            };
            mBundleManager.LoadBundleAsync(FormatAbName(path), load);
        }

        internal override void LoadAssetAsync<T, U>(string path, string name, Action<T, U> callback, U u)
        {
            Action<Bundle> load = (bundle) =>
            {
                T asset = bundle.LoadAsset<T>(name);
                callback(asset, u);
            };
            mBundleManager.LoadBundleAsync(FormatAbName(path), load);
        }

        internal override void LoadAssetAsync<T, U, V>(string path, string name, Action<T, U, V> callback, U u, V v)
        {
            Action<Bundle> load = (bundle) =>
            {
                T asset = bundle.LoadAsset<T>(name);
                callback(asset, u, v);
            };
            mBundleManager.LoadBundleAsync(FormatAbName(path), load);
        }

        internal override void LoadAssetAsync<T, U, V, W>(string path, string name, Action<T, U, V, W> callback, U u, V v, W w) 
        {
            Action<Bundle> load = (bundle) =>
            {
                T asset = bundle.LoadAsset<T>(name);
                callback(asset, u, v, w);
            };
            mBundleManager.LoadBundleAsync(FormatAbName(path), load);
        }

        internal override void UnloadBundle(string path, bool unloadLoadedAssets)
        {
            mBundleManager.UnloadBundle(FormatAbName(path), unloadLoadedAssets);
        }
    }
}
