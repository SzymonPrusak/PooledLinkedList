using System.Collections;
using SimEi.Collections;

namespace SimEi.PooledLinkedList.Tests
{
    [TestClass]
    public class ListTests
    {
        [TestMethod]
        public void ShouldInitEmptyList()
        {
            var list = new PooledLinkedList<int>();
            Assert.AreEqual(0, list.Count);
            Assert.IsFalse(list.First.HasValue);
            Assert.IsFalse(list.Last.HasValue);
        }

        [TestMethod]
        public void ShouldHandleInsertion()
        {
            var list = new PooledLinkedList<int>();
            var item1 = list.AddFirst(1);
            var item2 = list.AddFirst(2);

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(item2, list.First.Value);
            Assert.AreEqual(item1, list.Last.Value);
            Assert.IsFalse(item2.Prev.HasValue);
            Assert.AreEqual(item1, item2.Next);
            Assert.AreEqual(item2, item1.Prev);
            Assert.IsFalse(item1.Next.HasValue);

            var item3 = item2.AddBefore(3);
            var item4 = item2.AddAfter(4);
            var item5 = list.AddLast(5);

            Assert.AreEqual(5, list.Count);
            Assert.AreEqual(item3, list.First.Value);
            Assert.AreEqual(item5, list.Last.Value);
            Assert.AreEqual(item4, item2.Next);
            Assert.AreEqual(item4, item1.Prev);
        }

        [TestMethod]
        public void ShouldHandleDeletion()
        {
            var list = new PooledLinkedList<int>();
            var item1 = list.AddFirst(1);
            var item2 = list.AddFirst(2);
            var item3 = list.AddFirst(3);
            var item4 = list.AddFirst(4);
            var item5 = list.AddFirst(5);

            list.Remove(item1);

            Assert.IsFalse(item2.Next.HasValue);
            Assert.AreEqual(item2, list.Last.Value);

            list.Remove(item5);

            Assert.IsFalse(item4.Prev.HasValue);
            Assert.AreEqual(item4, list.First.Value);

            list.Remove(item3);

            Assert.AreEqual(item2, item4.Next);
            Assert.AreEqual(item4, item2.Prev);
        }

        [TestMethod]
        public void ShouldHandleSettingValues()
        {
            var list = new PooledLinkedList<int>();
            list.AddFirst(5).Value = 10;

            Assert.AreEqual(10, list.First.Value.Value);
        }

        [TestMethod]
        public void ShouldEnumerateItems()
        {
            var list = new PooledLinkedList<int>();
            list.AddFirst(3);
            list.AddFirst(4);
            list.AddFirst(5);
            list.AddFirst(6);
            list.AddFirst(7);
            list.AddLast(2);
            list.AddLast(1);

            CollectionAssert.AreEqual(new[] { 7, 6, 5, 4, 3, 2, 1 }, list);
        }

        [TestMethod]
        public void ShouldThrowOnAccessingRemovedItem()
        {
            var list = new PooledLinkedList<int>();
            var item = list.AddFirst(3);
            list.Remove(item);

            Assert.ThrowsException<InvalidOperationException>(() => item.Value);
            Assert.ThrowsException<InvalidOperationException>(() => item.Next);
            Assert.ThrowsException<InvalidOperationException>(() => item.Prev);
            Assert.ThrowsException<InvalidOperationException>(() => item.AddBefore(5));
            Assert.ThrowsException<InvalidOperationException>(() => item.AddAfter(10));
        }

        [TestMethod]
        public void ShouldThrowWhenTryingToResumeEnumeratorFromRemovedItem()
        {
            var list = new PooledLinkedList<int>();
            var item1 = list.AddLast(1);
            var item2 = list.AddLast(2);
            var item3 = list.AddLast(3);

            var enumerator = list.GetEnumerator();
            enumerator.MoveNext();
            list.Remove(item3);
            enumerator.MoveNext();
            list.Remove(item2);

            Assert.ThrowsException<InvalidOperationException>(() => enumerator.MoveNext());
        }

        [TestMethod]
        public void ShouldClear()
        {
            var list = new PooledLinkedList<int>();
            list.AddLast(1);
            list.AddLast(2);
            list.AddLast(3);
            list.Clear();

            Assert.AreEqual(0, list.Count);
            Assert.IsFalse(list.First.HasValue);
            Assert.IsFalse(list.Last.HasValue);

            var item5 = list.AddLast(5);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(item5, list.First.Value);
            Assert.AreEqual(item5, list.Last.Value);
        }

        [TestMethod]
        public void ShouldCopyToArray()
        {
            var list = new PooledLinkedList<int>();
            list.AddLast(1);
            list.AddLast(2);
            list.AddLast(3);

            var array = new int[4];
            list.CopyTo(array, 1);

            CollectionAssert.AreEqual(new[] { 0, 1, 2, 3 }, array);
        }

        [TestMethod]
        public void ShouldRemoveByValue()
        {
            var list = new PooledLinkedList<int>();
            list.AddLast(1);
            ((ICollection<int>)list).Remove(1);

            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public void ShouldContain()
        {
            var list = new PooledLinkedList<int>();
            list.AddLast(1);
            bool contains1 = ((ICollection<int>)list).Contains(1);
            bool contains5 = ((ICollection<int>)list).Contains(5);

            Assert.IsTrue(contains1);
            Assert.IsFalse(contains5);
        }

        [TestMethod]
        public void ShouldCopyToArrayThroughCollectionInterface()
        {
            var list = new PooledLinkedList<int>();
            list.AddLast(1);
            list.AddLast(2);
            list.AddLast(3);

            var array = new int[4];
            ((ICollection)list).CopyTo(array, 1);

            CollectionAssert.AreEqual(new[] { 0, 1, 2, 3 }, array);
        }
    }
}