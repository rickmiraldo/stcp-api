using HtmlAgilityPack;
using STCP_API.Models.Entities;
using STCP_API.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace STCP_API.Models.Clients
{
    public static class StopClient
    {
        private const string StcpEndpoint = "https://www.stcp.pt/pt/itinerarium/soapclient.php?codigo=";

        private const string ResultsFilter = "(//table[contains(@id,'smsBusResults')])[1]";
        private const string WarningFilter = "(//div[contains(@class,'msgBox warning')]//span)";

        private const string InvalidStopMessage = "Por favor, utilize o codigo SMSBUS indicado na placa da paragem";
        private const string NoBusesMessage = "Nao ha autocarros previstos para a paragem indicada nos proximos 60 minutos";

        public static async Task<Stop> GetNextBuses(string stopName, string busStopName = "", string zone = "")
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(StcpEndpoint + stopName);

            // Check if page returned HTTP code 200
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new HttpRequestException("Error reading page: " + (int)response.StatusCode + " " + response.StatusCode.ToString());
            }

            var pageContents = await response.Content.ReadAsStringAsync();

            HtmlDocument pageDocument = new HtmlDocument();
            pageDocument.LoadHtml(pageContents);

            var resultsCheck = pageDocument.DocumentNode.SelectSingleNode(ResultsFilter);

            try
            {
                if (resultsCheck == null)
                {
                    // Check for warning messages on page
                    var warningCheck = pageDocument.DocumentNode.SelectSingleNode(WarningFilter).InnerText;
                    if (warningCheck.Contains(InvalidStopMessage))
                    {
                        throw new InvalidBusStopNameException(stopName);
                    }
                    else if (warningCheck.Contains(NoBusesMessage))
                    {
                        var incomingBuses = new List<IncomingBus>();
                        var busStop = new Stop(stopName, incomingBuses, busStopName, zone);
                        return busStop;
                    }
                    else
                    {
                        throw new InvalidTableException(stopName);
                    }
                }
                else
                {
                    var incomingBuses = new List<IncomingBus>();
                    incomingBuses = ParseIncomingBuses(resultsCheck.OuterHtml);

                    var busStop = new Stop(stopName, incomingBuses, busStopName, zone);

                    return busStop;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static List<IncomingBus> ParseIncomingBuses(string htmlTable)
        {
            var incomingBuses = new List<IncomingBus>();

            // First filter - Get line number
            var splitNumber = htmlTable.Split("/pt/viajar/linhas/?linha=");

            for (int i = 1; i < splitNumber.Length; i++)
            {
                // TO-DO Validate for night buses
                var lineNumber = splitNumber[i].Substring(0, splitNumber[i].IndexOf('"')).TrimEnd();

                // Second filter - Get line name
                var splitName = splitNumber[i].Split("&nbsp;", 2);
                var lineName = splitName[1].Substring(0, splitName[1].IndexOf('<')).TrimEnd().TrimEnd('-').TrimEnd();

                // Third filter - Get ETA
                var splitTime = splitName[1].Split("<i>", 2);
                var timeString = splitTime[1].Substring(0, splitTime[1].IndexOf('<'));

                bool aPassar = false;

                // Watch out for different response when less than 1 minute remaining
                if (timeString.Contains("a passar"))
                {
                    timeString = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString();
                    aPassar = true;
                }

                DateTime time = new DateTime();
                DateTime.TryParse(timeString, out time);

                // TO-DO Validate if the code below works
                // Check if it's past midnight, adjust date accordingly
                if (DateTime.Now.Hour <= 23 && time.Hour == 0)
                {
                    time = time.AddDays(1);
                }

                // Fourth filter - Get waiting time
                var splitWait = splitTime[1].Split("<td>", 2);

                int waitTime = 0;

                // If less than 1 minute remaining just keep it set to 0
                if (!aPassar)
                {
                    waitTime = int.Parse(splitWait[1].Substring(0, splitWait[1].IndexOf('m')));
                }

                var bus = new IncomingBus(lineNumber, lineName, time, waitTime);

                incomingBuses.Add(bus);
            }

            return incomingBuses;
        }
    }
}
