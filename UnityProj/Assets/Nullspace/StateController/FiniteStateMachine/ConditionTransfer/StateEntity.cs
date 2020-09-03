using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nullspace
{
    public class StateEntity<T> : IState<T>
    {
        private List<T> mNextStates;
        private List<StateConditions> mNextConditions;
        private StateController<T> mOwnerCtl;

        public StateEntity(T stateType, StateController<T> ctl)
        {
            StateType = stateType;
            mOwnerCtl = ctl;
            mNextStates = new List<T>();
            mNextConditions = new List<StateConditions>();
        }

        public T StateType { get; set; }


        public StateEntity<T> AsCurrent()
        {
            mOwnerCtl.Current = this;
            return this;
        }

        /// <summary>
        /// 一个状态转移到下一个状态
        /// </summary>
        /// <param name="nextStateType">下一个状态类别</param>
        /// <returns>状态转移条件</returns>
        public override StateConditions AddTransfer(T nextStateType)
        {
            StateConditions conditions = null;
            int index = mNextStates.FindIndex((item) => { return item.Equals(nextStateType); });
            if (index < 0)
            {
                // 添加到列表
                mNextStates.Add(nextStateType);
                // 转移条件列表
                conditions = new StateConditions();
                mNextConditions.Add(conditions);
            }
            else
            {
                conditions = mNextConditions[index];
            }
            return conditions;
        }

        public bool ContainParameter(string parameterName)
        {
            foreach (StateConditions conditions in mNextConditions)
            {
                if (conditions.Contain(parameterName))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckTransfer(out T nextStateType)
        {
            int cnt = mNextConditions.Count;
            nextStateType = StateType;
            for (int i = 0; i < cnt; ++i)
            {
                bool isSuccess = mNextConditions[i].IsSuccess(mOwnerCtl);
                if (isSuccess)
                {
                    nextStateType = mNextStates[i];
                    return true;
                }
            }
            return false;
        }

        public override void Enter()
        {
            AsCurrent();
            base.Enter();
        }

    }
}
