using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;

namespace DemoWebShop
{
    public class DemoWebShopTests : IDisposable
    {
        private ChromeDriver driver;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
        }


        [Test]
        public void TestShoppingProcess()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // 1. Open the website
            driver.Navigate().GoToUrl("https://demowebshop.tricentis.com/");

            // 2. Click on 'Gift Cards' in the left menu
            driver.FindElement(By.LinkText("Gift Cards")).Click();

            // 3. Select a product with a price > 99 using XPath
            var selectedGiftCard = wait.Until(driver => driver.FindElement(By.XPath(
               "//div[@class='product-item'][.//span[@class='price actual-price'][. > 99]]\r\n"
            )));
            selectedGiftCard.FindElement(By.CssSelector("h2.product-title a")).Click();

            // 4.1. Fill in the 'Recipient's Name' field
            var recipientNameField = wait.Until(driver => driver.FindElement(By.Id("giftcard_4_RecipientName"))); 
            recipientNameField.SendKeys("Jonas Jonaitis");

            // 4.2. Fill in the 'Your Name' field
            var yourNameField = wait.Until(driver => driver.FindElement(By.Id("giftcard_4_SenderName"))); 
            yourNameField.SendKeys("Vardenis Pavardenis");

            // 5. Enter '5000' in the 'Qty' text field
            var qtyField = wait.Until(driver => driver.FindElement(By.Id("addtocart_4_EnteredQuantity"))); 
            qtyField.Clear();
            qtyField.SendKeys("5000");


            // 6. Click 'Add to cart'
            driver.FindElement(By.Id("add-to-cart-button-4")).Click();
            SuccessNotification();

            // 7. Click 'Add to wish list'
            var addToWishlistButton = wait.Until(driver => driver.FindElement(By.CssSelector(".add-to-wishlist-button")));
            addToWishlistButton.Click();

            SuccessNotification();

            // 8. Click on 'Jewelry'
            driver.FindElement(By.LinkText("Jewelry")).Click();

            // 9. Click 'Create Your Own Jewelry' link
            driver.FindElement(By.LinkText("Create Your Own Jewelry")).Click();

            // 10.1. Select "Material" as "Silver (1 mm)"
            var materialDropdown = wait.Until(driver => driver.FindElement(By.Id("product_attribute_71_9_15")));
            var materialSelect = new SelectElement(materialDropdown);
            materialSelect.SelectByValue("47"); 

            // 10.2. Enter "80" in the "Length in cm" field
            var lengthField = wait.Until(driver => driver.FindElement(By.Id("product_attribute_71_10_16")));
            lengthField.Clear();
            lengthField.SendKeys("80");

            // 10.3. Select "Star" as the pendant
            var pendantOption = wait.Until(driver => driver.FindElement(By.Id("product_attribute_71_11_17_50")));
            pendantOption.Click();

            // 11. Enter '26' in the 'Qty' text field
            var jewelryQtyField = driver.FindElement(By.Id("addtocart_71_EnteredQuantity"));
            jewelryQtyField.Clear();
            jewelryQtyField.SendKeys("26");

            // 12. Click 'Add to cart'
            driver.FindElement(By.Id("add-to-cart-button-71")).Click();
            SuccessNotification();

            // 13. Click 'Add to wish list'
            driver.FindElement(By.CssSelector(".add-to-wishlist-button")).Click();
            SuccessNotification();

            // 14. Click 'Wishlist' at the top
            driver.FindElement(By.LinkText("Wishlist")).Click();

            // 15. Check 'Add to cart' checkboxes
            var checkboxes = driver.FindElements(By.CssSelector(".add-to-cart input"));
            foreach (var checkbox in checkboxes)
            {
                if (!checkbox.Selected) checkbox.Click();
            }

            driver.FindElement(By.Name("addtocartbutton")).Click();

            var subTotalValueElement = wait.Until(driver =>
            driver.FindElement(By.CssSelector(".cart-total-left + td span"))
            );

            string subTotalValue = subTotalValueElement.Text.Trim();
            Assert.That(subTotalValue, Is.EqualTo("1002600.00"), "Sub-Total price does not match expected value.");

            TearDown();
        }
        public void SuccessNotification()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var successNotification = wait.Until(driver =>
            driver.FindElement(By.CssSelector(".bar-notification.success"))
            );
        }
        [TearDown]
        public void TearDown()
        {
            Dispose();
        }

        public void Dispose()
        {
            driver?.Dispose();
        }
    }
}
