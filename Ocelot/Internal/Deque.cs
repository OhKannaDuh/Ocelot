using System;
using System.Collections;
using System.Collections.Generic;

namespace Ocelot.Internal;

public sealed class Deque<T> : IEnumerable<T>
{
    private T[] buffer;

    private int head;

    private int tail;

    private int count;

    public Deque(int capacity = 8)
    {
        if (capacity < 1)
        {
            capacity = 8;
        }

        buffer = new T[capacity];
    }

    public int Count {
        get => count;
    }

    public bool IsEmpty {
        get => count == 0;
    }

    public void AddToFront(T item)
    {
        EnsureCapacityForOneMore();
        head = (head - 1 + buffer.Length) % buffer.Length;
        buffer[head] = item;
        count++;
    }

    public void AddToBack(T item)
    {
        EnsureCapacityForOneMore();
        buffer[tail] = item;
        tail = (tail + 1) % buffer.Length;
        count++;
    }

    public T RemoveFromFront()
    {
        if (count == 0)
        {
            throw new InvalidOperationException("Deque is empty.");
        }

        var item = buffer[head];
        buffer[head] = default!;
        head = (head + 1) % buffer.Length;
        count--;
        return item;
    }

    public T RemoveFromBack()
    {
        if (count == 0)
        {
            throw new InvalidOperationException("Deque is empty.");
        }

        tail = (tail - 1 + buffer.Length) % buffer.Length;
        var item = buffer[tail];
        buffer[tail] = default!;
        count--;
        return item;
    }

    public T PeekFront()
    {
        if (count == 0)
        {
            throw new InvalidOperationException("Deque is empty.");
        }

        return buffer[head];
    }

    public T PeekBack()
    {
        if (count == 0)
        {
            throw new InvalidOperationException("Deque is empty.");
        }

        var idx = (tail - 1 + buffer.Length) % buffer.Length;
        return buffer[idx];
    }

    public void Clear()
    {
        Array.Clear(buffer, 0, buffer.Length);
        head = tail = count = 0;
    }

    private void EnsureCapacityForOneMore()
    {
        if (count < buffer.Length) return;

        var newBuf = new T[buffer.Length * 2];
        if (head < tail)
        {
            Array.Copy(buffer, head, newBuf, 0, count);
        }
        else
        {
            var frontLen = buffer.Length - head;
            Array.Copy(buffer, head, newBuf, 0, frontLen);
            Array.Copy(buffer, 0, newBuf, frontLen, tail);
        }

        buffer = newBuf;
        head = 0;
        tail = count;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (var i = 0; i < count; i++)
        {
            var idx = (head + i) % buffer.Length;
            yield return buffer[idx];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
