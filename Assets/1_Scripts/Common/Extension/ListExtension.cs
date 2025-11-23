using System;
using System.Collections.Generic;
using JetBrains.Annotations;

public static class ListExtension
{
    public static bool IsNullOrEmpty<T>(this List<T> list)
    {
        return list == null || list.Count == 0;
    }

    public static void EnsureCapacity<TItem>([NotNull] this List<TItem> list, int capacity)
    {
        if (list.Capacity >= capacity) return;
        list.Capacity = capacity;
    }

    public static TItem RemoveLast<TItem>([NotNull] this List<TItem> list)
    {
        return list.RemoveLast(1);
    }

    public static TItem RemoveLast<TItem>([NotNull] this List<TItem> list, int count)
    {
        if (list.Count == 0)
        {
            throw new InvalidOperationException("List is empty.");
        }

        if (count > list.Count)
        {
            count = list.Count;
        }

        var index = list.Count - count;
        var last = list[index];
        list.RemoveRange(index, count);
        return last;
    }
}