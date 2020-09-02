using System;
using System.Collections.Generic;

namespace Nullspace
{
    public abstract class EventField
    {
        public event Action OnChanged;

        public void ResetEvents()
        {
            OnChanged = null;
        }

        public void InvokeChanged()
        {
            OnChanged?.Invoke();
        }
    }

    public class EventField<T> : EventField
    {
        private T mInternalValue;

        public EventField()
        {
        }

        public EventField(T defaultValue)
        {
            mInternalValue = defaultValue;
        }

        public T Value
        {
            get
            {
                return mInternalValue;
            }
            set
            {
                if (!EqualityComparer<T>.Default.Equals(mInternalValue, value))
                {
                    mInternalValue = value;
                    InvokeChanged();
                }
            }
        }
    }

}
