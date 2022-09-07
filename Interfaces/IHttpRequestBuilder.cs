using System.ComponentModel;
using System.Net.Http;

namespace CoreUtilities.Interfaces
{
	/// <summary>
	/// Interface for a class creating an http request builder. Allows for easy creation and execution of http 
	/// requests.
	/// </summary>
	public interface IHttpRequestBuilder
	{
		/// <summary>
		/// Enum of HTTP request types.
		/// </summary>
		public enum HttpCommandType
		{
			[Description("POST")]
			Post,
			[Description("GET")]
			Get,
			[Description("DELETE")]
			Delete
		}

		/// <summary>
		/// Creates a request which can have content/headers added to, then built.
		/// </summary>
		/// <param name="commandType">A <see cref="HttpCommandType"/> enum indicating what type of request this 
		/// is.</param>
		/// <param name="requestTo">The url to send the request to.</param>
		/// <returns>An instance of a <see cref="IHttpRequestBuilder"/> which can be added to or built.</returns>
		IHttpRequestBuilder CreateRequest(HttpCommandType commandType, string requestTo);

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
		/// <returns>A <see cref="HttpRequestMessage"/.></returns>
		HttpRequestMessage Build();
	}
}
