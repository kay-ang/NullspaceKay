
using System;
using System.Collections.Generic;

namespace Nullspace
{
    public class BTCompositeNode<Target> : BehaviourTreeNode<Target>
    {
        protected List<BehaviourTreeNode<Target>> mChildren;
        public BTCompositeNode() : base()
        {
            mChildren = new List<BehaviourTreeNode<Target>>();
        }

        public void AddChild(BehaviourTreeNode<Target> node)
        {
            mChildren.Add(node);
        }

        public override void Clear()
        {
            foreach (var node in mChildren)
            {
                node.Clear();
            }
        }

        public override BTNodeState Process(Target obj)
        {
            return BTNodeState.Ready;
        }

        public override void Reset()
        {
            foreach (var node in mChildren)
            {
                node.Reset();
            }
        }
    }
}
