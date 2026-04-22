public static class EnumSwitch
{
    public static T Exhaustive<T>(T value) where T : struct, System.Enum
    {
        return value;
    }
}
