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
        private const string FilterIncomingBusesKeyword = "filter";
        private const string FullBusesKeyword = "full";
        private const string NoIncomingBusesKeyword = "";

        private const string OutwardTripKeyword = "0";
        private const string ReturnTripKeyword = "1";

        public static async Task<Line> GetStopsFromLine(string lineNumber, string direction, string getIncomingBuses)
        {
            const string stcpEndpoint = "https://www.stcp.pt/pt/viajar/linhas/?linha=";
            const string stcpEndpoint2 = "&sentido=";
            const string stcpEndpoint3 = "&t=horarios";

            const string resultsFilter = "(//div[contains(@id,'bus-stop-results')])";

            // Check for these "wrong" line numbers
            switch (lineNumber)
            {
                case "700_V94":
                    lineNumber = "V94";
                    break;
                case "ZC":
                    lineNumber = "107";
                    break;
                case "ZR":
                    lineNumber = "103";
                    break;
                case "ZM":
                    lineNumber = "104";
                    break;
                case "ZF":
                    lineNumber = "106";
                    break;
                default:
                    break;
            }

            bool alsoCheckIncomingBuses = (getIncomingBuses.Equals(FullBusesKeyword) || getIncomingBuses.Equals(FilterIncomingBusesKeyword)) ? true : false;
            bool filterUnwantedLines = getIncomingBuses.Equals(FilterIncomingBusesKeyword) ? true : false;
            string directionToCheck = direction.Equals(ReturnTripKeyword) ? ReturnTripKeyword : OutwardTripKeyword;

            var co = new ChromeOptions();
            co.AddArgument("headless");
            co.AcceptInsecureCertificates = true;
            co.PageLoadStrategy = PageLoadStrategy.Normal;

            using (var driver = new ChromeDriver(co))
            {
                try
                {
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                    driver.Navigate().GoToUrl(stcpEndpoint + lineNumber + stcpEndpoint2 + directionToCheck + stcpEndpoint3);
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                    wait.Until(wt => !string.IsNullOrWhiteSpace(wt.FindElement(By.CssSelector("#bus-stop-results > table > tbody > tr:nth-child(1) > th.paragem")).Text));

                    var resultString = driver.FindElementByXPath(resultsFilter).GetAttribute("outerHTML");
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

        public static async Task<IEnumerable<Line>> GetAllLines(string getStops)
        {
            const string allLinesEndpoint = "https://www.stcp.pt/pt/viajar/linhas/";
            const string resultsFilter = "(//*[@id='viajar_linha'])";

            bool getAllStops = getStops.Equals("stops") ? true : false;

            var co = new ChromeOptions();
            co.AddArgument("headless");
            co.AcceptInsecureCertificates = true;
            co.PageLoadStrategy = PageLoadStrategy.Normal;

            using (var driver = new ChromeDriver(co))
            {
                try
                {
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                    driver.Navigate().GoToUrl(allLinesEndpoint);
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                    wait.Until(wt => !string.IsNullOrWhiteSpace(wt.FindElement(By.CssSelector("body > div.divGeralContent > div.divContentCenter > div.content-padding > table > tbody > tr > td > div > div.floatLeft > div.panel-linha-sentido > div.title")).Text));

                    var resultString = driver.FindElementByXPath(resultsFilter).GetAttribute("outerHTML");

                    driver.Close();

                    if (string.IsNullOrEmpty(resultString))
                    {
                        throw new HttpRequestException("Error reading page: No results found!");
                    }

                    var result = await ParseAllLines(resultString, getAllStops);

                    return result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private static async Task<List<Stop>> ParseLineStops(string htmlTable, bool alsoCheckIncomingBuses, bool filterUnwantedLines, string lineNumberToFilter, string direction)
        {
            const string line505StopLion1Direction0BusName = "MAT. MERCADO";
            const string line505StopLion1Direction1BusName = "H.S.JOÃO CIR";

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
                            if ((lineNumberToFilter == "505") && (stopId == "LION1"))
                            {
                                if ((direction == "0") && (incomingBus.Destination == line505StopLion1Direction1BusName))
                                {
                                    toBeRemoved.Add(incomingBus);
                                }
                                else if ((direction == "1") && (incomingBus.Destination == line505StopLion1Direction0BusName))
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

        private static async Task<List<Line>> ParseAllLines(string htmlTable, bool getStops)
        {
            var splitTemp = htmlTable.Split("value=");

            List<Line> lines = new List<Line>();

            for (int i = 1; i < splitTemp.Length; i++)
            {
                var splitNumber = splitTemp[i].Split('>');
                var lineNumber = splitNumber[1].Substring(0, splitNumber[1].IndexOf(' '));
                
                List<Stop> stops = new List<Stop>();
                string lineDirection = "";

                if (getStops)
                {
                    Line fullLine = await GetStopsFromLine(lineNumber, OutwardTripKeyword, NoIncomingBusesKeyword);
                    lineDirection = fullLine.LineDirection;
                    stops = fullLine.Stops;
                }
                else
                {
                    var splitName = splitTemp[i].Split("- ", 2);
                    lineDirection = splitName[1].Substring(0, splitName[1].IndexOf('<'));
                }

                Line line = new Line(lineNumber, lineDirection, stops);

                lines.Add(line);
            }

            return lines;
        }
    }
}