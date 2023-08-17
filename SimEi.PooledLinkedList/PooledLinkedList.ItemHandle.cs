using System;

namespace SimEi.Collections
{
    partial class PooledLinkedList<T>
    {
        public readonly struct ItemHandle : IEquatable<ItemHandle>
        {
            internal readonly int Index;
            internal readonly int Generation;
            private readonly PooledLinkedList<T> _owner;

            internal ItemHandle(PooledLinkedList<T> owner, int index, int generation)
            {
                Index = index;
                Generation = generation;
                _owner = owner;
            }


            public T Value
            {
                get => _owner[this];
                set => _owner[this] = value;
            }

            public ItemHandle? Next => _owner.GetNext(this);
            public ItemHandle? Prev => _owner.GetPrev(this);


            public ItemHandle AddBefore(T value) => _owner.AddBefore(this, value);
            public ItemHandle AddAfter(T value) => _owner.AddAfter(this, value);

            public override bool Equals(object obj) => obj is ItemHandle i && Equals(i);
            public override int GetHashCode() => (Index, Generation, _owner).GetHashCode();

            public bool Equals(ItemHandle i) => Index == i.Index && _owner == i._owner && Generation == i.Generation;
        }
    }
}
