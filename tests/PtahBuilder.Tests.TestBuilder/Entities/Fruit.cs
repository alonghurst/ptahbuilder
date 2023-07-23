namespace PtahBuilder.Tests.TestBuilder.Entities;

public class Fruit: TypeData
{
    public string Name { get; set; } = string.Empty;
    public string Colour { get; set; } = string.Empty;
    public bool HasSeeds { get; set; }
}