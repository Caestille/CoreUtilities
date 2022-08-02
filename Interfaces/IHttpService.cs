using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CoreUtilities.Interfaces
{
	public interface IHttpService
	{
		Task<(bool, string)> WaitForAndQueryResponseOverUri(string callbackUri, string query, CancellationToken? token = null);

		IHttpRequestBuilder GetHttpRequestBuilder();

		Task<(HttpStatusCode, string)> SendAsyncDisposeAndGetResponse(HttpRequestMessage request, CancellationToken? token = null);
	}
}
