using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.BuildSystem.Steps.Output.SimpleText;
using PtahBuilder.Util.Extensions;

namespace PtahBuilder.BuildSystem.Steps.Input.SimpleText;

public class SimpleTextInputStep:IStep<SimpleTextInput>
{
    private readonly IFilesConfig _filesConfig;
    private readonly SimpleTextInputConfig _config;

    public SimpleTextInputStep(IFilesConfig filesConfig, SimpleTextInputConfig config)
    {
        _filesConfig = filesConfig;
        _config = config;
    }

    public class SimpleTextInputConfig
    {
        public string RelativeDirectory { get; set; } = string.Empty;
        public string[] FileTypes { get; set; } = Array.Empty<string>();
        public Func<SimpleTextInput,bool>? ShouldBeExcluded { get; set; }
    }

    public Task Execute(IPipelineContext<SimpleTextInput> context, IReadOnlyCollection<Entity<SimpleTextInput>> entities)
    {
        _config.FileTypes = _config.FileTypes.Select(x => x.WithDot()).ToArray();

        var directory = Path.Combine(_filesConfig.WorkingDirectory, _config.RelativeDirectory);

        ReadDirectory(context, directory);

        return Task.CompletedTask;
    }

    private void ReadDirectory(IPipelineContext<SimpleTextInput> context, string directory)
    {
        foreach (var child in Directory.GetDirectories(directory))
        {
            ReadDirectory(context, child);
        }

        ReadFiles(context, directory);
    }

    private void ReadFiles(IPipelineContext<SimpleTextInput> context, string directory)
    {
        foreach (var file in Directory.GetFiles(directory))
        {
            var fileInfo = new FileInfo(file);

            if (_config.FileTypes.Any() && !_config.FileTypes.Contains(fileInfo.Extension))
            {
                continue;
            }

            var contents = File.ReadAllText(file);

            var simple = new SimpleTextInput(fileInfo.Name, fileInfo.Extension, fileInfo.Directory?.FullName ?? directory, contents);

            if (_config.ShouldBeExcluded == null || !_config.ShouldBeExcluded(simple))
            {
                context.AddEntityWithId(simple, fileInfo.FullName);
            }
        }
    }
}