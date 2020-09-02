
namespace Nullspace
{
    /// <summary>
    /// 子系统事件接口
    /// </summary>
    public interface IEventManager
    {
        /// <summary>
        /// 添加事件订阅。
        /// </summary>
        void AddListeners();
        /// <summary>
        /// 移除事件订阅。
        /// </summary>
        void RemoveListeners();
    }
}
