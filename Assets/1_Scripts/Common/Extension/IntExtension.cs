public static class IntExtension
{ 
    public static string ToCommaString(this int value)
    {
        return value.ToString("N0");
    }
    
    public static string ToPrice(this int value)
    {
        return value.ToString("N0");
    }
}
