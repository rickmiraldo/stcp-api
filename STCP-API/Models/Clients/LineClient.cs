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

        private const string resultsFilter = "(//div[contains(@id,'bus-stop-results')])";

        public static string GetStopsFromLine(string lineNumber)
            // TO-DO Change return type to Line
        {
            var co = new ChromeOptions();
            co.AddArgument("headless");
            co.AcceptInsecureCertificates = true;
            co.PageLoadStrategy = PageLoadStrategy.Normal;

            using (var driver = new ChromeDriver(co))
            {
                try
                {
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                    driver.Navigate().GoToUrl(stcpEndpoint + lineNumber);
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                    wait.Until(wt => !string.IsNullOrWhiteSpace(wt.FindElement(By.CssSelector("#bus-stop-results > table > tbody > tr:nth-child(1) > th.paragem")).Text));

                    var resultString = driver.FindElementByXPath(resultsFilter).GetAttribute("outerHTML");

                    driver.Close();

                    if (resultString == null)
                    {
                        throw new HttpRequestException("Error reading page: No results found!");
                    }




                    return resultString;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}