
namespace Nullspace
{
    // 直到 success
    public class BTUntilSuccessNode<Target> : BTDecoratorNode<Target>
    {
        public override BTNodeState Process(Target obj)
        {
            if (mChild.Process(obj) == BTNodeState.Success)
            {
                return BTNodeState.Success;
            }
            return BTNodeState.Running;
        }
    }

}
