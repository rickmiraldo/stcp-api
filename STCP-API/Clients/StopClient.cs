using HtmlAgilityPack;
using System.Net.Http;
using System.Threading.Tasks;

namespace STCP_API.Clients
{
    public static class StopClient
    {
        private const string stcpEndpoint = "https://www.stcp.pt/pt/itinerarium/soapclient.php?codigo=";
        private const string stopFilter = "(//table[contains(@id,'smsBusResults')])[1]";
        private const string errorFilter = "(//div[contains(@class,'msgBox warning')]//span)";

        public static async Task<string> GetNextBuses(string name)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(stcpEndpoint + name);
            var pageContents = await response.Content.ReadAsStringAsync();

            HtmlDocument pageDocument = new HtmlDocument();
            pageDocument.LoadHtml(pageContents);

            var table = pageDocument.DocumentNode.SelectSingleNode(errorFilter).InnerText;

            return table;
        }
    }
}
