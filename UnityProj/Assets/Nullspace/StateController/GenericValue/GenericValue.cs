
using System.Collections.Generic;

namespace Nullspace
{
    public class GenericValue
    {
        public static GenericValue CreateGenericValue<T>(string name, T v)
        {
            return new StateGenericValue<T>(name, v);
        }
        public static ConditionValue<T> CreateConditionValue<T>(string name, T v, ConditionOperationType cot)
        {
            return new ConditionValue<T>(name, v, cot);
        }

        /// <summary>
        /// StateConditionValue.CompareTo(StateGenericValue<T>)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual bool CompareTo(GenericValue param) { return false; }
        public string Name;
    }

    public class StateGenericValue<T> : GenericValue, IEqualityComparer<T>
    {
        public T Value;
        public StateGenericValue(string name, T v)
        {
            Name = name;
            Value = v;
        }

        public bool Equals(T x, T y)
        {
            return EqualityComparer<T>.Default.Equals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }

}
