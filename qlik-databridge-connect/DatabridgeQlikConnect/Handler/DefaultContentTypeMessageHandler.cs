using System.Web;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DatabridgeQlikConnect.Handler
{
    public class DefaultContentTypeMessageHandler : DelegatingHandler
    {
        protected async override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Content.Headers.ContentType == null)
                request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await base.SendAsync(request, cancellationToken);

            return response;
        }

    }
}