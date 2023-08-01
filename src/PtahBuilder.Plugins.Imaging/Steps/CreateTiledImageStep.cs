using System.Drawing;
using System.Drawing.Imaging;
using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

#pragma warning disable CA1416 // Validate platform compatibility
namespace PtahBuilder.Plugins.Imaging.Steps;

public record EntityImageConfig<T>(string Filename, int EntityWidth, int EntityHeight, int Columns, ImageFormat? ImageFormat = null, Func<Entity<T>, bool>? EntityFilter = null);

public abstract class CreateTiledImageStep<T> : CreateImageStep<T>
{
    private IReadOnlyCollection<EntityImageConfig<T>> _entityImageConfig = null!;

    protected CreateTiledImageStep(IFilesConfig filesConfig) : base(filesConfig)
    {
    }

    protected sealed override async Task Render(ImageOutputConfig<T> imageOutputConfig, IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities, Graphics graphics)
    {
        var config = _entityImageConfig.Single(x => x.Filename == imageOutputConfig.Filename);
        var x = 0;
        var y = 0;

        foreach (var entity in Sort(entities))
        {
            var xPos = x * config.EntityWidth;
            var yPos = y * config.EntityHeight;

            var region = new Region(new Rectangle(xPos, yPos, config.EntityWidth, config.EntityHeight));

            graphics.Clip = region;
            graphics.TranslateTransform(xPos, yPos);

            await RenderEntity(context, entity, graphics);

            x++;
            if (x >= config.Columns)
            {
                x = 0;
                y++;
            }

            graphics.ResetClip();
            graphics.ResetTransform();
        }
    }

    protected virtual IEnumerable<Entity<T>> Sort(IReadOnlyCollection<Entity<T>> entities) => entities;

    protected abstract Task RenderEntity(IPipelineContext<T> context, Entity<T> entity, Graphics graphics);

    protected override IEnumerable<ImageOutputConfig<T>> CreateConfigs(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        _entityImageConfig = CreateConfigs().ToArray();

        foreach (var config in _entityImageConfig)
        {
            var filteredEntities = config.EntityFilter != null ? entities.Where(x => config.EntityFilter(x)).ToArray() : entities;

            var cols = config.Columns < filteredEntities.Count ? config.Columns : filteredEntities.Count;

            if (cols == 0)
            {
                continue;
            }

            var rows = (int)Math.Ceiling((decimal)filteredEntities.Count / cols);

            var width = config.EntityWidth * cols;
            var height = rows * config.EntityHeight;

            yield return new(config.Filename, width, height, config.ImageFormat, config.EntityFilter);
        }
    }

    protected abstract IEnumerable<EntityImageConfig<T>> CreateConfigs();
}

#pragma warning restore CA1416 // Validate platform compatibility