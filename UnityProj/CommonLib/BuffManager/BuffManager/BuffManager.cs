using System;
using System.Collections;
using System.Collections.Generic;

namespace Nullspace
{
    public class BuffManager : CollectionUpdateLock, IEnumerable<Buff>
    {
        public event Action<Buff> OnBuffAwake;

        private List<Buff> mBuffs = new List<Buff>();

        protected IntValueStack mBuffTags = new IntValueStack();
        protected IntValueStack mBuffImmuneTags = new IntValueStack();

        public static uint BuffTypeToMask(BuffType type)
        {
            return EnumUtils.EnumToBaseType<BuffType, uint>(type);
        }

        public int Count
        {
            get
            {
                return mBuffs.Count;
            }
        }

        public Buff this[int index]
        {
            get
            {
                DebugUtils.Assert(index < Count, "");
                return mBuffs[index];
            }
        }

        public IEnumerator<Buff> GetEnumerator()
        {
            return mBuffs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mBuffs.GetEnumerator();
        }

        /// <summary>
        /// 此时还未添加到 mBuffs 容器
        /// </summary>
        /// <param name="bufferId"></param>
        /// <param name="caster"></param>
        /// <param name="ability"></param>
        /// <returns></returns>
        public bool CreateBuff(int bufferId, object caster, object ability)
        {
            // 获取Buff配置信息
            BuffData data = BuffData.Get(bufferId);
            DebugUtils.Assert(data != null, "bufferId " + bufferId);
            // 被免疫，不添加
            if (IsImmune(data.BuffTag))
            {
                return false;
            }
            // Buff实例化
            Buff buff = new Buff(data, this, caster, ability);
            OnBuffAwake?.Invoke(buff);
            return true;
        }

        /// <summary>
        /// 新添加的Buff可能是取消其他存在的Buff
        /// </summary>
        public void ImmuneExistBuff()
        {

        }

        public void RemoveImmuneTag(ValueModifier<int> tag)
        {
            mBuffImmuneTags.RemoveModifier(tag);
        }

        public ValueModifier<int> AddImmuneTag(int tag)
        {
            return mBuffImmuneTags.AddModifier(tag);
        }

        protected bool IsImmune(int tag)
        {
            return (mBuffImmuneTags.Value & tag) != 0;
        }

        /// <summary>
        /// 检查是否已经存在同一个Buff
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public Buff Find(BuffData template)
        {
            for (int i = mBuffs.Count - 1; i >= 0; i--)
            {
                var buff = mBuffs[i];
                if (buff.mBuffData == template)
                {
                    return buff;
                }
            }
            return null;
        }

        public void Update(float deltaTime)
        {
            LockUpdate();
            for (int i = mBuffs.Count - 1; i >= 0; i--)
            {
                var buff = mBuffs[i];
                buff.Update(deltaTime);
            }
            UnLockUpdate();
        }

    }
}
