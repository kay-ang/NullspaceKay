
using UnityEngine;

namespace Nullspace
{
    /// <summary>
    /// 开始触发器定义
    /// </summary>
    public class PathTriggerStart : PathTrigger
    {
        public PathTriggerStart(float length)
        {
            TriggerLength = length;
        }

        public override void OnDrawGizmos(Vector3 pos)
        {
            Gizmos.DrawSphere(pos, 1);
#if UNITY_EDITOR
            UnityEditor.Handles.Label(pos, "start");
#endif
        }

        public override void OnTrigger(IPathTrigger handler)
        {
            if (handler != null)
            {
                handler.OnPathStart();
            }
        }
    }

}
