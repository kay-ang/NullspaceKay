﻿using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Nullspace
{
    internal abstract class ResourceLoader
    {
        internal abstract T LoadAsset<T>(string path, string name) where T : Object;
        internal abstract void LoadAssetAsync<T>(string path, string name, Action<T> callback) where T : Object;
        internal abstract void LoadAssetAsync<T, U>(string path, string name, Action<T, U> callback, U u) where T : Object;
        internal abstract void LoadAssetAsync<T, U, V>(string path, string name, Action<T, U, V> callback, U u, V v) where T : Object;
        internal abstract void LoadAssetAsync<T, U, V, W>(string path, string name, Action<T, U, V, W> callback, U u, V v, W w) where T : Object;
        internal abstract void UnloadBundle(string path);

        internal virtual void UnloadAsset(UnityEngine.Object assetTarget)
        {
            Resources.UnloadAsset(assetTarget);
        }

        internal virtual void UnloadUnusedAssets()
        {
            Resources.UnloadUnusedAssets();
        }

        internal virtual GameObject InstanceGameObject(string path, string name, bool releaseAsset = true)
        {
            GameObject goAsset = LoadAsset<GameObject>(path, name);
            if (goAsset == null)
            {
                DebugUtils.Log(InfoType.Error, "wrong path: {0}/{1}", path, name);
                return null;
            }
            GameObject go = Object.Instantiate(goAsset);
            if (releaseAsset)
            {
                UnloadAsset(goAsset);
            }
            return go;
        }

        internal virtual void InstanceGameObjectAsync(string path, string name, Action<GameObject> callback, bool releaseAsset = true)
        {
            Action<GameObject> act = (goAsset) =>
            {
                GameObject go = Object.Instantiate(goAsset);
                if (releaseAsset)
                {
                    UnloadAsset(goAsset);
                }
            };
            LoadAssetAsync(path, name, act);
        }

        internal virtual void InstanceGameObjectAsync<T>(string path, string name, Action<GameObject, T> callback, T t, bool releaseAsset = true)
        {
            Action<GameObject> act = (goAsset) =>
            {
                GameObject go = Object.Instantiate(goAsset);
                if (releaseAsset)
                {
                    UnloadAsset(goAsset);
                }
                callback(go, t);
            };
            LoadAssetAsync(path, name, act);
        }

        internal virtual void InstanceGameObjectAsync<T, U>(string path, string name, Action<GameObject, T, U> callback, T t, U u, bool releaseAsset = true)
        {
            Action<GameObject> act = (goAsset) =>
            {
                GameObject go = Object.Instantiate(goAsset);
                if (releaseAsset)
                {
                    UnloadAsset(goAsset);
                }
                callback(go, t, u);
            };
            LoadAssetAsync(path, name, act);
        }

        internal virtual void InstanceGameObjectAsync<T, U, W>(string path, string name, Action<GameObject, T, U, W> callback, T t, U u, W w, bool releaseAsset = true)
        {
            Action<GameObject> act = (goAsset) =>
            {
                GameObject go = Object.Instantiate(goAsset);
                if (releaseAsset)
                {
                    UnloadAsset(goAsset);
                }
                callback(go, t, u, w);
            };
            LoadAssetAsync(path, name, act);
        }

    }
}