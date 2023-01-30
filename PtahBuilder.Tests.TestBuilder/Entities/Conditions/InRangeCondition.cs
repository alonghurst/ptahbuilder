namespace PtahBuilder.Tests.TestBuilder.Entities.Conditions;

public readonly struct InRangeCondition : ICondition<float>
{
    public InRangeCondition(float min, float max, bool isExclusionary = false)
    {
        Min = min;
        Max = max;
        IsExclusionary = isExclusionary;
    }

    public float Min { get; }
    public float Max { get; }

    public bool IsExclusionary { get; }

    public bool IsMet(float value)
    {
        return value >= Min && value <= Max;
    }

    public override string ToString() => $"{ConditionParser.WriteExclusionaryToken(IsExclusionary)}({Min} {Max})";
}