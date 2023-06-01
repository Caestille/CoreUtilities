using System.IO;
using System.Net.NetworkInformation;
using System.Net;

namespace CoreUtilities.HelperClasses
{
	public static class NetworkHelpers
	{
		private static string cachedPublicIp;

		public static string PublicIP
		{
			get
			{
				if (!string.IsNullOrEmpty(cachedPublicIp))
				{
					return cachedPublicIp;
				}

				try
				{
					string url = "http://checkip.dyndns.org";
					WebRequest req = WebRequest.Create(url);
					WebResponse resp = req.GetResponse();
					StreamReader sr = new StreamReader(resp.GetResponseStream());
					string response = sr.ReadToEnd().Trim();
					string[] a = response.Split(':');
					string a2 = a[1].Substring(1);
					string[] a3 = a2.Split('<');
					string a4 = a3[0];
					cachedPublicIp = a4;
					return a4;
				}
				catch
				{
					return "";
				}
			}
		}

		public static bool IsInternetConnected => NetworkInterface.GetIsNetworkAvailable();
	}
}
