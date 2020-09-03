
namespace Nullspace
{
    public class ShortEventField : EventField<short>
    {
        public ShortEventField() : base()
        {
        }
        public ShortEventField(short defaultValue) : base(defaultValue)
        {
        }
    }

    public class UIntEventField : EventField<uint>
    {
        public UIntEventField() : base()
        {

        }
        public UIntEventField(uint defaultValue) : base(defaultValue)
        {
        }
    }

    public class IntEventField : EventField<int>
    {
        public IntEventField() : base()
        {
        }
        public IntEventField(int defaultValue) : base(defaultValue)
        {
        }
    }

    public class FloatEventField : EventField<float>
    {
        public FloatEventField() : base()
        {
        }
        public FloatEventField(float defaultValue) : base(defaultValue)
        {
        }
    }

    public class BoolEventField : EventField<bool>
    {
        public BoolEventField() : base()
        {
        }
        public BoolEventField(bool defaultValue) : base(defaultValue)
        {
        }
    }
}
