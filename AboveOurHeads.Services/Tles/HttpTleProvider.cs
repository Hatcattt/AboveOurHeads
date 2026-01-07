using AboveOurHeads.Core.Interfaces;
using AboveOurHeads.Core.Models;
using Microsoft.Extensions.Logging;

namespace AboveOurHeads.Services.Tles
{
    public class HttpTleProvider : ITleProvider
    {
        private readonly ILogger<HttpTleProvider> _logger;
        private readonly HttpClient _httpClient;
        private readonly string BaseUri = "https://celestrak.org/NORAD/elements/";

        public HttpTleProvider(
            ILogger<HttpTleProvider> logger,
            HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(BaseUri);
        }

        public async Task<TleData?> GetTleAsync(int noradId)
        {
            if (noradId <= 0)
            {
                throw new ArgumentException("The noradId number for a satellite is invalid.");
            }

            var uri = $"gp.php?CATNR={noradId}&FORMAT=TLE";
            _logger.LogInformation("Downloading TLE from {BaseUri}{Uri}", BaseUri, uri);

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                var content = await _httpClient.GetStringAsync(uri, cts.Token);

                _logger.LogInformation("TLE downloaded successfully");
                return TleParser.Parse(content);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogWarning(ex, "Request timeout while downloading TLE from {Uri}", uri);
                return null;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error while downloading TLE from {Uri}", uri);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while downloading TLE from {Uri}", uri);
                return null;
            }
        }
    }
}
