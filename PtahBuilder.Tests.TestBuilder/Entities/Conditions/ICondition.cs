namespace PtahBuilder.Tests.TestBuilder.Entities.Conditions;

public interface ICondition
{
    bool IsExclusionary { get; }
}

public interface ICondition<T> : ICondition
{
    bool IsMet(T value);
}