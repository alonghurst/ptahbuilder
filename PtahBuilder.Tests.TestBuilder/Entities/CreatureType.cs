using PtahBuilder.Tests.TestBuilder.Entities.Dice;
using PtahBuilder.Tests.TestBuilder.Entities.Probability;

namespace PtahBuilder.Tests.TestBuilder.Entities;

public class CreatureType
{
    public string Name { get; set; } = string.Empty;
    
    public string TypeName { get; set; } = string.Empty;

    public bool CanBePlayerType { get; set; }

    public CreatureSize Size { get; set; } = CreatureSize.Medium;

    public bool HasBiography { get; set; }

    public Culture[] Cultures { get; set; } = Array.Empty<Culture>();

    public Conditions.Conditions? Conditions { get; set; }

    public ProbabilitySet Probability { get; set; } = ProbabilitySet.Default();

    public string[] Tags { get; set; } = Array.Empty<string>();

    public Rarity Rarity { get; set; } = Rarity.Common;

    public float MoveSpeed { get; set; } = 1f;

    public IDiceValue Health { get; set; } = new ConstantDiceValue(1);
}