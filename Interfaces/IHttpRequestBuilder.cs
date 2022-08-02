using System.ComponentModel;
using System.Net.Http;

namespace CoreUtilities.Interfaces
{
	public interface IHttpRequestBuilder
	{
		public enum HttpCommandType
		{
			[Description("POST")]
			Post,
			[Description("GET")]
			Get,
			[Description("DELETE")]
			Delete
		}

		IHttpRequestBuilder CreateRequest(HttpCommandType commandType, string requestTo);

		IHttpRequestBuilder WithUnvalidatedHeader(string header, string value);

		IHttpRequestBuilder WithHeaderContentType(string type);

		IHttpRequestBuilder WithContent(string name, string value);

		HttpRequestMessage Build();
	}
}
