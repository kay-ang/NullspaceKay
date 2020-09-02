using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nullspace
{
    public class ResourceLoadManager
    {
        private static Dictionary<string, ResourceItem> mResourceCache = new Dictionary<string, ResourceItem>();
        
        public static ResourceItem LoadAssetbundle(string assetbundleName)
        {
            if (mResourceCache.ContainsKey(assetbundleName))
            {
                return mResourceCache[assetbundleName];
            }
            else
            {

            }
            return null;
        }

        public static void LoadPrefab()
        {

        }

        public static void LoadAsset<T>(string assetbundleName, string assetName, bool releaseAb = false)
        {

        }

        public static void InstancePrefab(string assetbundleName, string assetName, bool releaseAb = false)
        {

        }
    }
}
