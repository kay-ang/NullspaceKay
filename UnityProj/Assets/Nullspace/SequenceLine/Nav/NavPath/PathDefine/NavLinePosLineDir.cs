
using GameData;
using System.Collections.Generic;
using UnityEngine;

namespace Nullspace
{
    public class NavLinePosLineDir : AbstractNavPath
    {
        public NavLinePosLineDir(NavPathData pathData, Vector3 offset, NavPathFlipType flipType, IPathTrigger triggerHandler) : base(pathData, offset, flipType, triggerHandler)
        {

        }
        protected override void UpdatePosAndTangent()
        {
            float len = mPathLengthMoved - GetLength(mCurrentWaypointIndex);
            float lenTotal = GetLength(mCurrentWaypointIndex + 1) - GetLength(mCurrentWaypointIndex);
            float u = len / lenTotal;
            Vector3 start = GetWaypoint(mCurrentWaypointIndex);
            Vector3 end = GetWaypoint(mCurrentWaypointIndex + 1);
            Vector3 linePos = (1 - u) * start + u * end;
            CurInfo.linePos = linePos;
            CurInfo.curvePos = linePos;
            CurInfo.lineDir = (end - start).normalized;
            CurInfo.curveDir = CurInfo.lineDir;
        }

    }
}
