
using System;

namespace Nullspace
{
    public class BTDecoratorNode<Target> : BehaviourTreeNode<Target>
    {
        protected BehaviourTreeNode<Target> mChild = null;

        public override void Clear()
        {
            mChild.Clear();
        }

        public override BTNodeState Process(Target obj)
        {
            return BTNodeState.Ready;
        }

        public void Proxy(BehaviourTreeNode<Target> child)
        {
            mChild = child;
        }

        public override void Reset()
        {
            mChild.Reset();
        }
    }

}
