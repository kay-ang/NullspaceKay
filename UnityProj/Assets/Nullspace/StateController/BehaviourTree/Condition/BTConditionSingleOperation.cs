using System;

namespace Nullspace
{
    // <,>,==,>=,<=,!=
    public class BTConditionSingleOperation<V, Target> : BehaviourTreeNode<Target>
    {
        private ConditionValue<V> mTargetValueCondition;
        private Func<V> mGetter;
        public BTConditionSingleOperation(ConditionOperationType operation, V targetValue, Func<V> getter) : base()
        {
            mTargetValueCondition = GenericValue.CreateConditionValue<V>("", targetValue, operation);
            mGetter = getter;
            DebugUtils.Assert(mGetter != null, "");
            DebugUtils.Assert(mTargetValueCondition != null, "");
        }

        public override BTNodeState Process(Target obj)
        {
            V value = mGetter();
            bool result = mTargetValueCondition.CompareTo(value);
            return result ? BTNodeState.Success : BTNodeState.Failure;
        }

        public override void Reset()
        {
            
        }

        public override void Clear()
        {
            
        }
    }


}
