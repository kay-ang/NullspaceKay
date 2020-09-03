
using UnityEngine;

namespace Nullspace
{
    public class NavPathController
    {
        // 在路径上移动：位置改变和事件触发
        public NavPathMoveType mPathType;
        private AbstractNavPath mNavPath;
        // 改变位置的gameObject
        public Transform mCtlPosition;
        // 改变旋转的gameObject
        public Transform mCtlRotate;
        // 响应触发器的实例
        public IPathTrigger mTriggerHandler;
        // 控制速度:基础速度
        public float mBaseSpeed;
        // 控制速度加成累加倍数值 百分比，默认值为0
        public float mLineSpeedTimes;
        // 控制速度加成累加数值，默认值为0
        public float mLineSpeedPlus;
        public NavPathController()
        {
            mNavPath = null;
            mBaseSpeed = 0;
            mLineSpeedTimes = 1;
            mLineSpeedPlus = 0;
        }

        // Update is called once per frame
        public void Update(float time)
        {
            MoveTo(time);
        }

        /// <summary>
        /// 开始移动
        /// </summary>
        /// <param name="lineSpeed">线速度设置</param>
        public void StartMove(AbstractNavPath navPath, float lineSpeed)
        {
            mNavPath = navPath;
            if (mNavPath != null)
            {
                mCtlPosition.position = mNavPath.CurInfo.curvePos;
                RotateTo(mNavPath.CurInfo.curveDir);
                SetLineSpeed(lineSpeed);
            }
        }

        protected float Speed { get { return (mBaseSpeed + mLineSpeedPlus) * (1 + mLineSpeedTimes); } }

        public virtual void MoveTo(float time)
        {
            float moved = Speed * time;
            if (moved > 0)
            {
                DragTo(moved);
            }
        }

        /// <summary>
        /// 将物体直接拉到某一位置
        /// </summary>
        /// <param name="moved">移动的位置长度</param>
        public void DragTo(float moved)
        {
            float start = Time.realtimeSinceStartup;
            NavPathPoint track = mNavPath.UpdatePath(moved);
            float end = Time.realtimeSinceStartup;
            if (!track.isFinished)
            {
                mCtlPosition.position = track.curvePos;
                RotateTo(track.curveDir);
            }
        }

        /// <summary>
        /// 旋转处理
        /// </summary>
        /// <param name="target">目标朝向</param>
        public virtual void RotateTo(Vector3 target)
        {
            mCtlRotate.forward = target;
        }

        /// <summary>
        /// 设置线速度
        /// </summary>
        /// <param name="speed">线速度</param>
        public void SetLineSpeed(float speed)
        {
            mBaseSpeed = speed;
        }

        public void OnDrawGizmosSelected()
        {
            if (mNavPath != null)
            {
                mNavPath.OnDrawGizmos();
            }
        }

        // 指定时间插入
        public void RegisterTrigger(float time, Callback callback)
        {
            float length = mBaseSpeed * time;
            mNavPath.InsertTriggerByLength(false, length, callback);
        }
    }
}
