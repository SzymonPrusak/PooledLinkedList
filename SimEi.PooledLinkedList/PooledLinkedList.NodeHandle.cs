using System;

namespace SimEi.Collections
{
    partial class PooledLinkedList<T>
    {
        public readonly struct NodeHandle : IEquatable<NodeHandle>
        {
            internal readonly int Index;
            internal readonly int Generation;
            private readonly PooledLinkedList<T> _owner;

            internal NodeHandle(PooledLinkedList<T> owner, int index, int generation)
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

            public NodeHandle? Next => _owner.GetNext(this);
            public NodeHandle? Prev => _owner.GetPrev(this);


            public NodeHandle AddBefore(T value) => _owner.AddBefore(this, value);
            public NodeHandle AddAfter(T value) => _owner.AddAfter(this, value);

            public override bool Equals(object obj) => obj is NodeHandle i && Equals(i);
            public override int GetHashCode() => (Index, Generation, _owner).GetHashCode();

            public bool Equals(NodeHandle i) => Index == i.Index && _owner == i._owner && Generation == i.Generation;
        }
    }
}
