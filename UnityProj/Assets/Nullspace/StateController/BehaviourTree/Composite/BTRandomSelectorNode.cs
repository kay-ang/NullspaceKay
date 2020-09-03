
namespace Nullspace
{
    public class BTRandomSelectorNode<Target> : BTCompositeNode<Target>
    {
        public override BTNodeState Process(Target obj)
        {
            int[] seq = MathUtils.RandomShuffle(mChildren.Count);
            foreach (int index in seq)
            {
                if (mChildren[index].Run(obj) == BTNodeState.Success)
                {
                    return BTNodeState.Success;
                }
            }
            return BTNodeState.Failure;
        }
    }
}
