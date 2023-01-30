namespace PtahBuilder.Tests.TestBuilder.Entities.Conditions;

public readonly struct AnyInSetCondition : ICondition<string>
{
    public AnyInSetCondition(IEnumerable<string> values, bool isExclusionary = false)
    {
        IsExclusionary = isExclusionary;
        Values = values.Select(v => v.Trim()).ToArray();
    }

    public bool IsExclusionary { get; }

    public string[] Values { get; }

    public bool IsMet(string value)
    {
        return Values.Any(x => x.Equals(value, StringComparison.OrdinalIgnoreCase));
    }

    public override string ToString() => $"{ConditionParser.WriteExclusionaryToken(IsExclusionary)}({string.Join(", ", Values)})";
}

public readonly struct AllFromSetCondition : ICondition<IReadOnlyCollection<string>>
{
    public bool IsExclusionary { get; }

    public AllFromSetCondition(IEnumerable<string> values, bool isExclusionary = false)
    {
        IsExclusionary = isExclusionary;
        Values = values.Select(v => v.Trim()).ToArray();
    }

    public string[] Values { get; }

    public bool IsMet(IReadOnlyCollection<string> value)
    {
        foreach (var s in Values)
        {
            if (!value.Contains(s))
            {
                return false;
            }
        }

        return true;
    }

    public override string ToString() => $"{ConditionParser.WriteExclusionaryToken(IsExclusionary)}{ConditionParser.MatchAllToken} ({string.Join(", ", Values)})";
}