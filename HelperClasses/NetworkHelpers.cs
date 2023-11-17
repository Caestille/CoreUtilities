using System.IO;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CoreUtilities.HelperClasses
{
	public static class NetworkHelpers
	{
		private static string? cachedPublicIp;

		private static HttpClient httpClient = new HttpClient();

		public static async Task<string> GetPublicIpAsync()
		{
			if (!string.IsNullOrEmpty(cachedPublicIp))
			{
				return cachedPublicIp;
			}

			try
			{
				var response = await (await httpClient.GetAsync("http://checkip.dyndns.org")).Content.ReadAsStringAsync();
				cachedPublicIp = response.Split(':')[1].Substring(1).Split('<')[0];
				return cachedPublicIp;
			}
			catch
			{
				return "";
			}
		}

		public static bool IsInternetConnected => NetworkInterface.GetIsNetworkAvailable();
	}
}
