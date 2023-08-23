using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;

namespace PtahBuilder.BuildSystem.Steps.Input
{
    public class HttpGetStep : IStep<string>
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;

        public HttpGetStep(HttpClient httpClient, string url)
        {
            _httpClient = httpClient;
            _url = url;
        }

        public async Task Execute(IPipelineContext<string> context, IReadOnlyCollection<Entity<string>> entities)
        {
            var response = await _httpClient.GetAsync(_url);

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
