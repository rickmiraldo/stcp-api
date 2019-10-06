using HtmlAgilityPack;
using STCP_API.Entities;
using STCP_API.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace STCP_API.Models
{
    public static class StopClient
    {
        private const string stcpEndpoint = "https://www.stcp.pt/pt/itinerarium/soapclient.php?codigo=";

        private const string resultsFilter = "(//table[contains(@id,'smsBusResults')])[1]";
        private const string warningFilter = "(//div[contains(@class,'msgBox warning')]//span)";

        private const string invalidStopMessage = "Por favor, utilize o codigo SMSBUS indicado na placa da paragem";
        private const string noBusesMessage = "Nao ha autocarros previstos para a paragem indicada nos proximos 60 minutos";

        public static async Task<Stop> GetNextBuses(string stopName)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(stcpEndpoint + stopName);

            // Check if page returned HTTP code 200
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new HttpRequestException("Error reading page: " + (int)response.StatusCode + " " + response.StatusCode.ToString());
            }

            var pageContents = await response.Content.ReadAsStringAsync();

            HtmlDocument pageDocument = new HtmlDocument();
            pageDocument.LoadHtml(pageContents);

            var resultsCheck = pageDocument.DocumentNode.SelectSingleNode(resultsFilter);

            try
            {
                if (resultsCheck == null)
                {
                    // Check for warning messags on page
                    var warningCheck = pageDocument.DocumentNode.SelectSingleNode(warningFilter).InnerText;
                    FilterWarning(warningCheck, stopName);
                }
                var incomingBuses = new List<IncomingBus>();
                incomingBuses = FindIncomingBuses(resultsCheck.OuterHtml);

                var busStop = new Stop();
                busStop.BusStopName = stopName;
                busStop.IncomingBuses = incomingBuses;

                return busStop;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void FilterWarning(string warningCheck, string stopName)
        {
            if (warningCheck.Contains(invalidStopMessage))
            {
                throw new InvalidBusStopNameException(stopName);
            }
            else if (warningCheck.Contains(noBusesMessage))
            {
                throw new NoBusesException(stopName);
            }
            throw new InvalidTableException(stopName);
        }

        private static List<IncomingBus> FindIncomingBuses(string htmlTable)
        {
            var incomingBuses = new List<IncomingBus>();

            // First filter - Get line number
            var splitNumber = htmlTable.Split("/pt/viajar/linhas/?linha=");

            for (int i = 1; i < splitNumber.Length; i++)
            {
                var bus = new IncomingBus();

                // TO-DO Validate for night buses
                var lineNumber = splitNumber[i].Substring(0, splitNumber[i].IndexOf('"')).TrimEnd();

                // Second filter - Get line name
                var splitName = splitNumber[i].Split("&nbsp;", 2);
                var lineName = splitName[1].Substring(0, splitName[1].IndexOf('<')).TrimEnd().TrimEnd('-').TrimEnd();

                // Third filter - Get ETA
                var splitTime = splitName[1].Split("<i>", 2);
                var timeString = splitTime[1].Substring(0, splitTime[1].IndexOf('<'));

                bool aPassar = false;

                DateTime time = new DateTime();

                // Watch out for different response when less than 1 minute remaining
                if (!timeString.Contains("a passar"))
                {
                    DateTime.TryParse(timeString, out time);
                }
                else
                {
                    time = DateTime.Now;
                    aPassar = true;
                }

                // Fourth filter - Get waiting time
                var splitWait = splitTime[1].Split("<td>", 2);

                int waitTime = 0;

                // If less than 1 minute remaining just keep it set to 0
                if (!aPassar)
                {
                    waitTime = int.Parse(splitWait[1].Substring(0, splitWait[1].IndexOf('m')));
                }

                bus.LineNumber = lineNumber;
                bus.LineName = lineName;
                bus.EstimatedTime = time;
                bus.WaitingTime = waitTime;

                incomingBuses.Add(bus);
            }

            return incomingBuses;
        }
    }
}
