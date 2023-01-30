namespace PtahBuilder.Tests.TestBuilder.Entities.Conditions;

public readonly struct ExactCondition<T> : ICondition<T>
{
    public ExactCondition(T value, bool isExclusionary = false)
    {
        Value = value;
        IsExclusionary = isExclusionary;
    }

    public T Value { get; }

    public bool IsExclusionary { get; }

    public bool IsMet(T value) => value != null && value.Equals(Value);

    public override string ToString() => $"{ConditionParser.WriteExclusionaryToken(IsExclusionary)}= {Value}";
}