namespace PtahBuilder.Tests.TestBuilder.Entities.Conditions;

public static class ConditionParser
{
    public const string MatchAllToken = "all";
    public const string ExclusionaryToken = "!";

    public static string WriteExclusionaryToken(bool isExclusionary) => isExclusionary ? ExclusionaryToken : string.Empty;

    public static ICondition Parse(string value)
    {
        value = value.Trim();
        bool wasBracketed = false;
        bool wasAll = false;

        var val = 0f;
        var isFloat = false;
        var isExclusionary = false;

        if (value.StartsWith(ExclusionaryToken))
        {
            isExclusionary = true;
            value = value.Substring(ExclusionaryToken.Length);
        }

        if (value.StartsWith((string)MatchAllToken))
        {
            wasAll = true;
            value = value.Substring(MatchAllToken.Length).Trim();
        }

        if (value.StartsWith("(") && value.EndsWith(")"))
        {
            wasBracketed = true;
            value = value.Substring(1, value.Length - 2).Trim();
        }

        var splits = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (splits.Length == 2)
        {
            isFloat = float.TryParse(splits[1], out val);

            if (splits[0].StartsWith(">"))
            {
                if (!isFloat)
                {
                    throw new InvalidOperationException($"Unable to parse {splits[1]} as a float");
                }

                return new GreaterThanCondition(val, splits[0].Contains("="));
            }
            else if (splits[0].StartsWith("<"))
            {
                if (!isFloat)
                {
                    throw new InvalidOperationException($"Unable to parse {splits[1]} as a float");
                }

                return new LessThanCondition(val, splits[0].Contains("="));
            }
            else if (splits[0].Trim() == "=")
            {
                return isFloat ? new ExactCondition<float>(val, isExclusionary) : new ExactCondition<string>(splits[1], isExclusionary);
            }
            else if (wasBracketed)
            {
                if (float.TryParse(splits[0], out var min) && float.TryParse(splits[1], out var max))
                {
                    return new InRangeCondition(min, max, isExclusionary);
                }
            }
        }

        if (wasBracketed)
        {
            var parts = value.Split(',', StringSplitOptions.RemoveEmptyEntries);

            return wasAll ? new AllFromSetCondition(parts, isExclusionary) : new AnyInSetCondition(parts, isExclusionary);
        }

        if (value.StartsWith("="))
        {
            value = value.Substring(1);
        }

        isFloat = float.TryParse(value, out val);
        return isFloat ? new ExactCondition<float>(val, isExclusionary) : new ExactCondition<string>(value, isExclusionary);
    }
}