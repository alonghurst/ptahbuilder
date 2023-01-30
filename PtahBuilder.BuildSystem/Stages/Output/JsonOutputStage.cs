using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution;
using PtahBuilder.BuildSystem.Services;
using PtahBuilder.Util.Services;

namespace PtahBuilder.BuildSystem.Stages.Output
{
    public class JsonOutputStage<T> : IStage<T>
    {
        private readonly IJsonService _jsonService;
        private readonly IOutputFileService _outputFileService;

        public JsonOutputStage(IJsonService jsonService, IOutputFileService outputFileService)
        {
            _jsonService = jsonService;
            _outputFileService = outputFileService;
        }

        public async Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
        {
            foreach (var entity in entities)
            {
                var file = _outputFileService.GetOutputFileForEntity(entity, "json");

                var json = _jsonService.Serialize(entity.Value);

                await File.WriteAllTextAsync(file, json);
            }
        }
    }
}
