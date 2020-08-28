
using UnityEngine;

namespace Nullspace
{
    /// <summary>
    /// 自定义回调触发器定义
    /// </summary>
    public class PathTriggerEvent : PathTrigger
    {
        // 触发回调
        protected Callback mCallback;

        public PathTriggerEvent(float length, Callback callback)
        {
            TriggerLength = length;
            mCallback = callback;
        }

        public override void OnDrawGizmos(Vector3 pos)
        {
            Gizmos.DrawSphere(pos, 1);
#if UNITY_EDITOR
            UnityEditor.Handles.Label(pos, "event");
#endif
        }

        public override void OnTrigger(IPathTrigger handler)
        {
            if (handler != null)
            {
                handler.OnPathTrigger(mCallback);
            }
        }
    }
}
