
using Nullspace;
using System.Collections.Generic;

namespace GameData
{
    public class AssetbundleUpdateData : GameDataList<AssetbundleUpdateData>
    {
        // 不允许 通过 FileUrl 初始化
        public static readonly string FileUrl = string.Format("{0}#{1}", AssetManager.AssetbundleInfoFile, AssetManager.AssetbundleInfosTag);
        public static readonly bool IsDelayInitialized = false;
        public static readonly List<string> KeyNameList = null;

        public static bool IsOriginNull() { return mDataList == null; }

        // string Version, todo
        public string Name { get; private set; }
        public string Md5Hash { get; private set; }
        public int FileSize { get; private set; }
    }
}
