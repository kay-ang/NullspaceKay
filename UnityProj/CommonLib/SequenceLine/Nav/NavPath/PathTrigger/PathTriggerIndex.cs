
using GameData;
using UnityEngine;

namespace Nullspace
{
    /// <summary>
    /// 自定义触发器定义
    /// </summary>
    public class PathTriggerIndex : PathTrigger
    {
        // 触发器索引Id，见 RFPathTriggerData
        protected int mTriggerId;

        public PathTriggerIndex(int id, float length)
        {
            mTriggerId = id;
            TriggerLength = length;
        }

        public override void OnDrawGizmos(Vector3 pos)
        {
            Gizmos.DrawSphere(pos, 0.5f);
            PathTriggerData data = PathTriggerData.Get(mTriggerId);
#if UNITY_EDITOR
            if (data.Type == BuffType.PAUSE_MOVE)
            {
                UnityEditor.Handles.Label(pos, string.Format("暂停{0}秒", data.Duration));
            }
            if (data.Type == BuffType.ACCELERATA_MOVE)
            {
                UnityEditor.Handles.Label(pos, string.Format("{0}%加速{1}秒", data.Accelerate * 100, data.Duration));
            }
            if (data.Type == BuffType.DECELERATA_MOVE)
            {
                UnityEditor.Handles.Label(pos, string.Format("{0}%减速{1}秒", data.Accelerate * 100, data.Duration));
            }
            if (data.Type == BuffType.FREEZE_MOVE)
            {
                UnityEditor.Handles.Label(pos, string.Format("冰冻{0}秒", data.Duration));
            }
#endif
        }

        public override void OnTrigger(IPathTrigger handler)
        {
            if (handler != null)
            {
                handler.OnPathTrigger(mTriggerId);
            }
        }
    }
}
