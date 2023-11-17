using CoreUtilities.HelperClasses.Extensions;
using CoreUtilities.Interfaces.HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace CoreUtilities.HelperClasses.HTTP
{
    /// <summary>
    /// Implementation of <see cref="IHttpRequestBuilder"/>. Provides conventient ways of building a 
    /// <see cref="HttpRequestMessage"/>.
    /// </summary>
    public class HttpRequestBuilder : IHttpRequestBuilder
    {
        private HttpRequestMessage? currentRequest;

        private string? requestType;
        private string? requestTo;
        private readonly Dictionary<string, string> unvalidatedHeaders = new Dictionary<string, string>();
        private readonly Dictionary<string, string> content = new Dictionary<string, string>();
        private string? headerContentType;

        /// <inheritdoc/>
        public IHttpRequestBuilder CreateRequest(IHttpRequestBuilder.HttpCommandType commandType, string requestTo)
        {
            try { currentRequest?.Dispose(); } catch { /* Already disposed */ }
            unvalidatedHeaders.Clear();
            content.Clear();
            this.requestTo = requestTo;
            requestType = commandType.GetEnumDescription();
            headerContentType = string.Empty;
            return this;
        }

        /// <inheritdoc/>
        public IHttpRequestBuilder WithContent(string name, string value)
        {
            content[name] = value;
            return this;
        }

        /// <inheritdoc/>
        public IHttpRequestBuilder WithHeaderContentType(string type)
        {
            headerContentType = type;
            return this;
        }

        /// <inheritdoc/>
        public IHttpRequestBuilder WithUnvalidatedHeader(string header, string value)
        {
            unvalidatedHeaders[header] = value;
            return this;
        }

        /// <inheritdoc/>
        public HttpRequestMessage Build()
        {
            currentRequest = new HttpRequestMessage(new HttpMethod(requestType!), requestTo);

            if (unvalidatedHeaders.Any())
            {
                foreach (var kvp in unvalidatedHeaders)
                {
                    currentRequest.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);
                }
            }

            if (content.Any())
            {
                StringBuilder contentSb = new StringBuilder().Append("{ ");
                int i = 0;
                foreach (var kvp in content)
                {
                    i++;
                    var comma = i == content.Count ? " " : ", ";
                    contentSb.Append($"\"{kvp.Key}\": \"{kvp.Value}\"{comma}");
                }
                contentSb.Append(" }");
                currentRequest.Content = new StringContent(contentSb.ToString());
            }

            if (!string.IsNullOrEmpty(headerContentType) && currentRequest.Content != null)
            {
                currentRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(headerContentType);
            }
            else
            {
                throw new InvalidOperationException("Missing content or header content type");
            }

            return currentRequest;
        }
    }
}
