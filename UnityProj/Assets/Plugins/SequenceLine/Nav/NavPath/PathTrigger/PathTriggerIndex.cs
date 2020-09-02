
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
