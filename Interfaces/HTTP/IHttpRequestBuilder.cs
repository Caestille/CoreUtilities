namespace CoreUtilities.Interfaces.HTTP;

using System.ComponentModel;
using System.Net.Http;

/// <summary>
/// Interface for a class creating an http request builder. Allows for easy creation and execution of http
/// requests.
/// </summary>
public interface IHttpRequestBuilder
{
    /// <summary>
    /// Adds an unvalidated header to a <see cref="HttpRequestMessage"/>.
    /// </summary>
    /// <param name="header">Header to add.</param>
    /// <param name="value">Value to add.</param>
    /// <returns>An instance of a <see cref="IHttpRequestBuilder"/> which can be added to or built.</returns>
    IHttpRequestBuilder WithUnvalidatedHeader(string header, string value);

    /// <summary>
    /// Sets the header content type.
    /// </summary>
    /// <param name="type">The header content type.</param>
    /// <returns>An instance of a <see cref="IHttpRequestBuilder"/> which can be added to or built.</returns>
    IHttpRequestBuilder WithHeaderContentType(string type);

    /// <summary>
    /// Adds content to the <see cref="HttpRequestMessage"/>.
    /// </summary>
    /// <param name="name">Content name.</param>
    /// <param name="value">Content value.</param>
    /// <returns>An instance of a <see cref="IHttpRequestBuilder"/> which can be added to or built.</returns>
    IHttpRequestBuilder WithContent(string name, string value);

    /// <summary>
    /// Build the <see cref="HttpRequestMessage"/> and returns it ready for sending.
    /// </summary>
    /// <returns>A <see cref="HttpRequestMessage"/>.</returns>.
    HttpRequestMessage Build();
}
