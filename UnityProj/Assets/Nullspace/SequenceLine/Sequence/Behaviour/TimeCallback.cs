
namespace Nullspace
{
    public class TimeCallback : BehaviourCallback
    {
        internal TimeCallback() : base(null, null, null)
        {

        }
        internal virtual string Tag { get; set; }
        internal virtual bool CanBeInterruptted() { return false; }
    }

    // 冷却 duration 秒
    public class CooldownCallback : TimeCallback
    {
        internal CooldownCallback(float startTime, float duration) : base()
        {
            SetStartDurationTime(startTime, duration);
        }

        // 不能都被打断
        internal override bool CanBeInterruptted() { return false; }
    }

    // 暂定 duration 秒
    public class PauseCallback : TimeCallback
    {
        internal PauseCallback(float startTime, float duration) : base()
        {
            SetStartDurationTime(startTime, duration);
        }

        internal PauseCallback(string tag, float startTime, float duration) : base()
        {
            SetStartDurationTime(startTime, duration);
            Tag = tag;
        }

        // 能都被打断
        internal override bool CanBeInterruptted() { return true; }
    }
}
