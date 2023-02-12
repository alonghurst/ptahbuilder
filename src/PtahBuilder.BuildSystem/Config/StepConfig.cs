namespace PtahBuilder.BuildSystem.Config;

public class StepConfig
{
    public StepConfig(Type stepType, object[] arguments)
    {
        StepType = stepType;
        Arguments = arguments;
    }

    public Type StepType { get; }
    public object[] Arguments { get; }
}