using AboveOurHeads.Tests.Helpers;
using System.Net;

namespace AboveOurHeads.Tests.Helpers.HttpHandler
{
    public class SuccessTleHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var tleContent = ValidTle.Content;

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(tleContent)
            };

            return Task.FromResult(response);
        }
    }
}
