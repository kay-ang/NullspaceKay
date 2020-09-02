
namespace Nullspace
{
    // 取非
    public class BTInvertNode<Target> : BTDecoratorNode<Target>
    {
        public override BTNodeState Process(Target obj)
        {
            mNodeState = mChild.Process(obj);
            if (mNodeState == BTNodeState.Failure)
            {
                mNodeState = BTNodeState.Success;
            }
            else if (mNodeState == BTNodeState.Success)
            {
                mNodeState = BTNodeState.Failure;
            }
            return mNodeState;
        }
    }
}
