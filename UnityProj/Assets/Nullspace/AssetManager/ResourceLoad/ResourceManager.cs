﻿using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Nullspace
{
    public enum ResourceLoadMode
    {
        EDITOR,
        AB
    }

    public sealed class ResourceManager
    {
        private static ResourceLoader mResourceLoader;
        
        public static void Initialize(ResourceLoadMode mode, string abDir = null, string manifestBundleName = null)
        {
            switch (mode)
            {
#if UNITY_EDITOR
                case ResourceLoadMode.EDITOR:
                    mResourceLoader = new ResourceEditorLoader();
                    break;
#endif
                case ResourceLoadMode.AB:
                    mResourceLoader = new ResourceAbLoader(abDir, manifestBundleName);
                    break;
            }
        }
        
        public static T LoadAsset<T>(string path, string name) where T : Object
        {
            return mResourceLoader.LoadAsset<T>(path, name);
        }

        public static void LoadAssetAsync<T>(string path, string name, Action<T> callback) where T : Object
        {
            mResourceLoader.LoadAssetAsync<T>(path, name, callback);
        }

        public static void LoadAssetAsync<T, U>(string path, string name, Action<T, U> callback, U u) where T : Object
        {
            mResourceLoader.LoadAssetAsync<T, U>(path, name, callback, u);
        }

        public static void LoadAssetAsync<T, U, V>(string path, string name, Action<T, U, V> callback, U u, V v) where T : Object
        {
            mResourceLoader.LoadAssetAsync<T, U, V>(path, name, callback, u, v);
        }

        public static void LoadAssetAsync<T, U, V, W>(string path, string name, Action<T, U, V, W> callback, U u, V v, W w) where T : Object
        {
            mResourceLoader.LoadAssetAsync<T, U, V, W>(path, name, callback, u, v, w);
        }

        public static GameObject InstanceGameObject(string path, string name, bool releaseAsset = true)
        {
            return mResourceLoader.InstanceGameObject(path, name, releaseAsset);
        }

        public static void InstanceGameObjectAsync(string path, string name, Action<GameObject> callback, bool releaseAsset = true)
        {
            mResourceLoader.InstanceGameObjectAsync(path, name, callback, releaseAsset);
        }

        public static void InstanceGameObjectAsync<T>(string path, string name, Action<GameObject, T> callback, T t, bool releaseAsset = true)
        {
            mResourceLoader.InstanceGameObjectAsync(path, name, callback, t, releaseAsset);
        }

        public static void InstanceGameObjectAsync<T, U>(string path, string name, Action<GameObject, T, U> callback, T t, U u, bool releaseAsset = true)
        {
            mResourceLoader.InstanceGameObjectAsync(path, name, callback, t, u, releaseAsset);
        }

        public static void InstanceGameObjectAsync<T, U, W>(string path, string name, Action<GameObject, T, U, W> callback, T t, U u, W w, bool releaseAsset = true)
        {
            mResourceLoader.InstanceGameObjectAsync(path, name, callback, t, u, w, releaseAsset);
        }

        public static void UnloadBundle(string path)
        {
            mResourceLoader.UnloadBundle(path);
        }

        public static void UnloadUnusedAssets()
        {
            mResourceLoader.UnloadUnusedAssets();
        }

        public static void UnloadAsset(Object assetTarget)
        {
            mResourceLoader.UnloadAsset(assetTarget);
        }

    }
}

