namespace PtahBuilder.BuildSystem.Config.Internal
{
    public class CustomValueParserConfig
    {
        public CustomValueParserConfig(Dictionary<Type, Func<object, object>> customValueParsers)
        {
            CustomValueParsers = customValueParsers;
        }

        public Dictionary<Type, Func<object, object>> CustomValueParsers { get; }
    }
}
