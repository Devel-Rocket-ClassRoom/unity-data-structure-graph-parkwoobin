using UnityEngine;
using System.Collections.Generic;

public class PriorityQueue<TElement, TPriority>
{
    private List<(TElement Element, TPriority Priority)> heap;
    private readonly IComparer<TPriority> comparer;
    public int Count => heap.Count;

    public PriorityQueue()
    {
        heap = new List<(TElement, TPriority)>();
        comparer = Comparer<TPriority>.Default;
    }
    public PriorityQueue(IComparer<TPriority> customComparer)
    {
        heap = new List<(TElement, TPriority)>();
        comparer = customComparer ?? Comparer<TPriority>.Default;
    }


    public void Enqueue(TElement element, TPriority priority)
    {
        heap.Add((element, priority));
        HeapfyUp(heap.Count - 1);

    }

    public TElement Dequeue()
    {
        if (heap.Count == 0)
        {
            throw new System.InvalidOperationException("큐가 비어 있습니다.");
        }
        TElement result = heap[0].Element; // 나갈 인덱스

        int lastIndex = heap.Count - 1;
        heap[0] = heap[lastIndex]; // 마지막 요소를 루트로 이동
        heap.RemoveAt(lastIndex); // 마지막 요소 제거

        if (heap.Count > 0)
        {
            HeapfyDown(0); // 루트에서 힙 속성 복원
        }

        return result;
    }

    public TElement Peek()
    {
        if (heap.Count == 0)
        {
            throw new System.InvalidOperationException("큐가 비어 있습니다.");
        }
        return heap[0].Element;
    }

    public void Clear()
    {
        heap.Clear();
    }

    public List<(TElement Element, TPriority Priority)> GetQueue()  // 디버그용 큐 상태 반환
    {
        return heap;
    }

    public void HeapfyUp(int index)
    {
        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;  // 부모 인덱스 계산

            if (comparer.Compare(heap[index].Priority, heap[parentIndex].Priority) >= 0)
            {
                break;
            }

            // Swap
            Swap(index, parentIndex);

            index = parentIndex;

        }
    }
    public void HeapfyDown(int index)
    {
        int lastIndex = heap.Count - 1;

        while (true)
        {
            // 좌 우 자식 인덱스 계산
            int leftChildIndex = 2 * index + 1;
            int rightChildIndex = 2 * index + 2;
            int smallestIndex = index;

            if (leftChildIndex <= lastIndex && comparer.Compare(heap[leftChildIndex].Priority, heap[smallestIndex].Priority) < 0)
            {
                smallestIndex = leftChildIndex;
            }

            if (rightChildIndex <= lastIndex && comparer.Compare(heap[rightChildIndex].Priority, heap[smallestIndex].Priority) < 0)
            {
                smallestIndex = rightChildIndex;
            }

            if (smallestIndex == index)
            {
                break;
            }

            // Swap
            Swap(index, smallestIndex);

            index = smallestIndex;
        }
    }

    private void Swap(int index, int parentIndex)
    {
        var temp = heap[index];
        heap[index] = heap[parentIndex];
        heap[parentIndex] = temp;
    }
}
