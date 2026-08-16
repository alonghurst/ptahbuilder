namespace PtahBuilder.Tests.TestBuilder.Entities;

public class Fruit: TypeData
{
    public string Colour { get; set; } = string.Empty;
    public bool HasSeeds { get; set; }

    public FruityEnum Fruitiness { get; set; }
}