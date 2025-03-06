namespace CoreUtilities.Services.HTTP;

using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoreUtilities.Helpers.Extensions;
using CoreUtilities.Helpers.HTTP;
using CoreUtilities.Interfaces.HTTP;

/// <summary>
/// Implementation of <see cref="IHttpRequestService"/>. Implements handy ways to create and send
/// <see cref="HttpRequestMessage"/>s.
/// </summary>
public class HttpRequestService : IHttpRequestService
{
    private readonly HttpClient httpClient = new HttpClient();

    /// <inheritdoc/>
    public IHttpRequestBuilder CreateRequestBuilder(string httpMethod, string requestUri)
        => new HttpRequestBuilder(httpMethod, requestUri);

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
    public async Task<(HttpStatusCode, string)> SendAsync(HttpRequestMessage request, CancellationToken? token)
    {
        try
        {
            var response = await this.httpClient.SendAsync(request).AsCancellable(token ?? CancellationToken.None);
            return (response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (TaskCanceledException)
        {
            // Task was cancelled
            // TODO: Logging

            return (HttpStatusCode.ServiceUnavailable, string.Empty);
        }
        finally
        {
            request.Dispose();
        }
    }
}
