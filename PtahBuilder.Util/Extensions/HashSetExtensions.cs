namespace PtahBuilder.Util.Extensions;

public static class HashSetExtensions
{
    public static void AddRange<T>(this HashSet<T> set, IEnumerable<T> range)
    {
        foreach (var r in range)
        {
            set.Add(r);
        }
    }

    public static void RemoveRange<T>(this HashSet<T> set, IEnumerable<T> range)
    {
        foreach (var r in range)
        {
            set.Remove(r);
        }
    }
}