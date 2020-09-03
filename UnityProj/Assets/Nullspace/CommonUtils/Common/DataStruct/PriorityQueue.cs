using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace Nullspace
{
    public sealed class PriorityQueueEventArgs<T> : EventArgs // where T : class
    {
        public PriorityQueueEventArgs(T oldFirstElement, T newFirstElement)
        {
            OldFirstElement = oldFirstElement;
            NewFirstElement = newFirstElement;
        }
        public T NewFirstElement { get; set; }
        public T OldFirstElement { get; set; }
    }

    [Serializable]
    public class PriorityQueue<K, V, P> // where V : class
    {
        private List<HeapNode<K, V, P>> mPriorityHeap;
        private HeapNode<K, V, P> mPlaceHolder;
        private Comparer<P> mPriorityComparer;
        public int Size;
        // public event EventHandler<KeyedPriorityQueueEventArgs<V>> mFirstElementChanged;
        public PriorityQueue()
        {
            mPriorityHeap = new List<HeapNode<K, V, P>>();
            mPriorityComparer = Comparer<P>.Default;
            mPlaceHolder = new HeapNode<K, V, P>();
            mPriorityHeap.Add(new HeapNode<K, V, P>());
        }

        public PriorityQueue(Comparer<P> compair)
        {
            mPriorityHeap = new List<HeapNode<K, V, P>>();
            mPriorityComparer = compair;
            mPlaceHolder = new HeapNode<K, V, P>();
            mPriorityHeap.Add(new HeapNode<K, V, P>());
        }

        public void Clear()
        {
            mPriorityHeap.Clear();
            mPriorityHeap.Add(new HeapNode<K, V, P>());
            Size = 0;
        }

        public V Dequeue()
        {
            V local = (Size < 1) ? default(V) : DequeueImpl();
            // V newHead = (Size < 1) ? default(V) : mPriorityHeap[1].Value;
            // this.RaiseHeadChangedEvent(default(V), newHead);
            return local;
        }

        private V DequeueImpl()
        {
            V local = this.mPriorityHeap[1].Value;
            this.mPriorityHeap[1] = mPriorityHeap[Size];
            this.mPriorityHeap[Size--] = this.mPlaceHolder;
            this.Heapify(1);
            return local;
        }

        public void Enqueue(K key, V value, P priority)
        {
            // V local = (Size > 0) ? this.mPriorityHeap[1].Value : default(V);
            int num = ++Size;
            int num2 = num >> 1;
            if (num == mPriorityHeap.Count)
            {
                mPriorityHeap.Add(mPlaceHolder);
            }
            while ((num > 1) && IsHigher(priority, mPriorityHeap[num2].Priority))
            {
                mPriorityHeap[num] = this.mPriorityHeap[num2];
                num = num2;
                num2 = num >> 1;
            }
            mPriorityHeap[num] = new HeapNode<K, V, P>(key, value, priority);
            // V newHead = mPriorityHeap[1].Value;
            //if (!newHead.Equals(local))
            //{
            //    this.RaiseHeadChangedEvent(local, newHead);
            //}
        }

        public V FindByPriority(P priority, Predicate<V> match)
        {
            if (Size >= 1)
            {
                return Search(priority, 1, match);
            }
            return default(V);
        }

        private void Heapify(int i)
        {
            int num = i << 1;
            int num2 = num + 1;
            int j = i;
            if ((num <= Size) && IsHigher(mPriorityHeap[num].Priority, mPriorityHeap[i].Priority))
            {
                j = num;
            }
            if ((num2 <= Size) && IsHigher(mPriorityHeap[num2].Priority, mPriorityHeap[j].Priority))
            {
                j = num2;
            }
            if (j != i)
            {
                Swap(i, j);
                Heapify(j);
            }
        }

        protected virtual bool IsHigher(P p1, P p2)
        {
            return (mPriorityComparer.Compare(p1, p2) < 1);
        }

        public V Peek()
        {
            if (Size >= 1)
            {
                return mPriorityHeap[1].Value;
            }
            return default(V);
        }

        public P PeekPriority()
        {
            if (Size >= 1)
            {
                return mPriorityHeap[1].Priority;
            }
            return default(P);
        }

        private void RaiseHeadChangedEvent(V oldHead, V newHead)
        {
            //if (!oldHead.Equals(newHead))
            //{
            //    EventHandler<KeyedPriorityQueueEventArgs<V>> firstElementChanged = this.mFirstElementChanged;
            //    if (firstElementChanged != null)
            //    {
            //        firstElementChanged(this, new KeyedPriorityQueueEventArgs<V>(oldHead, newHead));
            //    }
            //}
        }

        public bool Contain(K key)
        {
            if (Size >= 1)
            {
                for (int i = 1; i <= Size; i++)
                {
                    if (mPriorityHeap[i].Key.Equals(key))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public V Remove(K key)
        {
            if (Size >= 1)
            {
                // V oldHead = mPriorityHeap[1].Value;
                for (int i = 1; i <= Size; i++)
                {
                    if (mPriorityHeap[i].Key.Equals(key))
                    {
                        V local2 = mPriorityHeap[i].Value;
                        Swap(i, Size);
                        mPriorityHeap[Size--] = mPlaceHolder;
                        Heapify(i);
                        //V local3 = this.mPriorityHeap[1].Value;
                        //if (!oldHead.Equals(local3))
                        //{
                        //    this.RaiseHeadChangedEvent(oldHead, local3);
                        //}
                        return local2;
                    }
                }
            }
            return default(V);
        }

        private V Search(P priority, int i, Predicate<V> match)
        {
            V local = default(V);
            if (this.IsHigher(mPriorityHeap[i].Priority, priority))
            {
                if (match(mPriorityHeap[i].Value))
                {
                    local = mPriorityHeap[i].Value;
                }
                int num = i << 1;
                int num2 = num + 1;
                if ((local == null) && (num <= Size))
                {
                    local = Search(priority, num, match);
                }
                if ((local == null) && (num2 <= Size))
                {
                    local = Search(priority, num2, match);
                }
            }
            return local;
        }

        private void Swap(int i, int j)
        {
            HeapNode<K, V, P> node = mPriorityHeap[i];
            mPriorityHeap[i] = mPriorityHeap[j];
            mPriorityHeap[j] = node;
        }

        public ReadOnlyCollection<K> Keys
        {
            get
            {
                List<K> list = new List<K>();
                for (int i = 1; i <= Size; i++)
                {
                    list.Add(mPriorityHeap[i].Key);
                }
                return new ReadOnlyCollection<K>(list);
            }
        }

        public ReadOnlyCollection<V> Values
        {
            get
            {
                List<V> list = new List<V>();
                for (int i = 1; i <= Size; i++)
                {
                    list.Add(mPriorityHeap[i].Value);
                }
                return new ReadOnlyCollection<V>(list);
            }
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        private struct HeapNode<KK, VV, PP>
        {
            public KK Key;
            public VV Value;
            public PP Priority;
            public HeapNode(KK key, VV value, PP priority)
            {
                Key = key;
                Value = value;
                Priority = priority;
            }
        }

    }
}
