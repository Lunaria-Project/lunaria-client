using System.Collections.Generic;
using UnityEngine;

public static class StringExtension
{
    public static List<int> ParseIntList(this string raw)
    {
        if (string.IsNullOrEmpty(raw)) return new List<int>();

        var parts = raw.Split(',');
        var list = new List<int>();

        foreach (var part in parts)
        {
            if (int.TryParse(part.Trim(), out var num))
            {
                list.Add(num);
            }
        }
        return list;
    }

    public static List<string> ParseStringList(this string raw)
    {
        if (string.IsNullOrEmpty(raw)) return new List<string>();

        var list = new List<string>();
        var parts = raw.Split(',');
        list.AddRange(parts);
        return list;
    }

    public static Vector2 ParseVector2(this string raw)
    {
        var parts = raw.Split(',');
        if (float.TryParse(parts[0], out var x) && float.TryParse(parts[1], out var y)) return new Vector2(x, y);
        return Vector2.zero;
    }

    public static List<float> ParseFloatList(this string raw)
    {
        if (string.IsNullOrEmpty(raw)) return new List<float>();

        var parts = raw.Split(',');
        var list = new List<float>();

        foreach (var part in parts)
        {
            if (float.TryParse(part.Trim(), out var num))
            {
                list.Add(num);
            }
        }
        return list;
    }

    public static string ToNDigits(this int value, int digits)
    {
        return value.ToString($"D{digits}");
    }

    public static T ParseEnum<T>(this string value) where T : struct, System.Enum
    {
        return string.IsNullOrEmpty(value) ? default : (T)System.Enum.Parse(typeof(T), value, true);
    }

    public static bool ParseBool(this string value)
    {
        if (string.IsNullOrEmpty(value)) return false;
        if (int.TryParse(value, out var intVal)) return intVal != 0;
        if (bool.TryParse(value, out var boolVal)) return boolVal;
        return false;
    }
}