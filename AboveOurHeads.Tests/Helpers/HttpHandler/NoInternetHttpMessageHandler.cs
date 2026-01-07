namespace AboveOurHeads.Tests.Helpers.HttpHandler
{
    public class NoInternetHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            throw new HttpRequestException("No internet connection");
        }
    }

}
