
namespace SimEi.Collections
{
    partial class PooledLinkedList<T>
    {
        private struct Node
        {
            public T Value;
            public int PrevIndex;
            public int NextIndex;
            public int Generation;
        }
    }
}
