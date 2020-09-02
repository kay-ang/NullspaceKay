
namespace Nullspace
{
    public abstract class BTConditionNode<Target> : BehaviourTreeNode<Target>
    {
        public override BTNodeState Process(Target obj)
        {
            return BTNodeState.Ready;
        }
    }
    
}
