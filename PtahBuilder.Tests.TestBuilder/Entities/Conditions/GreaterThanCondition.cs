namespace PtahBuilder.Tests.TestBuilder.Entities.Conditions;

public readonly struct GreaterThanCondition : ICondition<float>
{
    public GreaterThanCondition(float value, bool isExclusionary = false, bool includeEqualTo = false)
    {
        Value = value;
        IsExclusionary = isExclusionary;
        IncludeEqualTo = includeEqualTo;
    }

    public float Value { get; }
    public bool IncludeEqualTo { get; }

    public bool IsExclusionary { get; }

    public bool IsMet(float value)
    {
        if (IncludeEqualTo)
        {
            return value >= Value;
        }

        return value > Value;
    }

    public override string ToString() => $"{ConditionParser.WriteExclusionaryToken(IsExclusionary)}>{(IncludeEqualTo ? "=" : "")} {Value}";
}