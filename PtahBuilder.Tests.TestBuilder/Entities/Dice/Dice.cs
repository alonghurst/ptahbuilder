namespace PtahBuilder.Tests.TestBuilder.Entities.Dice;

public readonly struct Dice : IDiceValue
{
    public int Sides { get; }
    public int Quantity { get; }

    public Dice(int sides, int quantity = 1)
    {
        Sides = sides;
        Quantity = quantity;
    }

    public override string ToString()
    {
        if (Quantity == 1)
        {
            return $"d{Sides}";
        }

        return $"{Quantity}d{Sides}";
    }

    public static Dice Parse(string text)
    {
        var parts = text.ToLower().Split('d', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 2)
        {
            return new Dice(Convert.ToInt32(parts[1]), Convert.ToInt32(parts[0]));
        }

        return new Dice(Convert.ToInt32(parts[0]));
    }
}