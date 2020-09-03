using System;
using System.Collections.Generic;

namespace Nullspace
{
    public class StateController<T>
    {
        private Dictionary<string, GenericValue> mParameters;
        //private StateEntity<T> mEntryState;
        //private StateEntity<T> mExitState;
        //private StateEntity<T> mAnyStates;
        private List<StateEntity<T>> mStateSet;

        public StateController()
        {
            mParameters = new Dictionary<string, GenericValue>();
            mStateSet = new List<StateEntity<T>>();
        }

        public StateEntity<T> Current { get; set; }

        public T State { get { return Current.StateType; } }

        /// <summary>
        /// 添加一个新状态
        /// </summary>
        /// <param name="stateType">状态类别</param>
        /// <returns></returns>
        public StateEntity<T> AddState(T stateType)
        {
            StateEntity<T> entity = GetEntity(stateType);
            if (entity == null)
            {
                entity = new StateEntity<T>(stateType, this);
                mStateSet.Add(entity);
            }
            return entity;
        }

        private StateEntity<T> GetEntity(T stateType)
        {
            int index = mStateSet.FindIndex((item) => { return stateType.Equals(item.StateType); });
            StateEntity<T> entity = null;
            if (index >= 0)
            {
                entity = mStateSet[index];
            }
            return entity;
        }

        public void Set<U>(string paraName, U value)
        {
            if (mParameters.ContainsKey(paraName))
            {
                StateGenericValue<U> param = mParameters[paraName] as StateGenericValue<U>;
                DebugUtils.Assert(param != null, "");
                if (!param.Value.Equals(value))
                {
                    param.Value = value;
                    Update(paraName);
                }
            }
        }

        public void Update(string paramName)
        {
            if (Current != null && Current.ContainParameter(paramName))
            {
                T nextTransfer;
                bool isDirty = Current.CheckTransfer(out nextTransfer);
                if (isDirty)
                {
                    DebugUtils.Assert(!nextTransfer.Equals(Current.StateType), "wrong changed state");
                    ChangeStatus(nextTransfer);
                }
            }
        }

        private void ChangeStatus(T nextState)
        {
            StateEntity<T> nextEntity = GetEntity(nextState);
            Current.Exit();
            nextEntity.Enter();
            nextEntity.Process();
        }

        /// <summary>
        /// 该控制参数名不能重复
        /// </summary>
        /// <param name="paraName">参数名</param>
        /// <param name="dataType">参数类型</param>
        /// <param name="ctlValue">参数控制值</param>
        /// <returns></returns>
        public StateController<T> AddParameter<P>(string paraName, P ctlValue)
        {
            if (!mParameters.ContainsKey(paraName))
            {
                GenericValue value = GenericValue.CreateGenericValue(paraName, ctlValue);
                mParameters.Add(paraName, value);
            }
            else
            {
                // duplicated
            }
            return this;
        }


        public bool CheckCondition(GenericValue condition)
        {
            DebugUtils.Assert(condition != null, "wrong condition");
            GenericValue param = mParameters[condition.Name];
            DebugUtils.Assert(param != null, "wrong parameter");
            return condition.CompareTo(param);
        }
    }
}
