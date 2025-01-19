namespace CoreUtilities.Helpers;

using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

public static class NetworkHelpers
{
    private static string? cachedPublicIp;

    private static HttpClient httpClient = new HttpClient();

    public static bool IsInternetConnected => NetworkInterface.GetIsNetworkAvailable();

    public static async Task<string> GetPublicIpAsync()
    {
        if (!string.IsNullOrEmpty(cachedPublicIp))
        {
            return cachedPublicIp;
        }

        try
        {
            var response = await (await httpClient.GetAsync("http://checkip.dyndns.org")).Content.ReadAsStringAsync();
            if (response.Contains("Current IP Address"))
            {
                cachedPublicIp = response.Split(':')[1].Substring(1).Split('<')[0];
                return cachedPublicIp;
            }

            return string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}
