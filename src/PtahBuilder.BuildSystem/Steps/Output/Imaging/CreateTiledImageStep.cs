using System.Drawing;
using System.Drawing.Imaging;
using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

#pragma warning disable CA1416 // Validate platform compatibility
namespace PtahBuilder.BuildSystem.Steps.Output.Imaging;

public record EntityImageConfig(string Filename, int EntityWidth, int EntityHeight, int Columns, ImageFormat? ImageFormat = null);

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
        
        foreach (var entity in Sort(entities))
        {
            var xPos = x * _entityImageConfig.EntityWidth;
            var yPos = y * _entityImageConfig.EntityHeight;

            var region = new Region(new Rectangle(xPos, yPos, _entityImageConfig.EntityWidth, _entityImageConfig.EntityHeight));

            graphics.Clip = region;
            graphics.TranslateTransform(xPos, yPos);

            await RenderEntity(context, entity, graphics);

            //graphics.DrawString($"{x} / {y}", ImagingDebug.Font, Brushes.Black, 2, 2);

            x++;
            if (x >= _entityImageConfig.Columns)
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

    protected sealed override ImageOutputConfig CreateConfig(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        _entityImageConfig = CreateConfig();

        var cols = _entityImageConfig.Columns < entities.Count ? _entityImageConfig.Columns : entities.Count;

        var rows = (int)Math.Ceiling((decimal)entities.Count / cols);

        var width = _entityImageConfig.EntityWidth * cols;
        var height = rows * _entityImageConfig.EntityHeight;

        return new ImageOutputConfig(_entityImageConfig.Filename, width, height, _entityImageConfig.ImageFormat);
    }



    protected abstract EntityImageConfig CreateConfig();
}

#pragma warning restore CA1416 // Validate platform compatibility