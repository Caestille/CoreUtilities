namespace CoreUtilities.Helpers.HTTP;

using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using CoreUtilities.Helpers.Extensions;
using CoreUtilities.Interfaces.HTTP;

/// <summary>
/// Implementation of <see cref="IHttpRequestBuilder"/>. Provides conventient ways of building a
/// <see cref="HttpRequestMessage"/>.
/// </summary>
public class HttpRequestBuilder : IHttpRequestBuilder
{
    private readonly Dictionary<string, string> unvalidatedHeaders = new Dictionary<string, string>();
    private readonly Dictionary<string, string> content = new Dictionary<string, string>();
    private HttpRequestMessage? currentRequest;
    private string? requestType;
    private string? requestTo;
    private string? headerContentType;

    /// <inheritdoc/>
    public IHttpRequestBuilder CreateRequest(IHttpRequestBuilder.HttpCommandType commandType, string requestTo)
    {
        try
        {
            this.currentRequest?.Dispose();
        }
        catch
        {
            /* Already disposed */
        }

        this.unvalidatedHeaders.Clear();
        this.content.Clear();
        this.requestTo = requestTo;
        this.requestType = commandType.GetEnumDescription();
        this.headerContentType = string.Empty;
        return this;
    }

    /// <inheritdoc/>
    public IHttpRequestBuilder WithContent(string name, string value)
    {
        this.content[name] = value;
        return this;
    }

    /// <inheritdoc/>
    public IHttpRequestBuilder WithHeaderContentType(string type)
    {
        this.headerContentType = type;
        return this;
    }

    /// <inheritdoc/>
    public IHttpRequestBuilder WithUnvalidatedHeader(string header, string value)
    {
        this.unvalidatedHeaders[header] = value;
        return this;
    }

    /// <inheritdoc/>
    public HttpRequestMessage Build()
    {
        this.currentRequest = new HttpRequestMessage(new HttpMethod(this.requestType!), this.requestTo);

        if (this.unvalidatedHeaders.Count != 0)
        {
            foreach (var kvp in this.unvalidatedHeaders)
            {
                this.currentRequest.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);
            }
        }

        if (this.content.Count != 0)
        {
            StringBuilder contentSb = new StringBuilder().Append("{ ");
            var i = 0;
            foreach (var kvp in this.content)
            {
                i++;
                var comma = i == this.content.Count ? " " : ", ";
                contentSb.Append($"\"{kvp.Key}\": \"{kvp.Value}\"{comma}");
            }

            contentSb.Append(" }");
            this.currentRequest.Content = new StringContent(contentSb.ToString());
        }

        if (!string.IsNullOrEmpty(this.headerContentType) && this.currentRequest.Content != null)
        {
            this.currentRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(this.headerContentType);
        }

        return this.currentRequest;
    }
}
