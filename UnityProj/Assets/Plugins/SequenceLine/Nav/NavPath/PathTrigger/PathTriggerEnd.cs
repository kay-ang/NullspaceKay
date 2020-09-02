
using UnityEngine;

namespace Nullspace
{
    /// <summary>
    /// 结束触发器定义
    /// </summary>

    public class PathTriggerEnd : PathTrigger
    {
        public PathTriggerEnd(float length)
        {
            TriggerLength = length;
        }

        public override void OnDrawGizmos(Vector3 pos)
        {
            Gizmos.DrawSphere(pos, 1);
#if UNITY_EDITOR
            UnityEditor.Handles.Label(pos, "end");
#endif
        }

        public override void OnTrigger(IPathTrigger handler)
        {
            if (handler != null)
            {
                handler.OnPathEnd();
            }
        }
    }
}
