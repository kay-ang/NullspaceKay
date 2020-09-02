using Nullspace;
using System.Collections.Generic;

namespace GameData
{
    public class BuildAssetbundleConfig : GameDataList<BuildAssetbundleConfig>
    {
        public static readonly string FileUrl = "BuildAssetbundleConfig#BuildAssetbundleConfigs";
        public static readonly bool IsDelayInitialized = false;
        public static readonly List<string> KeyNameList = null;

        public string Path { get; private set; }
        public bool Altas { get; private set; }
    }
}
