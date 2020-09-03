
using System.Collections.Generic;

namespace Nullspace
{
    public class DoubleCircleLinkedList<T>
    {
        private LinkedList<T> mDataList;
        private LinkedListNode<T> mCurrent;

        public DoubleCircleLinkedList()
        {
            mDataList = new LinkedList<T>();
        }

        public int Count
        {
            get
            {
                return mDataList.Count;
            }
        }

        public void RemoveLast()
        {
            mDataList.RemoveLast();
        }

        public void RemoveFirst()
        {
            mDataList.RemoveFirst();
        }

        public void Remove(LinkedListNode<T> node)
        {
            mDataList.Remove(node);
        }

        public void AddLast(T node)
        {
            mDataList.AddLast(node);
        }

        public void AddAfter(LinkedListNode<T> node, LinkedListNode<T> value)
        {
            mDataList.AddAfter(node, value);
        }

        public void AddAfter(LinkedListNode<T> node, T value)
        {
            mDataList.AddAfter(node, value);
        }

        public void AddBefore(LinkedListNode<T> node, LinkedListNode<T> value)
        {
            mDataList.AddBefore(node, value);
        }

        public void AddBefore(LinkedListNode<T> node, T value)
        {
            mDataList.AddBefore(node, value);
        }

        public void AddFirst(T node)
        {
            mDataList.AddFirst(node);
        }

        public T ResetCurrentFirst()
        {
            mCurrent = First;
            return mCurrent.Value;
        }

        public T ResetCurrentLast()
        {
            mCurrent = Last;
            return mCurrent.Value;
        }

        public T Next()
        {
            mCurrent = Next(mCurrent);
            return mCurrent.Value;
        }

        public T Previous()
        {
            mCurrent = Previous(mCurrent);
            return mCurrent.Value;
        }

        public LinkedListNode<T> Next(LinkedListNode<T> node)
        {
            if (node.Next == null)
                return mDataList.First;
            return node.Next;
        }

        public LinkedListNode<T> Previous(LinkedListNode<T> node)
        {
            if (node.Previous == null)
                return mDataList.Last;
            return node.Previous;
        }

        public LinkedListNode<T> Last
        {
            get
            {
                return mDataList.Last;
            }
        }

        public LinkedListNode<T> First
        {
            get
            {
                return mDataList.First;
            }
        }


    }
}
