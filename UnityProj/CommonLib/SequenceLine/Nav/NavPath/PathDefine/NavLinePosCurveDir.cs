
using GameData;
using UnityEngine;

namespace Nullspace
{
    public class NavLinePosCurveDir : AbstractNavPath
    {
        public NavLinePosCurveDir(NavPathData pathData, Vector3 offset, NavPathFlipType flipType, IPathTrigger triggerHandler) : base(pathData, offset, flipType, triggerHandler)
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
            Vector3 prevStart = mCurrentWaypointIndex == 0 ? mWaypointAppend[0] : GetWaypoint(mCurrentWaypointIndex - 1);
            Vector3 endNext = (mCurrentWaypointIndex + 2) >= mPathData.WayPoints.Count ? mWaypointAppend[1] : GetWaypoint(mCurrentWaypointIndex + 2);
            Vector3 tangent = .5f * (
                   (-prevStart + 3f * start - 3f * end + endNext) * (3 * u * u)
                   + (-prevStart + end)) + (2f * prevStart - 5f * start + 4f * end - endNext) * u;
            CurInfo.linePos = linePos;
            CurInfo.lineDir = (end - start).normalized;
            CurInfo.curvePos = (1 - u) * start + u * end;
            CurInfo.curveDir = tangent.normalized;
        }

    }
}
