namespace PtahBuilder.BuildSystem.Entities;

public class Validation
{
    public bool IsValid => Errors.Count == 0;

    public List<ValidationError> Errors { get; } = new();
}

public readonly struct ValidationError
{
    public ValidationError(string source, string error)
    {
        Source = source;
        Error = error;
    }

    public string Source { get; }
    public string Error { get; }
}