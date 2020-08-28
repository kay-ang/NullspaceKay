
using System;

namespace Nullspace
{
    public class BehaviorTreeRoot<Target> : BehaviourTreeNode<Target>
    {
        public BehaviourTreeNode<Target> Root { get; set; }
        public BehaviorTreeRoot()
        {
            Name = "root";
            Root = null;
        }

        public BehaviorTreeRoot(BehaviourTreeNode<Target> root)
        {
            Root = root;
            Name = "root";
        }

        public override BTNodeState Process(Target obj)
        {
            mNodeState = Root.Run(obj);
            return mNodeState;
        }

        public override void Reset()
        {
            Root.Reset();
        }

        public override void Clear()
        {
            Root.Clear();
        }
    }
}
