
namespace Nullspace
{
    /// <summary>
    /// 通过调用 Append 和 Insert 系列函数
    /// </summary>
    public class SequenceMultipleDynamic : SequenceMultiple
    {
        internal SequenceMultipleDynamic() : base(true)
        {

        }

        public float MaxPercent
        {
            get
            {
                return 1;
            }
        }

        public float MinPercent
        {
            get
            {
                return 1;
            }
        }
    }
}
