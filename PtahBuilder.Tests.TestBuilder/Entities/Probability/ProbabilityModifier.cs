namespace PtahBuilder.Tests.TestBuilder.Entities.Probability;

public struct ProbabilityModifier
{
    public ProbabilityModifier(float value, ProbabilityModifierType modifierType, Conditions.Conditions conditions)
    {
        Value = value;
        Modifier = modifierType;
        Conditions = conditions;
    }

    public float Value { get; private set; }

    public ProbabilityModifierType Modifier { get; private set; }

    public Conditions.Conditions Conditions { get; private set; }

    public override string ToString()
    {
        return $"{Modifier} {Value}";
    }
}