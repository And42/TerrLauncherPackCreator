using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace CommonLibrary.CommonUtils
{
    public static class UpdateUtils
    {
        public static int[] ConvertVersion(string version)
        {
            List<int> parts = version.Split('.').Select(int.Parse).ToList();

            if (parts.Count > 4)
                parts.RemoveRange(4, parts.Count - 4);
            while (parts.Count < 4)
                parts.Add(0);

            return parts.ToArray();
        }

        public static string GetLatestVersion()
        {
            using (var webClient = new WebClient())
                return webClient.DownloadString(new Uri(CommonConstants.VersionFileUrl));
        }
    }
}
