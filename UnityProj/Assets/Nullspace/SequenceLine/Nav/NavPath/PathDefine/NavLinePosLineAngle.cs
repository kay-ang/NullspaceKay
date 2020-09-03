
using GameData;
using UnityEngine;

namespace Nullspace
{
    public class NavLinePosLineAngle : AbstractNavPath
    {
        private float mFrameCountInv;   // 插值帧数的倒数
        private int mCurFrame;          // 当前执行的帧数
        private Vector3 mLast;          // 插值的起始方向
        private Vector3 mNext;          // 插值的结束方向

        public NavLinePosLineAngle(NavPathData pathData, Vector3 offset, NavPathFlipType flipType, IPathTrigger triggerHandler, float frameCount) : base(pathData, offset, flipType, triggerHandler)
        {
            // 会先执行 Initialize()
            mFrameCountInv = 1.0f / frameCount;
            mCurFrame = 0;
            CurInfo.isDirChanged = true;
            UpdatePosAndTangent();
            CurInfo.curveDir = mNext;
            mLast = mNext;
        }

        protected override void Initialize()
        {
            InitializeAppendWaypoint();
            RegisterAllTriggers();
        }

        protected override void UpdatePosAndTangent()
        {
            mCurFrame++;
            float len = mPathLengthMoved - GetLength(mCurrentWaypointIndex);
            float lenTotal = GetLength(mCurrentWaypointIndex + 1) - GetLength(mCurrentWaypointIndex);
            float u = len / lenTotal;
            Vector3 start = GetWaypoint(mCurrentWaypointIndex);
            Vector3 end = GetWaypoint(mCurrentWaypointIndex + 1);
            Vector3 linePos = (1 - u) * start + u * end;
            CurInfo.linePos = linePos;
            CurInfo.curvePos = linePos;
            if (CurInfo.isDirChanged)
            {
                mLast = CurInfo.curveDir;
                mNext = (end - start).normalized;
                mCurFrame = 0;
            }
            float pro = mCurFrame * mFrameCountInv;
            if (pro < 1)
            {
                CurInfo.curveDir = MathUtils.Interpolation(mLast, mNext, pro).normalized;
            }
            else
            {
                CurInfo.curveDir = mNext;
            }
        }
    }
}
