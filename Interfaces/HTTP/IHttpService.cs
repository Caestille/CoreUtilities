using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CoreUtilities.Interfaces.HTTP
{
    /// <summary>
    /// Interface for service implementing useful HTTP interactions in an easy to use manner.
    /// </summary>
    public interface IHttpService
    {
        /// <summary>
        /// Executes a query and waits (indefinitely) for a response over a given URI, unless cancelled via a given
        /// <see cref="CancellationToken"/>.
        /// </summary>
        /// <param name="callbackUri">The URI to wait for a response over.</param>
        /// <param name="query">The query to execute.</param>
        /// <param name="token">A <see cref="CancellationToken"/> used to cancel the query/wait manually.</param>
        /// <returns>A <see cref="Task{TResult}"/> which is the awaitable wrapper for the results of the query.
        /// </returns>
        Task<(bool, string)> WaitForAndQueryResponseOverUri(
            string callbackUri, string query, CancellationToken? token = null);

        /// <summary>
        /// Returns an instance of a <see cref="IHttpRequestBuilder"/> for building and then executing 
        /// <see cref="HttpRequestMessage"/>s.
        /// </summary>
        /// <returns>A <see cref="IHttpRequestBuilder"/>.</returns>
        IHttpRequestBuilder GetHttpRequestBuilder();

        /// <summary>
        /// Send a <see cref="HttpRequestMessage"/>, and waits for a response in an async manner. The request is then
        /// disposed.
        /// </summary>
        /// <param name="request">The request to send.</param>
        /// <param name="token">A <see cref="CancellationToken"/> used to cancel the request/wait manually.</param>
        /// <returns>A <see cref="Task{TResult}"/> which is the awaitable wrapper containing the status code of the 
        /// requests and the response string.</returns>
        Task<(HttpStatusCode, string)> SendAsyncDisposeAndGetResponse(
            HttpRequestMessage request, CancellationToken? token = null);
    }
}
