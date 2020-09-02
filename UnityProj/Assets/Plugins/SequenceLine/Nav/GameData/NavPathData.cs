
using Nullspace;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    public class NavPathData : GameDataMap<int, NavPathData>
    {
        public static readonly string FileUrl = "NavPathData#NavPathDatas";
        public static readonly bool IsDelayInitialized = true;
        public static readonly List<string> KeyNameList = new List<string>() { "ID" };

        public NavPathData()
        {
            OriginWayPoints = new List<Vector3>();
            WayPoints = new List<Vector3>();
            RangeLengths = new List<float>();
            SimulatePoints = new List<Vector3>();
            Triggers = new List<int>();
        }

        public int ID { get; set; }
        public float PathLength { get; set; }
        public List<Vector3> OriginWayPoints { get; set; }
        public List<Vector3> WayPoints { get; set; }
        public List<Vector3> SimulatePoints { get; set; }
        public List<float> RangeLengths { get; set; }
        public List<int> Triggers { get; set; }

    }
}
