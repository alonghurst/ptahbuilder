using System.Drawing;
using System.Drawing.Imaging;
using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

#pragma warning disable CA1416 // Validate platform compatibility
namespace PtahBuilder.BuildSystem.Steps.Output.Imaging;

public record ImageOutputConfig(string Filename, int Width, int Height, ImageFormat? ImageFormat = null);

public abstract class CreateImageStep<T> : IStep<T>
{
    private readonly IFilesConfig _filesConfig;

    protected CreateImageStep(IFilesConfig filesConfig)
    {
        _filesConfig = filesConfig;
    }

    public async Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        var config = CreateConfig(context, entities);

        using (var image = new Bitmap(config.Width, config.Height))
        {
            using (var graphics = Graphics.FromImage(image))
            {
                graphics.Clear(Color.White);

                await Render(context, entities, graphics);
            }

            var path = Path.Combine(_filesConfig.OutputDirectory, config.Filename);

            if (!Directory.Exists(_filesConfig.OutputDirectory))
            {
                Directory.CreateDirectory(_filesConfig.OutputDirectory);
            }

            image.Save(path, config.ImageFormat ?? ImageFormat.Png);
        }
    }

    protected abstract Task Render(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities, Graphics graphics);

    protected abstract ImageOutputConfig CreateConfig(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities);
}
#pragma warning restore CA1416 // Validate platform compatibility