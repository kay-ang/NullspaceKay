using GameData;
using System.Collections.Generic;
using UnityEngine;

namespace Nullspace
{

    public partial class AbstractNavPath
    {
        public virtual void OnDrawGizmos()
        {
            List<Vector3> points = mPathData.WayPoints;
            int cnt = points.Count;
            for (int i = 1; i < cnt; ++i)
            {
                Gizmos.color = i % 2 == 1 ? Color.red : Color.blue;
                Gizmos.DrawLine(points[i - 1] + mOffset, points[i] + mOffset);
            }

            if (mPathData.OriginWayPoints != null)
            {
                cnt = mPathData.OriginWayPoints.Count;
                Gizmos.color = Color.black;
                for (int i = 1; i < cnt; ++i)
                {
                    Gizmos.DrawLine(mPathData.OriginWayPoints[i - 1] + mOffset, mPathData.OriginWayPoints[i] + mOffset);
                }
            }

            if (mPathData.SimulatePoints != null)
            {
                cnt = mPathData.SimulatePoints.Count;
                Gizmos.color = Color.green;
                for (int i = 1; i < cnt; ++i)
                {
                    Gizmos.DrawLine(mPathData.SimulatePoints[i - 1] + mOffset, mPathData.SimulatePoints[i] + mOffset);
                }
            }

            Gizmos.color = Color.green;
            Gizmos.DrawLine(CurInfo.curvePos, CurInfo.curvePos + 10 * CurInfo.curveDir);
            cnt = mTriggers.Count;
            for (int i = 0; i < cnt; ++i)
            {
                mTriggers[i].OnDrawGizmos(TriggerPos(mTriggers[i].TriggerLength));
            }
            Gizmos.DrawSphere(CurInfo.linePos, 0.5f);
        }

        /// <summary>
        /// 触发器长度对应的坐标点
        /// </summary>
        /// <param name="length">触发器长度</param>
        /// <returns>坐标点</returns>
        private Vector3 TriggerPos(float length)
        {
            int i = 0;
            int cnt = mPathData.RangeLengths.Count;
            while (i < cnt)
            {
                if (mPathData.RangeLengths[i] <= length)
                {
                    i++;
                }
                else
                {
                    break;
                }
            }
            Vector3 result = Vector3.zero;
            if (i < 1)
            {
                result = mPathData.WayPoints[0];
            }
            else if (i >= cnt)
            {
                result = mPathData.WayPoints[cnt - 1];
            }
            else
            {
                float len1 = length - mPathData.RangeLengths[i - 1];
                float len2 = mPathData.RangeLengths[i] - mPathData.RangeLengths[i - 1];
                Vector3 all = mPathData.WayPoints[i] - mPathData.WayPoints[i - 1];
                result = len1 / len2 * all + mPathData.WayPoints[i - 1];
            }
            result = result + mOffset;
            NavPathUtils.Flip(bFlipType, ref result);
            return result;
        }

    }

    public partial class AbstractNavPath
    {
        /// <summary>
        /// 移除触发器，只保留 结束触发器
        /// </summary>
        public void RemoveTriggersBeforeEnd()
        {
            int cnt = mTriggers.Count;
            if (cnt > 1)
            {
                var endTrigger = mTriggers[cnt - 1];
                mTriggers.Clear();
                mTriggers.Add(endTrigger);
            }
        }

        /// <summary>
        /// 更新触发器
        /// </summary>
        protected void UpdateTrigger()
        {
            while (mTriggers.Count > 0)
            {
                // 是否存在被触发发的触发器
                if (mTriggers[0].TriggerLength < mPathLengthMoved)
                {
                    // float distance = mPathLengthMoved - mTriggers[0].mTriggerLength;
                    // 触发器处理
                    mTriggers[0].OnTrigger(mTriggerHandler);
                    // 移除触发器
                    mTriggers.RemoveAt(0);
                }
                else
                {
                    // 没有触发器被触发，结束
                    break;
                }
            }
        }

        /// <summary>
        /// 绑定触发器
        /// </summary>
        protected void RegisterAllTriggers()
        {
            mTriggers.Clear();
            RegisterStartTrigger();
            RegisterTriggers();
            RegisterEndTrigger();
            SortTriggerByLength();
        }

        /// <summary>
        /// 绑定开始触发器
        /// </summary>
        public void RegisterStartTrigger()
        {
            // -1 随便取，只是个排序字段
            RegisterTrigger(new PathTriggerStart(-1));
        }

        /// <summary>
        /// 绑定结束触发器
        /// </summary>
        public void RegisterEndTrigger()
        {
            RegisterTrigger(new PathTriggerEnd(PathLength - 0.001f));
        }

        /// <summary>
        /// 绑定指定的触发器Id
        /// </summary>
        /// <param name="triggerId">触发器Id，见 RFPathTriggerData</param>
        /// <param name="triggerLength">触发器在路径上的长度</param>
        public void RegisterTrigger(int triggerId, float triggerLength)
        {
            RegisterTrigger(new PathTriggerIndex(triggerId, triggerLength));
        }

        /// <summary>
        /// 注册编辑器下所有的触发器
        /// </summary>
        public void RegisterTriggers()
        {
            foreach (int id in mPathData.Triggers)
            {
                PathTriggerData trigger = PathTriggerData.Get(id);
                if (trigger != null)
                {
                    RegisterTrigger(id, trigger.Length);
                }
                else
                {
                    DebugUtils.Log(InfoType.Info, "RegisterTriggers" + " nil RFPathTriggerData " + id);
                }
            }
        }

        /// <summary>
        /// 添加指定触发器
        /// </summary>
        /// <param name="trigger">触发器对象</param>
        /// <param name="sortImmediate">是否立即排序</param>
        public void RegisterTrigger(PathTrigger trigger, bool sortImmediate = false)
        {
            mTriggers.Add(trigger);
            if (sortImmediate)
            {
                SortTriggerByLength();
            }
        }

        /// <summary>
        /// 排序所有的触发器
        /// </summary>
        public void SortTriggerByLength()
        {
            if (mTriggers.Count > 1)
            {
                mTriggers.Sort(PathTriggerSort.TriggerSortInstance);
            }
        }

        /// <summary>
        /// 插入一个触发器
        /// </summary>
        /// <param name="relateCurrentPos">length是否相对当前位置</param>
        /// <param name="length">长度值</param>
        /// <param name="callback">触发器回调</param>
        public void InsertTriggerByLength(bool relateCurrentPos, float length, int pathTriggerId)
        {
            float absoluteLength = length;
            if (relateCurrentPos)
            {
                absoluteLength += mPathLengthMoved;
            }
            if (absoluteLength > mPathData.PathLength)
            {
                absoluteLength = mPathData.PathLength;
            }
            RegisterTrigger(new PathTriggerIndex(pathTriggerId, absoluteLength));
        }

        /// <summary>
        /// 插入一个触发器
        /// </summary>
        /// <param name="relateCurrentPos">length是否相对当前位置</param>
        /// <param name="length">长度值</param>
        /// <param name="callback">触发器回调</param>
        public void InsertTriggerByLength(bool relateCurrentPos, float length, Callback callback)
        {
            if (callback == null)
            {
                return;
            }
            float absoluteLength = length;
            if (relateCurrentPos)
            {
                absoluteLength += mPathLengthMoved;
            }
            if (absoluteLength > mPathData.PathLength)
            {
                absoluteLength = mPathData.PathLength;
            }
            RegisterTrigger(new PathTriggerEvent(absoluteLength, callback), true);
        }

        /// <summary>
        /// 插入一个触发器:根据speed和time计算长度值
        /// </summary>
        /// <param name="relateCurrentPos">是否相对当前位置</param>
        /// <param name="speed">速度</param>
        /// <param name="time">时间</param>
        /// <param name="callback">触发器回调</param>
        public void InsertTriggerByTime(bool relateCurrentPos, float speed, float time, Callback callback)
        {
            InsertTriggerByLength(relateCurrentPos, speed * time, callback);
        }

        /// <summary>
        /// 插入一个触发器
        /// </summary>
        /// <param name="relateCurrentPos">是否相对当前位置</param>
        /// <param name="percent">路径长度值的占比</param>
        /// <param name="callback">触发器回调</param>
        public void InsertTriggerByPercent(bool relateCurrentPos, float percent, Callback callback)
        {
            InsertTriggerByLength(relateCurrentPos, percent * mPathData.PathLength, callback);
        }
    }

    public abstract partial class AbstractNavPath
    {      
        protected Vector3 mOffset;                  // 路径偏移     
        protected NavPathFlipType bFlipType;        // 开启镜像
        protected NavPathData mPathData;            // 路径数据   
        protected float mPathLengthMoved;           // 运行时已经移动的长度
        protected int mCurrentWaypointIndex;        // 运行时当前的路点索引
        protected Vector3[] mWaypointAppend;        // 首尾两点的控制点
        protected List<PathTrigger> mTriggers;      // 运行时触发器列表
        protected IPathTrigger mTriggerHandler;  // 触发器处理实例
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pathData">见 NavPathData </param>
        /// <param name="offset">路径偏移</param>
        /// <param name="pathFlipOn">路径中心对称(x, y, z) -> (-x, y, -z)</param>
        public AbstractNavPath(NavPathData pathData, Vector3 offset, NavPathFlipType flipType, IPathTrigger triggerHandler)
        {
            mOffset = offset;
            bFlipType = flipType;
            mPathLengthMoved = 0.0f;
            mCurrentWaypointIndex = 0;
            mTriggers = new List<PathTrigger>();
            CurInfo = new NavPathPoint();
            mWaypointAppend = new Vector3[2];
            mTriggerHandler = triggerHandler;
            mPathData = pathData;
            Initialize();
        }

        /// <summary>
        /// 更新路径数据
        /// </summary>
        /// <param name="moved">移动长度</param>
        /// <returns>当前时刻所处路径的点坐标和切向量</returns>
        public NavPathPoint UpdatePath(float moved)
        {
            // 累加已走路径长度
            mPathLengthMoved += moved;
            // 是否走完路径
            CurInfo.isFinished = IsFinished();
            if (!CurInfo.isFinished)
            {
                // 更新路点索引
                UpdateWaypointIndex();
                // 更新插值数据：位置和切向量 
                UpdatePosAndTangent();
            }
            // 触发器执行
            UpdateTrigger();
            return CurInfo;
        }

        /// <summary>
        /// 初始化数据，并指定触发器响应对象
        /// </summary>
        /// <param name="triggerHandler">触发器处理对象</param>
        protected virtual void Initialize()
        {
            InitializeAppendWaypoint();
            RegisterAllTriggers();
            UpdatePosAndTangent();
        }

        /// <summary>
        /// 计算当前路径点和切向
        /// </summary>
        protected abstract void UpdatePosAndTangent();

        /// <summary>
        /// 一个点为定点
        /// 首尾两点的控制点计算
        /// </summary>
        protected void InitializeAppendWaypoint()
        {
            Debug.Assert(mPathData.WayPoints.Count > 0, "wrong");
            if (mPathData.WayPoints.Count == 1)
            {
                // nothing todo
            }
            else if(mPathData.WayPoints.Count == 2)
            {
                Vector3 diff1 = mPathData.WayPoints[0] - mPathData.WayPoints[1];
                Vector3 diff2 = mPathData.WayPoints[mPathData.WayPoints.Count - 1] - mPathData.WayPoints[mPathData.WayPoints.Count - 2];
                mWaypointAppend[0] = mPathData.WayPoints[0] + diff1;
                mWaypointAppend[1] = mPathData.WayPoints[mPathData.WayPoints.Count - 1] + diff2;
            }
            else
            {
                Vector3 diff = (2.0f * mPathData.WayPoints[1] - mPathData.WayPoints[0] - mPathData.WayPoints[2]) * 0.5f;
                mWaypointAppend[0] = mPathData.WayPoints[0] + diff;
                diff = (2.0f * mPathData.WayPoints[mPathData.WayPoints.Count - 2] - mPathData.WayPoints[mPathData.WayPoints.Count - 3] - mPathData.WayPoints[mPathData.WayPoints.Count - 1]) * 0.5f;
                mWaypointAppend[1] = mPathData.WayPoints[mPathData.WayPoints.Count - 1] + diff;
            }
        }

        /// <summary>
        /// 路径总长度获取
        /// </summary>
        public float PathLength { get { return mPathData.PathLength; } }

        /// <summary>
        /// 路径触发器个数
        /// </summary>
        public int TriggerCount { get { return mTriggers.Count; } }

        /// <summary>
        /// 获取当前信息
        /// </summary>
        public NavPathPoint CurInfo { get; set; }

        /// <summary>
        /// 更新路点索引
        /// </summary>
        /// <returns></returns>
        protected virtual int UpdateWaypointIndex()
        {
            int lastWayIndex = mCurrentWaypointIndex;
            int nextWayIndex = mCurrentWaypointIndex + 1;
            while (mPathData.RangeLengths[nextWayIndex] < mPathLengthMoved)
            {
                mCurrentWaypointIndex = nextWayIndex++;
            }
            CurInfo.isDirChanged = lastWayIndex != mCurrentWaypointIndex;
            return lastWayIndex;
        }

        /// <summary>
        /// 路径是否已更新完
        /// </summary>
        /// <returns>路径是否完毕</returns>
        protected bool IsFinished()
        {
            return mPathLengthMoved >= mPathData.PathLength;
        }

        /// <summary>
        /// 获取路点
        /// </summary>
        /// <param name="index">路点索引</param>
        /// <returns></returns>
        protected Vector3 GetWaypoint(int index)
        {
            Vector3 pos = mPathData.WayPoints[index] + mOffset;
            NavPathUtils.Flip(bFlipType, ref pos);
            return pos;
        }

        /// <summary>
        /// 获取当前路点
        /// </summary>
        /// <returns>返回当前路点</returns>
        protected Vector3 GetCurrentWaypoint()
        {
            return GetWaypoint(mCurrentWaypointIndex);
        }

        /// <summary>
        /// 获取路径终点
        /// </summary>
        /// <returns>返回终点</returns>
        protected Vector3 EndPoint()
        {
            return GetWaypoint(mPathData.WayPoints.Count - 1);
        }

        /// <summary>
        /// 获取指定路点相对起始点的长度
        /// </summary>
        /// <param name="index">路点索引</param>
        /// <returns>路径长度</returns>
        protected float GetLength(int index)
        {
            return mPathData.RangeLengths[index];
        }
    }
}
