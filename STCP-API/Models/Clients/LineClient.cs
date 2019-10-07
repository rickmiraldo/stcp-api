using HtmlAgilityPack;
using STCP_API.Models.Entities;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace STCP_API.Models.Clients
{
    public static class LineClient
    {
        private const string stcpEndpoint = "https://www.stcp.pt/pt/viajar/linhas/?linha=";

        //public static async Task<Line> GetAllStopsFromLine(string lineNumber)
        //{
        //    HttpClient client = new HttpClient();
        //    var response = await client.GetAsync(stcpEndpoint + lineNumber);

        //    // Check if page returned HTTP code 200
        //    if (response.StatusCode != System.Net.HttpStatusCode.OK)
        //    {
        //        throw new HttpRequestException("Error reading page: " + (int)response.StatusCode + " " + response.StatusCode.ToString());
        //    }

        //    var pageContents = await response.Content.ReadAsStringAsync();

        //    HtmlDocument pageDocument = new HtmlDocument();
        //    pageDocument.LoadHtml(pageContents);

        //}
    }
}
