
using Nullspace;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    public class PathTriggerData : GameDataMap<int, PathTriggerData>
    {
        public static readonly string FileUrl = "NavPathData#PathTriggerDatas";
        public static readonly bool IsDelayInitialized = true;
        public static readonly List<string> KeyNameList = new List<string>() { "ID" };

        public PathTriggerData()
        {
            Params = new Dictionary<int, string>();
        }
        public int ID { get; set; }
        public int Type { get; set; }
        public float Length { get; set; }
        public float Duration { get; set; }
        public float Accelerate { get; set; }
        public Dictionary<int, string> Params { get; set; }
        public string GetParam(int fieldKey)
        {
            return Params[fieldKey];
        }
    }
}
