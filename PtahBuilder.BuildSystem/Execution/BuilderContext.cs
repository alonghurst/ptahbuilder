using PtahBuilder.BuildSystem.Config;

namespace PtahBuilder.BuildSystem.Execution
{
    public class BuilderContext
    {
        private readonly IServiceProvider _services;
        private readonly ExecutionConfig _config;

        public BuilderContext(IServiceProvider services, ExecutionConfig config)
        {
            _services = services;
            _config = config;
        }

        public Task Run() => Task.CompletedTask;
    }
}
