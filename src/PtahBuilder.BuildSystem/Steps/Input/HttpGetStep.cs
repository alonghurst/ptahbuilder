using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.BuildSystem.Steps.Input;

public class HttpGetStep : IStep<string>
{
    private readonly string[] _urls;

    public HttpGetStep(string[] urls)
    {
        _urls = urls;
    }

    public HttpGetStep(string url) : this(new[] { url })
    {

    }

    public async Task Execute(IPipelineContext<string> context, IReadOnlyCollection<Entity<string>> entities)
    {
        // ReSharper disable once ConvertToUsingDeclaration
        using (var httpClient = new HttpClient())
        {
            var tasks = _urls.Select(async url =>
            {
                // ReSharper disable once AccessToDisposedClosure
                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();

                    context.AddEntityWithId(data, url);
                }
                else
                {
                    throw new HttpRequestException($"Failed to GET ({response.StatusCode})", null, response.StatusCode);
                }
            });

            await Task.WhenAll(tasks);
        }
    }
}