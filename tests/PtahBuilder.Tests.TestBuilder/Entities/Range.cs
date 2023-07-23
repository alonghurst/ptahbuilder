// ReSharper disable NonReadonlyMemberInGetHashCode

using System.Text.Json.Serialization;

namespace PtahBuilder.Tests.TestBuilder.Entities;

public readonly struct Range : IEquatable<Range>
{
    public float Min { get; }

    public float Max { get;  }

    [JsonIgnore]
    public float Mid => Diff / 2 + Min;

    [JsonIgnore]
    public float Diff => Max - Min;

    public Range(float min, float max)
    {
        Min = min;
        Max = max;
    }

    public static Range Parse(string value)
    {
        var split = value.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

        var min = Convert.ToSingle(split[0]);
        var max = min;

        if (split.Length > 1)
        {
            max = Convert.ToSingle(split[1]);
        }

        return new(min, max);
    }

    public override string ToString() => $"{Min} {Max}";

    public bool Equals(Range other)
    {
        return Min.Equals(other.Min) && Max.Equals(other.Max);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is Range && Equals((Range)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (Min.GetHashCode() * 397) ^ Max.GetHashCode();
        }
    }

    public bool Contains(float value, bool lowerInclusive = true, bool upperInclusive = true)
    {
        if (value > Min && value < Max)
        {
            return true;
        }

        if (lowerInclusive && Math.Abs(value - Min) < 0.001f)
        {
            return true;
        }
        if (upperInclusive && Math.Abs(value - Max) < 0.001f)
        {
            return true;
        }

        return false;
    }
}