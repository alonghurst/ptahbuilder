using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.BuildSystem.Steps.Input
{
    public class HttpGetStep : IStep<string>
    {
        private readonly string _url;

        public HttpGetStep(string url)
        {
            _url = url;
        }

        public async Task Execute(IPipelineContext<string> context, IReadOnlyCollection<Entity<string>> entities)
        {
            using var httpClient = new HttpClient();

            var response = await httpClient.GetAsync(_url);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();

                context.AddEntityWithId(data, _url);
            }
            else
            {
                throw new HttpRequestException($"Failed to GET ({response.StatusCode})", null, response.StatusCode);
            }
        }
    }
}
