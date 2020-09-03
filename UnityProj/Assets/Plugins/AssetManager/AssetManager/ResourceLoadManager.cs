#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Nullspace
{
    public partial class ResourceLoadManager
    {
        /// <summary>
        /// 指定路径 加载 资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetbundleName"></param>
        /// <param name="assetName"></param>
        /// <param name="releaseAb"></param>
        /// <returns></returns>
        public static T LoadAsset<T>(string assetPath) where T : UnityEngine.Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }

        public static T InstancePrefab<T>(string assetPath) where T : UnityEngine.Object
        {
            T t = LoadAsset<T>(assetPath);
            return GameObject.Instantiate(t);
        }
    }
}

#endif
