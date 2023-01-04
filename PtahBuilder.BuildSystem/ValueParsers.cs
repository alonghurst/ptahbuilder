namespace PtahBuilder.BuildSystem;

public static class ValueParsers
{
    internal static List<KeyValuePair<Type, Func<object, object>>> UserDefinedValueParsers { get; } = new();

    public static void AddUserDefinedValueParser(Type type, Func<object, object> parser)
    {
        UserDefinedValueParsers.Add(new KeyValuePair<Type, Func<object, object>>(type, parser));
    }
}