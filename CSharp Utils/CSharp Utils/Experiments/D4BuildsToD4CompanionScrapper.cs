using CSharp_Utils.Entities;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;

namespace CSharp_Utils.Experiments
{
    public class D4BuildsToD4CompanionScrapper
    {
        private WebDriver _driver;
        private WebDriverWait _driverWait;
        protected virtual bool Headless { get; set; } = true;
        private string JsExportScript { get; } = File.ReadAllText("Ressources/D4BuildsScrapper.js");

        public D4BuildsExport Export()
        {
            var scriptResult = (string)_driver.ExecuteScript(JsExportScript);

            return JsonSerializer.Deserialize<D4BuildsExport>(scriptResult);
        }

        public IEnumerable<D4BuildsExport> ExportAll()
        {
            var cnt = _driver.FindElements(By.ClassName("variant__button")).Count;
            for (int i = 0; i < cnt; i++)
            {
                _ = _driver.ExecuteScript($"document.querySelectorAll('.variant__button')[{i}].click()");
                Thread.Sleep(50);
                var scriptResult = (string)_driver.ExecuteScript(JsExportScript);
                yield return JsonSerializer.Deserialize<D4BuildsExport>(scriptResult);
            }
        }

        public D4BuildsExport ExportVanilla()
        {
            // Huge speed boost
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(0);

            D4BuildsExport d4BuildExport = new();

            // Name
            d4BuildExport.Name = $"{_driver.FindElement(By.Id("renameBuild")).GetAttribute("value")} - {_driver.FindElement(By.CssSelector(".variant__button.active>:first-child>:first-child")).GetAttribute("value")}";

            // Class
            d4BuildExport.D4Class = (D4Class)Enum.Parse(typeof(D4Class), _driver.FindElement(By.ClassName("builder__header__description")).Text.Split(" ")[^1]);

            // Aspects
            d4BuildExport.Aspects = _driver.FindElements(By.ClassName("builder__gear__name")).Select(e => e.Text).Where(e => e.Contains("Aspect")).ToList();

            // Armor
            d4BuildExport.Helm = _driver.FindElement(By.ClassName("Helm")).FindElements(By.ClassName("filled")).Select(e => e.Text).ToList();
            d4BuildExport.ChestArmor = _driver.FindElement(By.ClassName("ChestArmor")).FindElements(By.ClassName("filled")).Select(e => e.Text).ToList();
            d4BuildExport.Gloves = _driver.FindElement(By.ClassName("Gloves")).FindElements(By.ClassName("filled")).Select(e => e.Text).ToList();
            d4BuildExport.Pants = _driver.FindElement(By.ClassName("Pants")).FindElements(By.ClassName("filled")).Select(e => e.Text).ToList();
            d4BuildExport.Boots = _driver.FindElement(By.ClassName("Boots")).FindElements(By.ClassName("filled")).Select(e => e.Text).ToList();

            // Accessories
            d4BuildExport.Amulet = _driver.FindElement(By.ClassName("Amulet")).FindElements(By.ClassName("filled")).Select(e => e.Text).ToList();
            d4BuildExport.Ring1 = _driver.FindElement(By.ClassName("Ring1")).FindElements(By.ClassName("filled")).Select(e => e.Text).ToList();
            d4BuildExport.Ring1 = _driver.FindElement(By.ClassName("Ring2")).FindElements(By.ClassName("filled")).Select(e => e.Text).ToList();

            // Weapons
            if (IsElementPresent(By.ClassName("Weapon")))
                d4BuildExport.Weapon = _driver.FindElement(By.ClassName("Weapon")).FindElements(By.ClassName("filled")).Select(e => e.Text).ToList();
            if (IsElementPresent(By.ClassName("Offhand")))
                d4BuildExport.Weapon = _driver.FindElement(By.ClassName("Offhand")).FindElements(By.ClassName("filled")).Select(e => e.Text).ToList();
            if (IsElementPresent(By.ClassName("RangedWeapon")))
                d4BuildExport.Weapon = _driver.FindElement(By.ClassName("RangedWeapon")).FindElements(By.ClassName("filled")).Select(e => e.Text).ToList();
            if (IsElementPresent(By.ClassName("BludgeoningWeapon")))
                d4BuildExport.Weapon = _driver.FindElement(By.ClassName("BludgeoningWeapon")).FindElements(By.ClassName("filled")).Select(e => e.Text).ToList();
            if (IsElementPresent(By.ClassName("SlashingWeapon")))
                d4BuildExport.Weapon = _driver.FindElement(By.ClassName("SlashingWeapon")).FindElements(By.ClassName("filled")).Select(e => e.Text).ToList();
            if (IsElementPresent(By.ClassName("WieldWeapon1")))
                d4BuildExport.Weapon = _driver.FindElement(By.ClassName("WieldWeapon1")).FindElements(By.ClassName("filled")).Select(e => e.Text).ToList();
            if (IsElementPresent(By.ClassName("WieldWeapon2")))
                d4BuildExport.Weapon = _driver.FindElement(By.ClassName("WieldWeapon2")).FindElements(By.ClassName("filled")).Select(e => e.Text).ToList();

            // Reset Timeout
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(10 * 1000);

            return d4BuildExport;
        }

        public void Navigate(string buildId)
        {
            _driver.Navigate().GoToUrl($@"https://d4builds.gg/builds/{buildId}/");
            _driverWait.Until(e => !string.IsNullOrEmpty(e.FindElement(By.Id("renameBuild")).GetAttribute("value")));
            Thread.Sleep(250);
        }

        public void Start()
        {
            CreateDriver();
            _driverWait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(10 * 1000);
        }

        public void Stop()
        {
            // Close & Destroy driver
            _driver?.Close();
            _driver?.Dispose();
            _driver?.Quit();
        }

        protected void CreateDriver()
        {
            // Options: Headless, size, security, ...
            var options = new ChromeOptions();
            if (Headless)
            {
                options.AddArgument("--headless");
                options.AddArgument("--disable-gpu"); // Applicable to windows os only
            }
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-popup-blocking");
            options.AddArgument("--disable-notifications");
            options.AddArgument("--dns-prefetch-disable");
            options.AddArgument("--disable-dev-shm-usage"); // Overcome limited resource problems
            options.AddArgument("--no-sandbox"); // Bypass OS security model
            options.AddArgument("--window-size=1600,900");

            // Create driver
            _driver = new ChromeDriver(options: options);
        }

        protected bool IsElementPresent(By by)
        {
            try
            {
                _driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}
