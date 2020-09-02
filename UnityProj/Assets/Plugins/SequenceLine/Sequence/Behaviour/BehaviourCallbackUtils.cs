
namespace Nullspace
{
    internal class BehaviourCallbackUtils
    {
        /// <summary>
        /// 只存在begine 和 end，不执行 process
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="duration"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        public static void Create(float startTime, float duration, Callback begin = null, Callback end = null)
        {

        }

        /// <summary>
        /// 每帧执行 process
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="duration"></param>
        /// <param name="begin"></param>
        /// <param name="process"></param>
        /// <param name="end"></param>
        public static void Create(float startTime, float duration, Callback begin = null, Callback process = null, Callback end = null)
        {

        }

        /// <summary>
        /// 在 duration 时间内，执行 targetFrameCount 次 process
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="duration"></param>
        /// <param name="targetFrameCount"></param>
        /// <param name="begin"></param>
        /// <param name="process"></param>
        /// <param name="end"></param>
        public static void Create(float startTime, float duration, int targetFrameCount, Callback begin = null, Callback process = null, Callback end = null)
        {

        }


    }
}
