using System.Text.Json.Serialization;

namespace PtahBuilder.Tests.TestBuilder.Entities.Probability;

public readonly struct ProbabilitySet
{
    public ProbabilitySet(float @base, params ProbabilityModifier[] modifiers)
    {
        Base = @base;
        Modifiers = modifiers;
    }

    public float Base { get; }

    public ProbabilityModifier[] Modifiers { get; }

    [JsonIgnore] 
    public bool IsDefault => Math.Abs(Base - 1) < 0.001f && Modifiers.Length == 0;

    public static ProbabilitySet Default() => new(1, Array.Empty<ProbabilityModifier>());
}