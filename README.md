# PooledLinkedList

Implementation of a collection similar to the standard `System.Collections.Generic.LinkedList<T>`, but without heap allocation on every item insertion by using struct pool under the hood.

## Motivation
When working with Unity, sometimes I had to use the standard `LinkedList<T>`, but it came with a big cost - as the Unity's garbage collector is a technology way behind current GC for CoreCLR (as for August 2023 - this is going to change soon because Unity is working on moving to CoreCLR), you have to be careful with every heap allocation. This way, I came up with an idea of implementing `LinkedList<T>` with a node pool, being an array of structs, so allocations are needed only during expanding the pool.

## Target Frameworks
- netstandard2.1

## Getting started
Install through NuGet package manager.
```
dotnet add package SimEi.PooledLinkedList
```

Tests in `SimEi.PooledLinkedList.Tests` package use MSTest.

## Usage
API is similar to the standard `LinkedList<T>`.

### Creating list
```cs
using SimEi.Collections;
var list1 = new PooledLinkedList<int>();
var list2 = new PooledLinkedList<int>(capacity: 5); // Initial pool capacity = 5.
```

### Item manipulation
```cs
list.AddFirst(1);
var last = list.AddLast(3);
var newLast = list.AddAfter(last, 4);
var second = list.AddBefore(last, 2);
// list contains { 1, 2, 3, 4 }.

list.Remove(last);
// list contains { 1, 2, 4 }.

newLast.Value = 100;
// list contains { 1, 2, 100 }.

list.Clear()
// list is empty.
```

### Item manipulation through `ItemHandle`
You can also add items relative to the current node handle.
```cs
list.AddFirst(5).AddAfter(10).AddAfter(20).AddBefore(15);
// list contains { 5, 10, 15, 20 }.
```

## License
This project is licensed under MIT license.
