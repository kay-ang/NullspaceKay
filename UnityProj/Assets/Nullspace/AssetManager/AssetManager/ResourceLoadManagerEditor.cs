#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace Nullspace
{

    public partial class ResourceLoadManager
    {
        private static AssetBundleManifest mManifest;
        private static Dictionary<string, Bundle> mResourceCache;

        static ResourceLoadManager()
        {
            mResourceCache = new Dictionary<string, Bundle>();
            AssetBundle ab = AssetBundle.LoadFromFile("AssetBundle");
            mManifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="assetbundleName"></param>
        /// <param name="dependenceSet"></param>
        private static void GetAllDependencies(string assetbundleName, HashSet<string> dependenceSet)
        {
            if (dependenceSet.Contains(assetbundleName))
            {
                return ;
            }
            dependenceSet.Add(assetbundleName);
            string[] dependences = mManifest.GetAllDependencies(assetbundleName);
            foreach (string dependence in dependences)
            {
                LoadAssetbundle(dependence);
            }
        }

        private static Bundle LoadAssetbundle(string assetbundleName)
        {
            if (mResourceCache.ContainsKey(assetbundleName))
            {
                return mResourceCache[assetbundleName];
            }
            else
            {
                
                mResourceCache[assetbundleName].AddRef();
            }
            return null;
        }

        public static void ReleaseAssetbundle(string assetbundleName)
        {
            if (mResourceCache.ContainsKey(assetbundleName))
            {
                Bundle item = mResourceCache[assetbundleName];
                item.DelRef();
            }
        }

        public static T LoadAsset<T>(string assetbundleName, string assetName, bool releaseAb = false) where T : UnityEngine.Object
        {
            Bundle item = LoadAssetbundle(assetbundleName);
            T t = item.Ab.LoadAsset<T>(assetName);
            return t;
        }

        public static T InstancePrefab<T>(string assetbundleName, string assetName, bool releaseAb = false) where T : UnityEngine.Object
        {
            T t = LoadAsset<T>(assetbundleName, assetName, false);

            return default(T);
        }
    }
    
}
#endif