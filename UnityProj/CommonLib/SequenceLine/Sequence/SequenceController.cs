using System;
using System.Collections;
using System.Collections.Generic;

namespace Nullspace
{
    /// <summary>
    /// 只负责控制另一个 Sequence的 暂定和恢复
    /// </summary>
    public class SequenceController<T> : SequenceMultipleDynamic where T : BehaviourCollection
    {
        public T Target;

        internal SequenceController(T target) : base()
        {
            Target = target;
        }

        public override bool IsPlaying()
        {
            return mState == ThreeState.Playing || (Target != null && Target.IsPlaying());
        }

        public override void Update(float deltaTime)
        {
            // 执行基类
            base.Update(deltaTime);
            if (!base.IsPlaying() && Target != null)
            {
                Target.Update(deltaTime);
            }
        }

        public override void Clear()
        {
            base.Clear();
        }

        public override void Reset()
        {
            base.Reset();
            if (Target != null)
            {
                Target.Reset();
            }
        }


        public bool Pause(string tag)
        {
            return Pause(tag, 100000000f); // 默认值
        }

        public bool Pause(string tag, float duration)
        {
            // 是否存在
            if (FindBehaviour(tag) != null) // unique tag
            {
                DebugUtils.Log(InfoType.Warning, "duplicated tag: " + tag);
                return false;
            }
            PauseCallback ec = new PauseCallback(tag, mMaxDuration, duration);
            Insert(ElappsedTime, ec, duration);
            return true;
        }

        /// <summary>
        /// 这里指挥清理一个，添加的时候确保只有一个 Tag
        /// </summary>
        /// <param name="tag"></param>
        public bool Resume(string tag)
        {
            BehaviourCallback bc = FindBehaviour(tag);
            if (bc != null)
            {
                mBehaviours.Remove(bc);
                return true;
            }
            return false;
        }

        private BehaviourCallback FindBehaviour(string tag)
        {
            BehaviourCallback res = FindBehaviour(tag, mAddBehaviourCaches);
            if (res == null)
            {
                res = FindBehaviour(tag, mBehaviours);
            }
            return res;
        }

        private BehaviourCallback FindBehaviour(string tag, LinkedList<BehaviourCallback> lst)
        {
            // 队列中查找
            foreach (BehaviourCallback bc in lst)
            {
                PauseCallback pc = bc as PauseCallback;
                if (pc != null && pc.Tag == tag && !bc.IsFinished)
                {
                    return pc;
                }
            }
            return null;
        }

    }
}
