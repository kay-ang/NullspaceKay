
namespace Nullspace
{
    /// <summary>
    /// 触发器接口
    /// </summary>
    public interface IPathTrigger
    {
        /// <summary>
        /// 出生时触发
        /// </summary>
        void OnPathStart();

        /// <summary>
        /// 游玩路径时触发
        /// </summary>
        void OnPathEnd();

        /// <summary>
        /// 静态触发器处理
        /// </summary>
        /// <param name="triggerId">触发器id，查询触发器配置参数</param>
        void OnPathTrigger(int triggerId);

        /// <summary>
        /// 触发器通过注册绑定回调函数
        /// </summary>
        /// <param name="callback"></param>
        void OnPathTrigger(Callback callback);
    }
}
