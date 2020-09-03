
using UnityEngine;

namespace Nullspace
{
    public class ResourceCacheBindParent
    {
        public static Transform CacheUnused;
        public static Transform WorldEffectBind;

        static ResourceCacheBindParent()
        {
            CacheUnused = new GameObject("CacheUnused").transform;
            WorldEffectBind = new GameObject("WorldEffectBind").transform;
        }

        

        public static bool IsCacheUnusedParent(GameObject go)
        {
            if (go != null)
            {
                return go.transform.parent.name == "CacheUnused";
            }
            return false;
        }

    }
}
