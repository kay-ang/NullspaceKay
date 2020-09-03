
namespace Nullspace
{

    // 直到 fail
    public abstract class BTUtilFailureNode<Target> : BTDecoratorNode<Target>
    {
        public override BTNodeState Process(Target obj)
        {
            if (mChild.Process(obj) == BTNodeState.Failure)
            {
                return BTNodeState.Success;
            }
            return BTNodeState.Running;
        }
    }
}
