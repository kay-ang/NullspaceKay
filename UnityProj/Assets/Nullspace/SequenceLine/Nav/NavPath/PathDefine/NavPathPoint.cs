using UnityEngine;

namespace Nullspace
{
    /// <summary>
    /// 记录所处路径当前时刻的信息
    /// </summary>
    public class NavPathPoint
    {
        // 当前时刻 曲线位置坐标
        public Vector3 curvePos = Vector3.zero;
        // 当前时刻 曲线位置切线
        public Vector3 curveDir = Vector3.zero;
        // 当前时刻 折线位置坐标
        public Vector3 linePos = Vector3.zero;
        // 当前时刻 折线位置切线
        public Vector3 lineDir = Vector3.zero;
        // 当前时刻 所处折线段是否改变
        public bool isDirChanged = false;
        // 当前时刻 所有折线段是否走完
        public bool isFinished = false;
    }

}
