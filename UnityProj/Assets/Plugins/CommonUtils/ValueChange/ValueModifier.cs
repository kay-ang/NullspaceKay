
using System.Collections.Generic;

namespace Nullspace
{
    public class ValueModifier<T>
    {
        public ValueStackGeneric<T> Parent;
        public T CurrentValue;

        public T Value
        {
            get
            {
                return CurrentValue;
            }
            set
            {
                if (!EqualityComparer<T>.Default.Equals(CurrentValue, value))
                {
                    CurrentValue = value;
                    Parent.IsDirty = true;
                    Parent.InvokeChanged();
                }
            }
        }

        public ValueModifier(ValueStackGeneric<T> parent)
        {
            Parent = parent;
            Value = default(T);
        }

        public ValueModifier(ValueStackGeneric<T> parent, T value)
        {
            Parent = parent;
            Value = value;
        }
    }
}
