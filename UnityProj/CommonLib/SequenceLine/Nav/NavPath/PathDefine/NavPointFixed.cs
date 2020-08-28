
using GameData;
using UnityEngine;

namespace Nullspace
{
    public class NavPointFixed : AbstractNavPath
    {
        public NavPointFixed(NavPathData pathData, Vector3 offset, NavPathFlipType flipType, IPathTrigger triggerHandler) : base(pathData, offset, flipType, triggerHandler)
        {

        }

        /// <summary>
        /// 更新路点索引
        /// </summary>
        /// <returns></returns>
        protected override int UpdateWaypointIndex()
        {
            mCurrentWaypointIndex = 0;
            CurInfo.isDirChanged = false;
            return 0;
        }

        protected override void UpdatePosAndTangent()
        {
            Vector3 start = GetWaypoint(mCurrentWaypointIndex);
            CurInfo.linePos = start;
            CurInfo.lineDir = Vector3.forward;
            CurInfo.curvePos = start;
            CurInfo.curveDir = Vector3.forward;
        }
    }
}
