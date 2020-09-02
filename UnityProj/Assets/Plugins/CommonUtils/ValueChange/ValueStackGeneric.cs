using System;
using System.Collections.Generic;

namespace Nullspace
{
    public abstract class ValueStackGeneric<T>
    {
        protected T mBaseValue;
        protected T mLastValue;
        protected List<ValueModifier<T>> mModifiers;
        internal bool IsDirty = true;
        public Action<T> OnValueChanged;
        public ValueStackGeneric()
        {
            mModifiers = new List<ValueModifier<T>>();
        }

        public T Delta { get; set; }

        protected abstract void ResetDelta();

        protected abstract T Add(T l, T r);

        public void InvokeChanged()
        {
            ResetDelta();
            mLastValue = Value;
            OnValueChanged?.Invoke(Value);
        }

        public T Value
        {
            get
            {
                if (IsDirty)
                {
                    T modifiedValue = mBaseValue;
                    modifiedValue = Add(modifiedValue, Sum(mModifiers));
                    mLastValue = modifiedValue;
                }
                return mLastValue;
            }
            set
            {
                BaseValue = value;
            }
        }

        protected T Sum(List<ValueModifier<T>> modifiers)
        {
            T total = default(T);
            for (int i = 0; i < modifiers.Count; i++)
            {
                total = Add(total, modifiers[i].Value);
            }
            return total;
        }

        public T BaseValue
        {
            get
            {
                return mBaseValue;
            }
            set
            {
                if (!EqualityComparer<T>.Default.Equals(mBaseValue, value))
                {
                    mBaseValue = value;
                    IsDirty = true;
                }
            }
        }

        public ValueModifier<T> AddModifier(T startingValue)
        {
            ValueModifier<T> mod = new ValueModifier<T>(this, startingValue);
            mModifiers.Add(mod);
            IsDirty = true;
            InvokeChanged();
            return mod;
        }

        public void RemoveModifier(ValueModifier<T> modifier)
        {
            mModifiers.Remove(modifier);
            IsDirty = true;
            InvokeChanged();
        }

    }
}
