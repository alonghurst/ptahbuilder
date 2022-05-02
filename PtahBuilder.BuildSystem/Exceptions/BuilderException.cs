namespace PtahBuilder.BuildSystem.Exceptions;

public class BuilderException : Exception
{
    public BuilderException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}