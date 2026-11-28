using System.Collections.Generic;
using Lunaria;

public static class TextExtension
{
    public static void SetTexts(this IEnumerable<Text> texts, string content)
    {
        foreach (var text in texts)
        {
            text.SetText(content);
        }   
    }
}
