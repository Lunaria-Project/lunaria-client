using Lunaria;

public static class TextExtension
{
    public static void SetTexts(this Text[] texts, string content)
    {
        foreach (var text in texts)
        {
            text.SetText(content);
        }   
    }
}
