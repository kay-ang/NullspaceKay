
using System;

namespace Nullspace
{
    public enum BTConditionBoolean
    {
        AND,
        OR,
    }

    // AND
    // a < b < c 
    // a < b <= c 
    // a <= b < c 
    // a <= b <= c 
    // and so on

    // OR
    // a < b or b > c 
    // a < b or b >= c 
    // a > b or b < c 
    // a >= b or b <= c
    // and so on
    public class BTConditionRangeOperation<V, Target> : BehaviourTreeNode<Target>
    {
        private BTConditionSingleOperation<V, Target> LeftOperation;
        private BTConditionSingleOperation<V, Target> RightOperation;
        private BTConditionBoolean RangeBoolean; // || && 
        public BTConditionRangeOperation(BTConditionBoolean rangeBoolean, ConditionOperationType leftOperation, V leftValue, ConditionOperationType rightOperation, V rightValue, Func<V> getter) : base()
        {
            LeftOperation = new BTConditionSingleOperation<V, Target>(leftOperation, leftValue, getter);
            RightOperation = new BTConditionSingleOperation<V, Target>(leftOperation, rightValue, getter);
            RangeBoolean = rangeBoolean;
        }

        public override BTNodeState Process(Target obj)
        {
            BTNodeState state = LeftOperation.Process(obj);
            if (RangeBoolean == BTConditionBoolean.AND)
            {
                if (state == BTNodeState.Success)
                {
                    state = RightOperation.Process(obj);
                }
            }
            else if (RangeBoolean == BTConditionBoolean.OR)
            {
                if (state == BTNodeState.Failure)
                {
                    state = RightOperation.Process(obj);
                }
            }

            return state;
        }

        public override void Reset()
        {
            
        }

        public override void Clear()
        {
            
        }
    }
}
