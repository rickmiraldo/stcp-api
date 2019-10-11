using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using STCP_API.Models.Entities;
using STCP_API.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace STCP_API.Models.Clients
{
    public static class LineClient
    {
        private const string stcpEndpoint = "https://www.stcp.pt/pt/viajar/linhas/?linha=";
        private const string stcpEndpoint2 = "&sentido=";
        private const string stcpEndpoint3 = "&t=horarios";

        private const string resultsFilter = "(//div[contains(@id,'bus-stop-results')])";

        public static async Task<Line> GetStopsFromLine(string lineNumber, string direction, string getIncomingBuses)
        {
            bool alsoCheckIncomingBuses = getIncomingBuses.Equals("full") ? true : false;
            string sentidoConsulta = direction.Equals("1") ? "1" : "0";
            
            var co = new ChromeOptions();
            co.AddArgument("headless");
            co.AcceptInsecureCertificates = true;
            co.PageLoadStrategy = PageLoadStrategy.Normal;

            using (var driver = new ChromeDriver(co))
            {
                try
                {
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                    driver.Navigate().GoToUrl(stcpEndpoint + lineNumber + stcpEndpoint2 + sentidoConsulta + stcpEndpoint3);
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
                    busStops = await ParseLineStops(resultString, alsoCheckIncomingBuses);

                    var line = new Line(lineNumber, lineDirection, busStops);

                    return line;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private static async Task<List<Stop>> ParseLineStops(string htmlTable, bool alsoCheckIncomingBuses = false)
        {
            var stops = new List<Stop>();

            // Get first zone
            var splitFirstZone = htmlTable.Split("<span>");
            string zone = splitFirstZone[1].Substring(0, splitFirstZone[1].IndexOf('<'));

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

                // TO-DO Add boolean to check for incoming buses if needed
                var buses = new List<IncomingBus>();

                if (alsoCheckIncomingBuses)
                {
                    var stopNextBuses = await StopClient.GetNextBuses(stopId);
                    buses = stopNextBuses.IncomingBuses;
                }

                var stop = new Stop(stopId, buses, stopName, zone);
                stops.Add(stop);
            }
            return stops;
        }
    }
}