
namespace Nullspace
{
    public abstract class BehaviourTreeNode<Target>
    {
        protected BTNodeState mNodeState;
        public string Name { get; set; }

        public BehaviourTreeNode()
        {
            mNodeState = BTNodeState.Ready;
        }

        public virtual void Enter(Target obj)
        {

        }

        public virtual BTNodeState Run(Target obj)
        {
            Enter(obj);
            mNodeState = Process(obj);
            Leave(obj);
            return mNodeState;
        }

        public virtual void Leave(Target obj)
        {
            DebugUtils.Log(InfoType.Info, string.Format("Leave {0} {1}", Name, mNodeState));
        }

        public abstract void Reset();
        public abstract void Clear();

        public abstract BTNodeState Process(Target obj);
    }

}
