using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using System;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using SeleniumExtras.WaitHelpers;

namespace DemoqaTests
{
    [TestFixture]
    public class WebTablesTests
    {
        private ChromeDriver driver;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://web.archive.org/web/20241224192818/https://demoqa.com/");
        }

        [TearDown]
        public void Teardown()
        {
            driver.Dispose();
        }

        [Test]
        public void WebTables()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            CloseBannerIfPresent();
            ClickElement(By.XPath("//h5[text()='Elements']"));
            ClickElement(By.XPath("//span[text()='Web Tables']"));

            for (int i = 0; i < 8; i++)
            {
                ClickElement(By.Id("addNewRecordButton"));
                driver.FindElement(By.Id("firstName")).SendKeys("TestFirst" + i);
                driver.FindElement(By.Id("lastName")).SendKeys("TestLast" + i);
                driver.FindElement(By.Id("userEmail")).SendKeys($"test{i}@example.com");
                driver.FindElement(By.Id("age")).SendKeys("20");
                driver.FindElement(By.Id("salary")).SendKeys("4444");
                driver.FindElement(By.Id("department")).SendKeys("IT");
                ClickElement(By.Id("submit"));

                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("firstName")));
            }

            ClickElement(By.CssSelector(".-next"));
            ClickElement(By.CssSelector(".rt-tr-group:nth-child(1) span[title='Delete']"));

            Assert.That(
                wait.Until(d => d.FindElement(By.CssSelector("input[type='number']")).GetAttribute("value")),
                Is.EqualTo("1"),
                "The input value is not equal to 1."
            );

            //wait.Until(d => d.FindElement(By.ClassName("-totalPages")).Text == "1");
        }

        private void ClickElement(By locator)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            CloseBannerIfPresent();

            var element = wait.Until(ExpectedConditions.ElementToBeClickable(locator));

            ScrollIntoView(element);

            try
            {
                element.Click();
            }
            catch
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
            }
        }

        private void ScrollIntoView(IWebElement element)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
        }

        private void CloseBannerIfPresent()
        {
            try
            {
                var banner = driver.FindElement(By.Id("fixedban"));
                if (banner.Displayed)
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].style.display='none';", banner);
                }
            }
            catch (NoSuchElementException)
            {
            }
        }
    }
}
