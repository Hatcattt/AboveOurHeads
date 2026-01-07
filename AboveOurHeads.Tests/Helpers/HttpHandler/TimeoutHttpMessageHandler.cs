namespace AboveOurHeads.Tests.Helpers.HttpHandler
{
    public class TimeoutHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            throw new TaskCanceledException("Request timed out");
        }
    }
}
