using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace DemoWebShopData12
{
    public class DemoWebShopData12Tests
    {
        private string baseUrl = "https://demowebshop.tricentis.com/";
        private static string? user1Email;
        private static string? user2Email;
        private static string password = "Test@12345";
        private string data1Path = "data1.txt";
        private string data2Path = "data2.txt";
        private ChromeDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            driver.Navigate().GoToUrl(baseUrl);
        }

        [TearDown]
        public void TearDown()
        {
            if (driver != null)
            {
                driver.Dispose(); 
                Console.WriteLine("WebDriver session closed.");
            }
        }

        [Test, Order(1)]
        public void CreateUser()
        {
            // Register User 1
            user1Email = "testuser1_" + DateTime.Now.Ticks + "@example.com";
            RegisterUser(user1Email);
            Console.WriteLine($"Created User 1: {user1Email}");

        }

        [Test, Order(2)]
        public void Data1()
        {
            user1Email = "testuser1_" + DateTime.Now.Ticks + "@example.com";
            RegisterUser(user1Email);
            Console.WriteLine($"Created User 1: {user1Email}");
            AddProductsToCart(data1Path);
            CheckoutAndConfirmOrder();
        }

        [Test, Order(3)]
        public void Data2()
        {
            PerformLogin(user1Email);
            AddProductsToCart(data2Path);
            CheckoutAndConfirmOrder();
        }

        private void RegisterUser(string email)
        {
            driver.FindElement(By.ClassName("ico-login")).Click();
            driver.FindElement(By.ClassName("register-button")).Click();

            driver.FindElement(By.Id("gender-male")).Click();
            driver.FindElement(By.Id("FirstName")).SendKeys("Test");
            driver.FindElement(By.Id("LastName")).SendKeys("User");
            driver.FindElement(By.Id("Email")).SendKeys(email);
            driver.FindElement(By.Id("Password")).SendKeys(password);
            driver.FindElement(By.Id("ConfirmPassword")).SendKeys(password);
            driver.FindElement(By.Id("register-button")).Click();
            driver.FindElement(By.ClassName("register-continue-button")).Click();
        }

        private void PerformLogin(string email)
        {
            driver.FindElement(By.ClassName("ico-login")).Click();
            driver.FindElement(By.Id("Email")).SendKeys(email);
            driver.FindElement(By.Id("Password")).SendKeys(password);
            driver.FindElement(By.ClassName("login-button")).Click();

            Assert.That(driver.FindElement(By.ClassName("ico-logout")).Displayed, Is.True, "Login failed!");
        }

        private void AddProductsToCart(string filePath)
        {
            string digitalDownloadsUrl = baseUrl + "digital-downloads";
            driver.Navigate().GoToUrl(digitalDownloadsUrl);

            List<string> products = File.ReadAllLines(filePath).ToList();

            foreach (string product in products)
            {
                try
                {
                    driver.Navigate().GoToUrl(digitalDownloadsUrl);

                    IWebElement addToCartButton = wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath($"//h2[@class='product-title']/a[text()='{product}']/../../..//input[@class='button-2 product-box-add-to-cart-button']")));

                    addToCartButton.Click();
                    wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("bar-notification")));

                    Console.WriteLine($"Added product: {product}");
                }
                catch (NoSuchElementException)
                {
                    Console.WriteLine($"Product '{product}' not found.");
                }
                catch (WebDriverException e)
                {
                    Console.WriteLine($"Error adding product '{product}': {e.Message}");
                }
            }
        }

        private void CheckoutAndConfirmOrder()
        {
            driver.FindElement(By.ClassName("ico-cart")).Click();
            SelectProductsInCart();

            wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("termsofservice"))).Click();
            driver.FindElement(By.Id("checkout")).Click();

            FillBillingAddress();
            wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("payment-method-next-step-button"))).Click();
            wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("payment-info-next-step-button"))).Click();
            wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("confirm-order-next-step-button"))).Click();

            string confirmationMessage = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[@class='section order-completed']//strong"))).Text;

            Assert.That(confirmationMessage.Contains("Your order has been successfully processed!"), Is.True, "Order was not placed successfully.");
            Console.WriteLine("Order successfully placed.");
        }

        private void SelectProductsInCart()
        {
            try
            {
                IReadOnlyCollection<IWebElement> productCheckboxes = driver.FindElements(By.XPath("//input[@type='checkbox' and contains(@name, 'removefromcart')]"));

                foreach (IWebElement checkbox in productCheckboxes)
                {
                    if (!checkbox.Selected)
                    {
                        checkbox.Click();
                    }
                }

                Console.WriteLine("All products in the shopping cart have been selected.");
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("No product selection checkboxes found in the shopping cart.");
            }
        }

        private void FillBillingAddress()
        {
            try
            {
                IWebElement addressDropdown = driver.FindElement(By.Id("billing-address-select"));

                if (addressDropdown.Displayed)
                {
                    SelectElement selectAddress = new SelectElement(addressDropdown);
                    selectAddress.SelectByIndex(0); 

                    Console.WriteLine("Existing billing address found. Clicking 'Continue'.");

                    IWebElement continueButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("new-address-next-step-button")));
                    continueButton.Click();
                    return; 
                }
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("No existing billing address dropdown found. Filling new address.");
            }

            IWebElement countryDropdown = driver.FindElement(By.Id("BillingNewAddress_CountryId"));
            SelectElement selectCountry = new SelectElement(countryDropdown);
            selectCountry.SelectByText("United States");

            driver.FindElement(By.Id("BillingNewAddress_City")).SendKeys("New York");
            driver.FindElement(By.Id("BillingNewAddress_Address1")).SendKeys("123 Main St");
            driver.FindElement(By.Id("BillingNewAddress_ZipPostalCode")).SendKeys("10001");
            driver.FindElement(By.Id("BillingNewAddress_PhoneNumber")).SendKeys("1234567890");

            IWebElement newAddressContinueButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("new-address-next-step-button")));
            newAddressContinueButton.Click();
        }

    }
}
