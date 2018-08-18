using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace CommonLibrary.CommonUtils
{
    public static class UpdateUtils
    {
        public static async Task<int[]> GetLatestVersionFourDigitsAsync()
        {
            HtmlNode latestRelease = (await GetLatestReleaseNodeAsync()).SelectNodes("//div[contains(@class, 'release-header')]").FirstOrDefault();

            if (latestRelease == null)
                throw new Exception("Can't find latest release page");

            string latestVersion = latestRelease.Descendants("a").FirstOrDefault()?.InnerText;

            if (latestVersion == null)
                throw new Exception("Can't find latest version text");

            latestVersion = latestVersion.Substring(1);

            var result = latestVersion.Split('.').Select(int.Parse).ToList();

            if (result.Count > 4)
                result.RemoveRange(4, result.Count - 4);

            while (result.Count < 4)
                result.Add(0);

            return result.ToArray();
        }

        public static async Task<string> GetLatestVersionUrlAsync()
        {
            HtmlNode latestRelease = await GetLatestReleaseNodeAsync();

            string link = latestRelease.Descendants("details").First().Element("ul").Element("li").Element("a").GetAttributeValue("href", null);

            return "https://github.com" + link;
        }

        private static async Task<HtmlNode> GetLatestReleaseNodeAsync()
        {
            string releasePageHtml;

            using (var webClient = new WebClient())
            {
                releasePageHtml = await webClient.DownloadStringTaskAsync(CommonConstants.UpdateUrl);
            }

            var htmlDoc = new HtmlDocument();

            htmlDoc.LoadHtml(releasePageHtml);

            return htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'release-entry')]").First();
        }
    }
}
