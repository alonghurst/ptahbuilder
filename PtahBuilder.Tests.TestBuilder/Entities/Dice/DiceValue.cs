namespace PtahBuilder.Tests.TestBuilder.Entities.Dice;

public class DiceValue : IDiceValue
{
    public DiceValue(IDiceValue left, IDiceValue right, EquationOperator @operator)
    {
        Left = left;
        Right = right;
        Operator = @operator;
    }

    public IDiceValue Left { get; }
    public IDiceValue Right { get; }
    public EquationOperator Operator { get; }

    public override string ToString()
    {
        var op = Operator == EquationOperator.Add ? "+" : "-";

        return $"({Left} {op} {Right})";
    }
}

public enum EquationOperator
{
    Add,
    Subtract
}