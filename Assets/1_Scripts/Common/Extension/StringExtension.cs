using System.Collections.Generic;
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
}
