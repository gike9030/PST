using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using System;
using OpenQA.Selenium.Support.UI;

namespace DemoqaTests
{
    [TestFixture]
    public class ProgressBarTests
    {
        private ChromeDriver driver;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://web.archive.org/web/20250112093337/http://demoqa.com/");
        }

        [TearDown]
        public void Teardown()
        {
            driver.Dispose();
        }

        [Test]
        public void ProgressBar()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            CloseBannerIfPresent();
            ClickElement(By.XPath("//h5[text()='Widgets']"));
            ClickElement(By.XPath("//span[text()='Progress Bar']"));
            ClickElement(By.Id("startStopButton"));

            wait.Until(d => d.FindElement(By.CssSelector(".progress-bar"))
                .GetAttribute("aria-valuenow") == "100");

            ClickElement(By.Id("resetButton"));
            string progressValue = driver.FindElement(By.CssSelector(".progress-bar"))
                .GetAttribute("aria-valuenow");

            Assert.That(progressValue, Is.EqualTo("0"), "Progress bar did not reset to 0%.");
        }

        private void ClickElement(By locator)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            CloseBannerIfPresent();

            var element = wait.Until(d =>
            {
                var elem = d.FindElement(locator);
                return (elem.Displayed && elem.Enabled) ? elem : null;
            });

            ScrollIntoView(element);

            try
            {
                element.Click();
            }
            catch (ElementClickInterceptedException)
            {
                CloseBannerIfPresent();
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
            }
        }

        private void ScrollIntoView(IWebElement element)
        {
            ((IJavaScriptExecutor)driver)
                .ExecuteScript("arguments[0].scrollIntoView(true);", element);
        }

        private void CloseBannerIfPresent()
        {
            try
            {
                var banner = driver.FindElement(By.Id("fixedban"));
                if (banner.Displayed)
                {
                    ((IJavaScriptExecutor)driver)
                        .ExecuteScript("arguments[0].style.display='none';", banner);
                }
            }
            catch (NoSuchElementException)
            {
                
            }
        }
    }
}
