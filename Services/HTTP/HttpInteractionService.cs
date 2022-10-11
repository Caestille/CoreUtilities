using CoreUtilities.HelperClasses.Extensions;
using CoreUtilities.Interfaces.HTTP;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoreUtilities.Services.HTTP
{
    /// <summary>
    /// Implementation of <see cref="IHttpService"/>. Implements handy ways to create and send 
    /// <see cref="HttpRequestMessage"/>s.
    /// </summary>
    public class HttpInteractionService : IHttpService
    {
        private HttpClient httpClient = new HttpClient();
        private Func<IHttpRequestBuilder> httpRequestBuilderCreator;

        /// <summary>
        /// Constructor for the <see cref="HttpInteractionService"/>.
        /// </summary>
        /// <param name="builderCreateFunc">A <see cref="Func{T}"/> which returns an instance of a 
        /// <see cref="IHttpRequestBuilder"/>.</param>
        public HttpInteractionService(Func<IHttpRequestBuilder> builderCreateFunc)
        {
            httpRequestBuilderCreator = builderCreateFunc;
        }

        /// <inheritdoc/>
        public IHttpRequestBuilder GetHttpRequestBuilder()
        {
            return httpRequestBuilderCreator();
        }

        /// <inheritdoc/>
        public async Task<(bool, string)> WaitForAndQueryResponseOverUri(
            string callbackUri, string query, CancellationToken? token = null)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add(callbackUri);
            listener.Start();

            HttpListenerContext? context = null;
            try
            {
                context = await listener.GetContextAsync().AsCancellable(token ?? CancellationToken.None);
            }
            catch (TaskCanceledException)
            {
                listener.Stop();
                return (false, string.Empty);
            }

            string html = string.Format("<html><body></body></html>");
            var buffer = Encoding.UTF8.GetBytes(html);
            context.Response.ContentLength64 = buffer.Length;
            var stream = context.Response.OutputStream;
            var responseTask = stream.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
            {
                stream.Close();
                listener.Stop();
            });

            string? result = context.Request.QueryString[query];

            return (result != null, result ?? string.Empty);
        }

        /// <inheritdoc/>
        public async Task<(HttpStatusCode, string)> SendAsyncDisposeAndGetResponse(
            HttpRequestMessage request, CancellationToken? token)
        {
            HttpResponseMessage response = null;
            try
            {
                response = await httpClient.SendAsync(request).AsCancellable(token ?? CancellationToken.None);
            }
            catch (TaskCanceledException e)
            {
                // Task was cancelled
                // TODO: Logging
            }
            finally
            {
                request.Dispose();
            }

            return (response != null ? response.StatusCode : HttpStatusCode.ServiceUnavailable,
                response != null ? await response.Content.ReadAsStringAsync() : string.Empty);
        }
    }
}
