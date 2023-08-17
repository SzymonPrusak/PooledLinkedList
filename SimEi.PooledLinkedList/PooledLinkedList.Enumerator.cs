using System;
using System.Collections;
using System.Collections.Generic;

namespace SimEi.Collections
{
	partial class PooledLinkedList<T>
	{
		private class Enumerator : IEnumerator<T>
		{
			private readonly PooledLinkedList<T> _list;

			private bool _started;
			private int _index;
			private int _generationAtIndex;

			public Enumerator(PooledLinkedList<T> list)
			{
				_list = list;
				_index = NoLinkIndex;
			}


			public T Current
			{
				get
				{
					if (_index == NoLinkIndex)
						throw new InvalidOperationException("no current item");
					return _list._nodes[_index].Value;
				}
			}

			object IEnumerator.Current => Current;


			public void Dispose()
			{

			}

			public bool MoveNext()
			{
				if (!_started)
                {
                    _started = true;
                    _index = _list._firstItemIndex;
					_generationAtIndex = _list._nodes[_index].Generation;
					return _index != NoLinkIndex;
				}

				if (_index == NoLinkIndex)
					return false;

				if (_generationAtIndex != _list._nodes[_index].Generation)
					throw new InvalidOperationException("item at enumerator has been deleted from the collection");

				_index = _list._nodes[_index].NextIndex;
                _generationAtIndex = _list._nodes[_index].Generation;
                return _index != NoLinkIndex;
			}

			public void Reset()
			{
				_started = false;
				_index = NoLinkIndex;
			}
		}
	}
}
