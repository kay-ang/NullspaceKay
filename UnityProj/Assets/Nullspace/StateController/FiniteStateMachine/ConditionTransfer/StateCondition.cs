
using System.Collections.Generic;

namespace Nullspace
{
    public class StateConditions
    {
        private List<GenericValue> mConditions;

        public StateConditions()
        {
            mConditions = new List<GenericValue>();
        }

        /// <summary>
        /// 添加条件
        /// </summary>
        /// <param name="parameterName">条件参数名</param>
        /// <param name="comType">比较类别</param>
        /// <param name="value">目标值</param>
        /// <returns></returns>
        public StateConditions With<T>(string parameterName, ConditionOperationType cot, T value)
        {
            // 这里不去重复
            // 比如 条件是一个区间
            GenericValue condition = GenericValue.CreateConditionValue(parameterName, value, cot);
            mConditions.Add(condition);
            return this;
        }

        public bool IsSuccess<T>(StateController<T> controller)
        {
            foreach (GenericValue condition in mConditions)
            {
                if (!controller.CheckCondition(condition))
                {
                    return false;
                }
            }
            return true;
        }

        public bool Contain(string parameterName)
        {
            int index = mConditions.FindIndex((item) => { return item.Name == parameterName; });
            return index >= 0;
        }
    }

    


    

}
