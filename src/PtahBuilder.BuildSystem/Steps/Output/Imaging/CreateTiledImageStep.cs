using System.Drawing;
using System.Drawing.Imaging;
using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

#pragma warning disable CA1416 // Validate platform compatibility
namespace PtahBuilder.BuildSystem.Steps.Output.Imaging;

public record EntityImageConfig(string Filename, int Width, int Height, int Columns, ImageFormat? ImageFormat = null);

public abstract class CreateTiledImageStep<T> : CreateImageStep<T>
{
    private EntityImageConfig _entityImageConfig = null!;

    protected CreateTiledImageStep(IFilesConfig filesConfig) : base(filesConfig)
    {
    }

    protected sealed override async Task Render(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities, Graphics graphics)
    {
        var x = 0;
        var y = 0;

        foreach (var entity in entities)
        {
            var xPos = x * _entityImageConfig.Width;
            var yPos = y * _entityImageConfig.Height;
            var region = new Region(new Rectangle(xPos, yPos, _entityImageConfig.Width, _entityImageConfig.Height));

            graphics.Clip = region;
            graphics.TranslateTransform(xPos, yPos);

            await RenderEntity(context, entity, graphics);

            x++;
            if (x >= _entityImageConfig.Columns)
            {
                x = 0;
                y++;
            }
        }

        graphics.Clip = new Region();
        graphics.TranslateTransform(0, 0);
    }

    protected abstract Task RenderEntity(IPipelineContext<T> context, Entity<T> entity, Graphics graphics);

    protected sealed override ImageOutputConfig CreateConfig(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        _entityImageConfig = CreateConfig();

        var rows = (int)Math.Ceiling((decimal)entities.Count / _entityImageConfig.Columns);

        var width = _entityImageConfig.Width * _entityImageConfig.Columns;
        var height = rows * _entityImageConfig.Height;

        return new(_entityImageConfig.Filename, width, height, _entityImageConfig.ImageFormat);
    }

    protected abstract EntityImageConfig CreateConfig();
}

#pragma warning restore CA1416 // Validate platform compatibility