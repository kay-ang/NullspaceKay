using Nullspace;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    // [Start, End]
    public class AppVersionUpdateData : GameDataList<AppVersionUpdateData>
    {
        // 不允许 通过 FileUrl 初始化
        public static readonly string FileUrl = string.Format("{0}#{1}", ResourceUpdateManager.AppVersionInfoFile, ResourceUpdateManager.AppVersionInfoTag);
        public static readonly bool IsDelayInitialized = false;
        public static readonly List<string> KeyNameList = null;

        public Vector3Int Start { get; private set; }
        public Vector3Int End { get; private set; }
        public List<Vector3Int> Filters { get; private set; }
        public string InfoTips { get; private set; }
    }
}
