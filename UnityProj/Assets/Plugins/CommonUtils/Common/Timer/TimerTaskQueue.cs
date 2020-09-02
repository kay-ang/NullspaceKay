using System;
using System.Diagnostics;

namespace Nullspace
{
    /// <summary>
    /// interval == 0, 表示非 loop
    /// interval > 0, 表示 以 interval 循环
    /// </summary>
    public class TimerTaskQueue
    {
        public static TimerTaskQueue Instance = new TimerTaskQueue();

        private int mNextTimerId;
        private int mCurrentTick;
        private PriorityQueue<int, TimerTask, int> mPriorityQueue;
        private Stopwatch mStopWatch;
        private readonly object mQueueLock = new object();

        public TimerTaskQueue()
        {
            mPriorityQueue = new PriorityQueue<int, TimerTask, int>();
            mStopWatch = new Stopwatch();
            mCurrentTick = 0;
            mNextTimerId = 0;
        }

        public int AddTimer(int start, int interval, Action handler)
        {
            Callback0 callback = ObjectPools.Instance.Acquire<Callback0>();
            callback.Handler = handler;
            var p = GetTimerData(callback, start, interval);
            return AddTimer(p);
        }

        public int AddTimer<T>(int start, int interval, Action<T> handler, T arg1)
        {
            Callback1<T> callback = ObjectPools.Instance.Acquire<Callback1<T>>();
            callback.Arg1 = arg1;
            callback.Handler = handler;
            TimerTask p = GetTimerData(callback, start, interval);
            return AddTimer(p);
        }

        public int AddTimer<T, U>(int start, int interval, Action<T, U> handler, T arg1, U arg2)
        {
            Callback2<T, U> callback = ObjectPools.Instance.Acquire<Callback2<T, U>>();
            callback.Arg1 = arg1;
            callback.Arg2 = arg2;
            callback.Handler = handler;
            TimerTask p = GetTimerData(callback, start, interval);
            return AddTimer(p);
        }

        public int AddTimer<T, U, V>(int start, int interval, Action<T, U, V> handler, T arg1, U arg2, V arg3)
        {
            Callback3<T, U, V> callback = ObjectPools.Instance.Acquire<Callback3<T, U, V>>();
            callback.Arg1 = arg1;
            callback.Arg2 = arg2;
            callback.Arg3 = arg3;
            callback.Handler = handler;
            TimerTask p = GetTimerData(callback, start, interval);
            return AddTimer(p);
        }
        public int AddTimer<T, U, V, W>(int start, int interval, Action<T, U, V> handler, T arg1, U arg2, V arg3, W arg4)
        {
            Callback4<T, U, V, W> callback = ObjectPools.Instance.Acquire<Callback4<T, U, V, W>>();
            callback.Arg1 = arg1;
            callback.Arg2 = arg2;
            callback.Arg3 = arg3;
            callback.Arg4 = arg4;
            callback.Handler = handler;
            TimerTask p = GetTimerData(callback, start, interval);
            return AddTimer(p);
        }
        public void DelTimer(int timerId)
        {
            TimerTask p = null;
            lock (mQueueLock)
            {
                p = mPriorityQueue.Remove(timerId);
            }
            if (p != null)
            {
                ObjectPools.Instance.Release(p);
            }
        }

        private void Update()
        {
            Tick();
        }

        public void Tick()
        {
            mCurrentTick += (int)mStopWatch.ElapsedMilliseconds;
            mStopWatch.Reset();
            mStopWatch.Start();
            while (mPriorityQueue.Size != 0)
            {
                TimerTask p;
                lock (mQueueLock)
                {
                    p = mPriorityQueue.Peek();
                }
                if (mCurrentTick < p.NextTick)
                {
                    break;
                }
                lock (mQueueLock)
                {
                    mPriorityQueue.Dequeue();
                }
                if (p.Interval > 0)
                {
                    p.NextTick += p.Interval;
                    lock (mQueueLock)
                    {
                        mPriorityQueue.Enqueue(p.TimerId, p, p.NextTick);
                    }
                    p.DoAction();
                }
                else
                {
                    p.DoAction();
                    ObjectPools.Instance.Release(p);
                }
            }
        }

        public void Reset()
        {
            mCurrentTick = 0;
            mNextTimerId = 0;
            mStopWatch.Stop();
            lock (mQueueLock)
            {
                while (mPriorityQueue.Size != 0)
                {
                    TimerTask p = mPriorityQueue.Dequeue();
                    ObjectPools.Instance.Release(p);
                }
            }
        }

        private int AddTimer(TimerTask p)
        {
            lock (mQueueLock)
            {
                mPriorityQueue.Enqueue(p.TimerId, p, p.NextTick);
            }
            return p.TimerId;
        }

        private TimerTask GetTimerData(Callback callback, int start, int interval)
        {
            TimerTask task = ObjectPools.Instance.Acquire<TimerTask>();
            task.Callback = callback;
            task.Interval = interval;
            task.TimerId = NextTimerId;
            task.NextTick = mCurrentTick + 1 + start;
            return task;
        }

        public int NextTimerId { get { return ++mNextTimerId; } }

    }

}

