namespace PtahBuilder.BuildSystem.Config;

public class StageConfig
{
    public StageConfig(Type stageType, object?[] arguments)
    {
        StageType = stageType;
        Arguments = arguments;
    }

    public Type StageType { get; }
    public object?[] Arguments { get; }
}