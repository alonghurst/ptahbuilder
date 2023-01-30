namespace PtahBuilder.Tests.TestBuilder.Entities.Conditions;

public readonly struct ConditionsBag
{
    public ConditionsBag(float? moistureRatio = null, float? temperature = null, float? elevationRatio = null, string biome = "", float? timeOfDay = null, string season = "", IReadOnlyCollection<string>? tags = null)
    {
        MoistureRatio = moistureRatio;
        Temperature = temperature;
        ElevationRatio = elevationRatio;
        Biome = biome;
        TimeOfDay = timeOfDay;
        Season = season;
        Tags = tags ?? Array.Empty<string>();
    }

    public ConditionsBag()
    {
        Biome = string.Empty;
        Season = string.Empty;
        Tags = Array.Empty<string>();

        MoistureRatio = null;
        Temperature = null;
        ElevationRatio = null;
        TimeOfDay = null;
    }

    public float? MoistureRatio { get; }
    public float? Temperature { get; }
    public float? ElevationRatio { get; }

    public string Biome { get; }

    public float? TimeOfDay { get; }
    public string Season { get; }

    public IReadOnlyCollection<string> Tags { get; }
}

public class Conditions
{
    public ICondition<float>? MoistureRatio { get; set; }
    public ICondition<float>? Temperature { get; set; }
    public ICondition<float>? ElevationRatio { get; set; }

    public ICondition<string>? Biome { get; set; }

    public ICondition<float>? TimeOfDay { get; set; }
    public ICondition<string>? Season { get; set; }

    public ICondition? Tags { get; set; }

    public bool IsMet(ConditionsBag bag)
    {
        return IsMet(bag.MoistureRatio,
            bag.Temperature,
            bag.ElevationRatio,
            bag.Biome,
            bag.TimeOfDay,
            bag.Season,
            bag.Tags);
    }

    public bool IsMet(
        float? moistureRatio,
        float? temperature,
        float? elevationRatio,
        string biome,
        float? timeOfDay,
        string season,
        IReadOnlyCollection<string> tags)
    {
        return Check(MoistureRatio, moistureRatio) &&
               Check(Temperature, temperature) &&
               Check(ElevationRatio, elevationRatio) &&
               Check(Biome, biome) &&
               Check(TimeOfDay, timeOfDay) &&
               Check(Season, season) &&
               Check(Tags, tags);
    }

    private bool Check(ICondition? condition, IReadOnlyCollection<string> values)
    {
        if (condition == null)
        {
            return true;
        }

        var metResult = !condition.IsExclusionary;

        if (condition is AllFromSetCondition allFromSet)
        {
            return allFromSet.IsMet(values) ? metResult : !metResult;
        }
        else if (condition is ICondition<string> stringCondition)
        {
            foreach (var value in values)
            {
                if (stringCondition.IsMet(value))
                {
                    return metResult;
                }
            }

            return !metResult;
        }

        throw new NotImplementedException($"Unable to handle condition of type \"{condition.GetType()}\"");
    }

    private bool Check(ICondition<string>? condition, string value)
    {
        if (condition == null)
        {
            return true;
        }

        var metResult = !condition.IsExclusionary;

        return condition.IsMet(value) ? metResult : !metResult;
    }

    private bool Check(ICondition<float>? condition, float? value)
    {
        if (condition == null)
        {
            return true;
        }

        var metResult = !condition.IsExclusionary;

        // There is a condition but no value so it can't be met
        if (!value.HasValue)
        {
            return !metResult;
        }

        return condition.IsMet(value.Value) ? metResult : !metResult;
    }
}