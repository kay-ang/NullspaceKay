
namespace Nullspace
{
    public class BTSequenceNode<Target> : BTCompositeNode<Target>
    {
        public override BTNodeState Process(Target obj)
        {
            for (int i = 0; i < mChildren.Count; ++i)
            {
                mNodeState = mChildren[i].Run(obj);
                if (mNodeState == BTNodeState.Running)
                {
                    return mNodeState;
                }
                if (mNodeState == BTNodeState.Failure)
                {
                    return mNodeState;
                }
            }
            mNodeState = BTNodeState.Success;
            return mNodeState;
        }
    }

}
