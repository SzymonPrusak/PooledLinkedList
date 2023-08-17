using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SimEi.Collections
{
    public partial class PooledLinkedList<T> : ICollection<T>, IReadOnlyCollection<T>, ICollection
    {
        private const int DefaultCapacity = 4;
        private const int NoLinkIndex = -1;

        private readonly Queue<int> _freeNodeIndexQueue;

        private Node[] _nodes;
        private int _count;
        private int _firstItemIndex;
        private int _lastItemIndex;

        public PooledLinkedList()
            : this(DefaultCapacity)
        {

        }

        public PooledLinkedList(int capacity)
        {
            _freeNodeIndexQueue = new Queue<int>(capacity);
            for (int i = 0; i < capacity; i++)
                _freeNodeIndexQueue.Enqueue(i);

            _nodes = new Node[capacity];
            _count = 0;
            _firstItemIndex = NoLinkIndex;
            _lastItemIndex = NoLinkIndex;
        }



        public int Count => _count;

        public ItemHandle? First => GetItemOrDefault(_firstItemIndex);
        public ItemHandle? Last => GetItemOrDefault(_lastItemIndex);

        public T this[ItemHandle item]
        {
            get
            {
                Validate(item);
                return _nodes[item.Index].Value;
            }
            set
            {
                Validate(item);
                _nodes[item.Index].Value = value;
            }
        }

        bool ICollection<T>.IsReadOnly => false;

        bool ICollection.IsSynchronized => true;
        object ICollection.SyncRoot => this;



        public ItemHandle? GetNext(ItemHandle item)
        {
            Validate(item);
            int nextIdx = _nodes[item.Index].NextIndex;
            return GetItemOrDefault(nextIdx);
        }

        public ItemHandle? GetPrev(ItemHandle item)
        {
            Validate(item);
            int prevIdx = _nodes[item.Index].PrevIndex;
            return GetItemOrDefault(prevIdx);
        }


        public ItemHandle AddFirst(T value)
        {
            return AddBetween(NoLinkIndex, _firstItemIndex, value);
        }

        public ItemHandle AddLast(T value)
        {
            return AddBetween(_lastItemIndex, NoLinkIndex, value);
        }

        public ItemHandle AddBefore(ItemHandle item, T value)
        {
            Validate(item);

            int prevIdx = _nodes[item.Index].PrevIndex;
            int nextIdx = item.Index;
            return AddBetween(prevIdx, nextIdx, value);
        }

        public ItemHandle AddAfter(ItemHandle item, T value)
        {
            Validate(item);

            int prevIdx = item.Index;
            int nextIdx = _nodes[item.Index].NextIndex;
            return AddBetween(prevIdx, nextIdx, value);
        }

        public void Remove(ItemHandle item)
        {
            Validate(item);
            int prevIdx = _nodes[item.Index].PrevIndex;
            int nextIdx = _nodes[item.Index].NextIndex;
            LinkNodes(prevIdx, nextIdx);

            ReleaseNode(item.Index);
            _count--;
        }

        public void Clear()
        {
            _freeNodeIndexQueue.Clear();
            for (int i = 0; i < _nodes.Length; i++)
                ReleaseNode(i);

            _firstItemIndex = NoLinkIndex;
            _lastItemIndex = NoLinkIndex;
            _count = 0;
        }


        public void CopyTo(T[] array, int arrayIndex)
        {
            int i = arrayIndex;
            for (var cur = First; cur.HasValue; cur = GetNext(cur.Value), i++)
                array[i] = _nodes[cur.Value.Index].Value;
        }


        public IEnumerator<T> GetEnumerator() => new Enumerator(this);


        void ICollection<T>.Add(T item) => AddLast(item);

        bool ICollection<T>.Remove(T value)
        {
            var comparer = EqualityComparer<T>.Default;
            for (var cur = First; cur.HasValue; cur = GetNext(cur.Value))
            {
                if (comparer.Equals(_nodes[cur.Value.Index].Value, value))
                {
                    Remove(cur.Value);
                    return true;
                }
            }
            return false;
        }

        bool ICollection<T>.Contains(T item)
        {
            var comparer = EqualityComparer<T>.Default;
            for (var cur = First; cur.HasValue; cur = GetNext(cur.Value))
            {
                if (comparer.Equals(_nodes[cur.Value.Index].Value, item))
                    return true;
            }

            return false;
        }

        void ICollection.CopyTo(Array array, int index)
        {
            int i = index;
            for (var cur = First; cur.HasValue; cur = GetNext(cur.Value), i++)
                array.SetValue(_nodes[cur.Value.Index].Value, i);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Validate(ItemHandle item)
        {
            if (_nodes[item.Index].Generation != item.Generation)
                throw new InvalidOperationException("tried to use handle after it has been removed from the list");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ItemHandle? GetItemOrDefault(int index)
        {
            if (index == NoLinkIndex)
                return null;
            return new ItemHandle(this, index, _nodes[index].Generation);
        }


        private ItemHandle AddBetween(int prevIdx, int nextIdx, T value)
        {
            EnlargeIfFull();

            var idx = _freeNodeIndexQueue.Dequeue();
            _nodes[idx].Value = value;
            _count++;

            LinkNodes(prevIdx, idx);
            LinkNodes(idx, nextIdx);
            return new ItemHandle(this, idx, _nodes[idx].Generation);
        }


        private void LinkNodes(int prev, int next)
        {
            if (prev != NoLinkIndex)
            {
                _nodes[prev].NextIndex = next;
            }
            else
            {
                _firstItemIndex = next;
            }

            if (next != NoLinkIndex)
            {
                _nodes[next].PrevIndex = prev;
            }
            else
            {
                _lastItemIndex = prev;
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReleaseNode(int index)
        {
            _nodes[index].Generation++;
            _nodes[index].Value = default;
            _freeNodeIndexQueue.Enqueue(index);
        }


        private void EnlargeIfFull()
        {
            if (_count == _nodes.Length)
            {
                Enlarge();
            }
        }

        private void Enlarge()
        {
            var oldNodes = _nodes;
            int newLength = oldNodes.Length * 2;
            _nodes = new Node[newLength];
            Array.Copy(oldNodes, _nodes, oldNodes.Length);

            for (int i = oldNodes.Length; i < newLength; i++)
                _freeNodeIndexQueue.Enqueue(i);
        }
    }
}
