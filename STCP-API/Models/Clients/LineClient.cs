using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using STCP_API.Models.Entities;
using STCP_API.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace STCP_API.Models.Clients
{
    public static class LineClient
    {
        private const string StcpEndpoint = "https://www.stcp.pt/pt/viajar/linhas/?linha=";
        private const string StcpEndpoint2 = "&sentido=";
        private const string StcpEndpoint3 = "&t=horarios";

        private const string FilterKeyword = "filter";
        private const string FullKeyword = "full";

        private const string Line505StopLion1Direction0BusName = ""; // TO-DO Get bus name
        private const string Line505StopLion1Direction1BusName = "H.S.JOÃO CIR";

        private const string ResultsFilter = "(//div[contains(@id,'bus-stop-results')])";

        public static async Task<Line> GetStopsFromLine(string lineNumber, string direction, string getIncomingBuses)
        {
            bool alsoCheckIncomingBuses = (getIncomingBuses.Equals(FullKeyword) || getIncomingBuses.Equals(FilterKeyword)) ? true : false;
            bool filterUnwantedLines = getIncomingBuses.Equals(FilterKeyword) ? true : false;
            string directionToCheck = direction.Equals("1") ? "1" : "0";
            
            var co = new ChromeOptions();
            co.AddArgument("headless");
            co.AcceptInsecureCertificates = true;
            co.PageLoadStrategy = PageLoadStrategy.Normal;

            using (var driver = new ChromeDriver(co))
            {
                try
                {
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                    driver.Navigate().GoToUrl(StcpEndpoint + lineNumber + StcpEndpoint2 + directionToCheck + StcpEndpoint3);
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                    wait.Until(wt => !string.IsNullOrWhiteSpace(wt.FindElement(By.CssSelector("#bus-stop-results > table > tbody > tr:nth-child(1) > th.paragem")).Text));

                    var resultString = driver.FindElementByXPath(ResultsFilter).GetAttribute("outerHTML");
                    var lineDirection = driver.FindElementByXPath("/html/body/div[3]/div[1]/div[1]/table/tbody/tr/td/div/div[1]/div[1]/div/div[2]").Text;

                    driver.Close();

                    if (string.IsNullOrEmpty(lineDirection))
                    {
                        throw new InvalidLineNumberException(lineNumber);
                    }
                    else if (string.IsNullOrEmpty(resultString))
                    {
                        throw new HttpRequestException("Error reading page: No results found!");
                    }

                    var busStops = new List<Stop>();
                    busStops = await ParseLineStops(resultString, alsoCheckIncomingBuses, filterUnwantedLines, lineNumber, directionToCheck);

                    var line = new Line(lineNumber, lineDirection, busStops);

                    return line;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private static async Task<List<Stop>> ParseLineStops(string htmlTable, bool alsoCheckIncomingBuses, bool filterUnwantedLines, string lineNumberToFilter, string direction)
        {
            var stops = new List<Stop>();

            string zone = "";

            var splitBusLine = htmlTable.Split("bus-line");

            for (int i = 1; i < splitBusLine.Length; i++)
            {
                var splitTemp = splitBusLine[i].Split("paragem=");
                var splitName = splitTemp[1].Split('>');
                string stopName = splitName[1].Substring(0, splitName[1].IndexOf('<'));

                var splitStopId = splitTemp[2].Split('>');
                string stopId = splitStopId[1].Substring(0, splitStopId[1].IndexOf('<'));

                // Update if there's a new zone
                if (splitTemp[2].Contains("<span>"))
                {
                    var splitZone = splitTemp[2].Split("<span>");
                    zone = splitZone[1].Substring(0, splitZone[1].IndexOf('<'));
                }

                var buses = new List<IncomingBus>();

                if (alsoCheckIncomingBuses)
                {
                    // If requested, get next buses from all stops
                    var stopNextBuses = await StopClient.GetNextBuses(stopId);

                    // If requested, filter out only the current searched line from the result
                    if (filterUnwantedLines)
                    {
                        var toBeRemoved = new List<IncomingBus>();
                        foreach (var incomingBus in stopNextBuses.IncomingBuses)
                        {
                            if (incomingBus.LineNumber != lineNumberToFilter)
                            {
                                toBeRemoved.Add(incomingBus);
                            }
                            // Exception in line 505 stop LION1 where both directions stop at the same stop - Do a manual check
                            if ((lineNumberToFilter == "505") || (stopId == "LION1"))
                            {
                                if ((direction == "0") || (incomingBus.LineName == Line505StopLion1Direction1BusName))
                                {
                                    toBeRemoved.Add(incomingBus);
                                }
                                else if ((direction == "1") || (incomingBus.LineName == Line505StopLion1Direction0BusName))
                                {
                                    toBeRemoved.Add(incomingBus);
                                }
                            }
                        }
                        foreach (var bus in toBeRemoved)
                        {
                            stopNextBuses.IncomingBuses.Remove(bus);
                        }
                    }

                    buses = stopNextBuses.IncomingBuses;
                }

                var stop = new Stop(stopId, buses, stopName, zone);
                stops.Add(stop);
            }
            return stops;
        }
    }
}