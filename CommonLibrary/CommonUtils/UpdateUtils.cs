using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace CommonLibrary.CommonUtils
{
    public static class UpdateUtils
    {
        public static async Task<bool> IsUpdateAvailable()
        {
            int[] appVersion = ConvertVersion(GetCurrentVersion());
            int[] latestVersion = ConvertVersion(await GetLatestVersion());

            for (int i = 0; i < 4; i++)
                if (latestVersion[i] > appVersion[i])
                    return true;

            return false;
        }

        private static int[] ConvertVersion(string version)
        {
            List<int> parts = version.Split('.').Select(int.Parse).ToList();

            if (parts.Count > 4)
                parts.RemoveRange(4, parts.Count - 4);
            while (parts.Count < 4)
                parts.Add(0);

            return parts.ToArray();
        }

        private static string GetCurrentVersion()
        {
            return GetAssemblyName().Version.AssertNotNull().ToString(4);
        }

        private static async Task<string> GetLatestVersion()
        {
            using (HttpClient client = CreateHttpClient())
                return await client.GetStringAsync(new Uri(CommonConstants.VersionFileUrl));
        }

        private static HttpClient CreateHttpClient()
        {
            return new HttpClient { DefaultRequestHeaders = { { "User-Agent", $"{GetAssemblyName().Name}/{GetCurrentVersion()}" } } };
        }

        private static AssemblyName GetAssemblyName()
        {
            return Assembly.GetEntryAssembly().AssertNotNull().GetName();
        }
    }
}
