
using System.Collections;

namespace Nullspace
{
    public class ConditionValue<T> : StateGenericValue<T>
    {
        public ConditionOperationType CompareType;
        public ConditionValue(string name, T v, ConditionOperationType cot) : base(name, v)
        {
            CompareType = cot;
        }
        public bool CompareTo(T v)
        {
            // 条件值 与 参数目标值 比较
            int compare = Comparer.Default.Compare(Value, v);
            switch (CompareType)
            {
                case ConditionOperationType.EQUAL:
                    return compare == 0;
                case ConditionOperationType.GREATER:
                    return compare == 1;
                case ConditionOperationType.GREATER_EQUAL:
                    return compare != -1;
                case ConditionOperationType.LESS:
                    return compare == -1;
                case ConditionOperationType.LESS_EQUAL:
                    return compare != 1;
                case ConditionOperationType.NOT_EQUAL:
                    return compare != 0;
            }
            return false;
        }

        public override bool CompareTo(GenericValue param)
        {
            StateGenericValue<T> p = param as StateGenericValue<T>;
            DebugUtils.Assert(p != null, "");
            return CompareTo(p.Value);
        }
    }
}
