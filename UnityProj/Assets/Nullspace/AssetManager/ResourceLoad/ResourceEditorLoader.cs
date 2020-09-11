#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Nullspace
{
    internal class ResourceEditorLoader : ResourceLoader
    {
        private string PrefabName(string name)
        {
            if (!name.EndsWith(".prefab"))
            {
                name = string.Format("{0}.prefab", name);
            }
            return name;
        }

        private string PrefabPath(string path)
        {
            if (!path.StartsWith("Assets/"))
            {
                path = string.Format("Assets/{0}", path);
            }
            return path;
        }

        private string ContactFilePath(string path, string name)
        {
            return string.Format("{0}/{1}", path, name);
        }

        internal override GameObject InstanceGameObject(string path, string name)
        {
            return base.InstanceGameObject(PrefabPath(path), PrefabName(name));
        }

        internal override void InstanceGameObjectAsync(string path, string name, Action<GameObject> callback)
        {
            base.InstanceGameObjectAsync(PrefabPath(path), PrefabName(name), callback);
        }

        internal override void InstanceGameObjectAsync<T>(string path, string name, Action<GameObject, T> callback, T t)
        {
            base.InstanceGameObjectAsync(PrefabPath(path), PrefabName(name), callback, t);
        }

        internal override void InstanceGameObjectAsync<T, U>(string path, string name, Action<GameObject, T, U> callback, T t, U u)
        {
            base.InstanceGameObjectAsync(PrefabPath(path), PrefabName(name), callback, t, u);
        }

        internal override void InstanceGameObjectAsync<T, U, W>(string path, string name, Action<GameObject, T, U, W> callback, T t, U u, W w)
        {
            base.InstanceGameObjectAsync(PrefabPath(path), PrefabName(name), callback, t, u, w);
        }


        internal override T LoadAsset<T>(string path, string name)
        {
            return AssetDatabase.LoadAssetAtPath<T>(ContactFilePath(path, name));
        }

        internal override void LoadAssetAsync<T>(string path, string name, Action<T> callback)
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(ContactFilePath(path, name));
            callback(asset);
        }

        internal override void LoadAssetAsync<T, U>(string path, string name, Action<T, U> callback, U u)
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(ContactFilePath(path, name));
            callback(asset, u);
        }

        internal override void LoadAssetAsync<T, U, V>(string path, string name, Action<T, U, V> callback, U u, V v)
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(ContactFilePath(path, name));
            callback(asset, u, v);
        }

        internal override void LoadAssetAsync<T, U, V, W>(string path, string name, Action<T, U, V, W> callback, U u, V v, W w)
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(ContactFilePath(path, name));
            callback(asset, u, v, w);
        }

        internal override void UnloadBundle(string path, bool unloadLoadedAssets)
        {
            
        }

    }
}
#endif