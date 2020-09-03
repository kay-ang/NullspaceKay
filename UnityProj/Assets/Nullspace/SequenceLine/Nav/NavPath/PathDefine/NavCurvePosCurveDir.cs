
using GameData;
using UnityEngine;

namespace Nullspace
{
    public class NavCurvePosCurveDir : AbstractNavPath
    {
        public NavCurvePosCurveDir(NavPathData pathData, Vector3 offset, NavPathFlipType flipType, IPathTrigger triggerHandler) : base(pathData, offset, flipType, triggerHandler)
        {

        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// 待优化版本：思路清晰为主
        /// </summary>
        protected override void UpdatePosAndTangent()
        {
            // 计算或缓存所处线段的总长度
            float lenTotal = GetLength(mCurrentWaypointIndex + 1) - GetLength(mCurrentWaypointIndex);
            // 计算所处线段已走过的长度
            float len = mPathLengthMoved - GetLength(mCurrentWaypointIndex);
            // 计算 已走过的线段长度/线段的总长度
            float u = len / lenTotal;

            // 确定当前线段的前一个点
            Vector3 prevStart = mCurrentWaypointIndex == 0 ? mWaypointAppend[0] : GetWaypoint(mCurrentWaypointIndex - 1);
            // 确定当前线段的起点
            Vector3 start = GetWaypoint(mCurrentWaypointIndex);
            // 确定当前线段的终点
            Vector3 end = GetWaypoint(mCurrentWaypointIndex + 1);
            // 确定当前线段的下一个点
            Vector3 endNext = (mCurrentWaypointIndex + 2) >= mPathData.WayPoints.Count ? mWaypointAppend[1] : GetWaypoint(mCurrentWaypointIndex + 2);

            //// 套用插值公式 计算当前时刻所处曲线的点
            //Vector3 inter = .5f * (
            //       (-prevStart + 3f * start - 3f * end + endNext) * (u * u * u)
            //       + (2f * prevStart - 5f * start + 4f * end - endNext) * (u * u)
            //       + (-prevStart + end) * u
            //       + 2f * start);
            //// 套用插值公式 计算当前时刻所处曲线点的切线
            //Vector3 tangent = .5f * (
            //       (-prevStart + 3f * start - 3f * end + endNext) * (3 * u * u)
            //       + (2f * prevStart - 5f * start + 4f * end - endNext) * (2 * u)
            //       + (-prevStart + end) * 1
            //       + 2f * start * 0);

            // 套用插值公式 计算当前时刻所处曲线的点
            float tu = u * u;
            Vector3 inter = .5f * (
                   (-prevStart + 3f * start - 3f * end + endNext) * (u * tu)
                   + (2f * prevStart - 5f * start + 4f * end - endNext) * tu
                   + (-prevStart + end) * u) + start;
            // 套用插值公式 计算当前时刻所处曲线点的切线
            Vector3 tangent = .5f * (
                   (-prevStart + 3f * start - 3f * end + endNext) * (3 * tu)
                   + (-prevStart + end)) + (2f * prevStart - 5f * start + 4f * end - endNext) * u;
            // 线性插值 计算当前时刻所处线段的点
            CurInfo.linePos = (1 - u) * start + u * end;
            // 计算当前时刻所处线段的方向
            CurInfo.lineDir = (end - start).normalized;
            // 保存 当前时刻点
            CurInfo.curvePos = inter;
            // 保存 当前时刻点坐标的切向
            CurInfo.curveDir = tangent.normalized;
        }

    }
}
