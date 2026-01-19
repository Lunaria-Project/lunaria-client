using System;
using System.Collections.Generic;
using JetBrains.Annotations;

public static class CollectionExtension
{
    public static bool IsNullOrEmpty<T>(this IList<T> list)
    {
        return list == null || list.Count == 0;
    }

    public static T GetAt<T>(this IList<T> list, int index, T defaultValue = default)
    {
        if (list == null || index < 0 || index >= list.Count) return defaultValue;
        return list[index];
    }

    public static T GetAtWithError<T>(this IList<T> list, int index, T defaultValue = default)
    {
        if (list == null || index < 0 || index >= list.Count)
        {
            LogManager.LogErrorFormat("invalid index in IList", list, index);
            return defaultValue;
        }

        return list[index];
    }
    
    public static T GetLast<T>(this IList<T> list, T defaultValue = default)
    {
        if (list == null || list.Count <= 0) return defaultValue;
        return list[^1];
    }

    #region List

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

    #endregion
}