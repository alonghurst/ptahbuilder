using System.Diagnostics;

namespace PtahBuilder.Tests.TestBuilder.Entities.Dice;

public interface IDiceResult
{
    int Result { get; }
}

[DebuggerDisplay("{Result}")]
public readonly struct ConstantResult : IDiceResult
{
    public ConstantResult(int result)
    {
        Result = result;
    }

    public int Result { get; }
}

[DebuggerDisplay("{Result} / {Sides}")]
public readonly struct DiceResult : IDiceResult
{
    public DiceResult(int sides, int result)
    {
        Sides = sides;
        Result = result;
    }

    public int Result { get; }
    public int Sides { get; }

    public bool WasCritical => Result == Sides;
}

public readonly struct DiceSetResult : IDiceResult
{
    public DiceSetResult(IEnumerable<DiceResult> dice)
    {
        Dice = dice.ToArray();
        Result = Dice.Sum(x => x.Result);
    }

    public int Result { get; }
    public IReadOnlyCollection<DiceResult> Dice { get; }
}

public readonly struct DiceEquationResult : IDiceResult
{
    public DiceEquationResult(IDiceResult left, IDiceResult right, EquationOperator @operator)
    {
        Left = left;
        Right = right;
        Operator = @operator;
        Result = @operator == EquationOperator.Add ? Left.Result + Right.Result : Left.Result - Right.Result;
    }

    public EquationOperator Operator { get; }

    public int Result { get; }

    public IDiceResult Right { get; }

    public IDiceResult Left { get; }
}