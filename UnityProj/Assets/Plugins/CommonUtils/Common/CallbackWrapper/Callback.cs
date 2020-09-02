using System;

namespace Nullspace
{
    public static class CallbackUtils
    {
        public static void Release(Callback callback)
        {
            ObjectPools.Instance.Release(callback);
        }

        public static Callback Acquire(Action action)
        {
            Callback0 callback = ObjectPools.Instance.Acquire<Callback0>();
            callback.Handler = action;
            return callback;
        }
        public static Callback Acquire<T>(Action<T> action, T arg1)
        {
            Callback1<T> callback = ObjectPools.Instance.Acquire<Callback1<T>>();
            callback.Handler = action;
            callback.Arg1 = arg1;
            return callback;
        }

        public static Callback Acquire<U, V>(Action<U, V> action, U arg1, V arg2)
        {
            Callback2<U, V> callback = ObjectPools.Instance.Acquire<Callback2<U, V>>();
            callback.Handler = action;
            callback.Arg1 = arg1;
            callback.Arg2 = arg2;
            return callback;
        }
        public static Callback Acquire<U, V, W>(Action<U, V, W> action, U arg1, V arg2, W arg3)
        {
            Callback3<U, V, W> callback = ObjectPools.Instance.Acquire<Callback3<U, V, W>>();
            callback.Handler = action;
            callback.Arg1 = arg1;
            callback.Arg2 = arg2;
            callback.Arg3 = arg3;
            return callback;
        }
        public static Callback Acquire<U, V, W, T>(Action<U, V, W, T> action, U arg1, V arg2, W arg3, T arg4)
        {
            Callback4<U, V, W, T> callback = ObjectPools.Instance.Acquire<Callback4<U, V, W, T>>();
            callback.Handler = action;
            callback.Arg1 = arg1;
            callback.Arg2 = arg2;
            callback.Arg3 = arg3;
            callback.Arg4 = arg4;
            return callback;
        }
    }

    public abstract class Callback : ObjectBase
    {
        public abstract Delegate Handler
        {
            get;
            set;
        }
        protected override void Acquire()
        {

        }
        protected override void Release()
        {

        }
        public override void Destroy()
        {

        }

        public abstract void Run();
    }

}
