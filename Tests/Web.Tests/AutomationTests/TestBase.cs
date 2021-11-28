using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Sanakan.Api.Models;
using Sanakan.Common.Converters;
using Sanakan.DAL;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Web.Tests.IntegrationTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.AutomationTests
{
    [TestClass]
    public partial class TestBase
    {
        protected static HttpClient _client;

        [ClassInitialize]
        public static async Task Setup(TestContext context)
        {
            var factory = new TestWebApplicationFactory();
            _client = factory.CreateClient();
        }

        [ClassCleanup]
        public static async Task Cleanup()
        {
            _client.Dispose();
        }

        [TestMethod]
        public async Task Should_Display_Swagger_Page()
        {
            var driver = new ChromeDriver();
            driver.Url = _client.BaseAddress.ToString() + "index.html";
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            wait.Until(dr => ((IJavaScriptExecutor)dr).ExecuteScript("return document.readyState").Equals("complete"));
            var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            screenshot.SaveAsFile(@"screenshot.jpg", ScreenshotImageFormat.Jpeg);
        }
    }
}