namespace PtahBuilder.Tests.TestBuilder.Entities.Dice;

public readonly struct ConstantDiceValue : IDiceValue
{
    public ConstantDiceValue(int value)
    {
        Value = value;
    }

    public int Value { get; }

    public override string ToString() => Value.ToString();


    public static explicit operator ConstantDiceValue(int v) => new(v);
}