using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Net.Http;
using System.Security.Cryptography;

namespace Sanakan.Web.Integration.Tests.AutomationTests
{
#if DEBUG
    [TestClass]
#endif
    public partial class TestBase
    {
        private static HttpClient _client = null;
        private static ChromeDriver _driver = null;
        private static SHA1 _sha = null;
        private static TestWebApplicationFactory<Startup> _factory = null;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            _sha = SHA1.Create();
            _factory = new TestWebApplicationFactory<Startup>(true);
            var options = new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri(@"https://localhost:5001"),
                AllowAutoRedirect = true,
            };

            _client = _factory.CreateClient(options);
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            chromeOptions.AddArguments("start-maximized");
            chromeOptions.AddArguments("ignore-certificate-errors");
            _driver = new ChromeDriver(chromeOptions);
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            if (_client != null)
            {
                _client.Dispose();
            }

            if (_driver != null)
            {
                _driver.Quit();
            }

            _sha.Dispose();
        }

        [TestMethod]
        public void Should_Display_Swagger_Page()
        {
            _driver.Navigate().GoToUrl(_factory.RootUri);
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            wait.Until(dr => ((IJavaScriptExecutor)dr).ExecuteScript("return document.readyState").Equals("complete"));
            new WebDriverWait(_driver, TimeSpan.FromSeconds(10)).Until(drv => !IsElementVisible(By.ClassName("loading-container")));
            var title = _driver.FindElement(By.ClassName("title")).Text.Should().Contain("Sanakan API");
            _driver.Title.Should().Be("Sanakan API");
        }

        private bool IsElementVisible(By searchElementBy)
        {
            try
            {
                return _driver.FindElement(searchElementBy).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}