using CSharp_Utils.Entities;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
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

        public D4BuildsExport Export()
        {
            var res = (string)_driver.ExecuteScript("""
                function getAllAffixes(category) {
                    var res = [];
                    document.querySelectorAll(`:scope .${category} .filled`).forEach((e) => res.push(e.innerText));
                    return res;
                }

                function getAllAspects() {
                    var res = [];
                    document.querySelectorAll(`:scope .builder__gear__name`).forEach((e) => res.push(e.innerText));
                    res = res.filter(function (e) {
                        return e.includes('Aspect');
                    });
                    return res;
                }

                function getClass() {
                    switch (document.querySelector('.builder__header__description').lastChild.textContent) {
                        case 'Sorcerer':
                            return 0;

                        case 'Druid':
                            return 1;

                        case 'Barbarian':
                            return 2;

                        case 'Rogue':
                            return 3;

                        case 'Necromancer':
                            return 4;

                        default:
                            return null;
                    }
                }

                var result = {};
                result.Name = document.querySelector('#renameBuild').value;
                result.D4Class = getClass();

                result.Aspects = getAllAspects();

                result.Helm = getAllAffixes('Helm');
                result.ChestArmor = getAllAffixes('ChestArmor');
                result.Gloves = getAllAffixes('Gloves');
                result.Pants = getAllAffixes('Pants');
                result.Boots = getAllAffixes('Boots');
                result.Amulet = getAllAffixes('Amulet');
                result.Ring1 = getAllAffixes('Ring1');
                result.Ring2 = getAllAffixes('Ring2');

                if (document.querySelector('.Weapon')) result.Weapon = getAllAffixes('Weapon');
                if (document.querySelector('.Offhand')) result.Offhand = getAllAffixes('Offhand');
                if (document.querySelector('.RangedWeapon')) result.Weapon = getAllAffixes('RangedWeapon');
                if (document.querySelector('.BludgeoningWeapon')) result.BludgeoningWeapon = getAllAffixes('BludgeoningWeapon');
                if (document.querySelector('.SlashingWeapon')) result.SlashingWeapon = getAllAffixes('SlashingWeapon');
                if (document.querySelector('.WieldWeapon1')) result.WieldWeapon1 = getAllAffixes('WieldWeapon1');
                if (document.querySelector('.WieldWeapon2')) result.WieldWeapon2 = getAllAffixes('WieldWeapon2');

                return JSON.stringify(result, null, 2);
                """);

            return JsonSerializer.Deserialize<D4BuildsExport>(res);
        }

        public D4BuildsExport ExportVanilla()
        {
            D4BuildsExport d4BuildExport = new();

            // Name
            d4BuildExport.Name = _driver.FindElement(By.Id("renameBuild")).GetAttribute("value");

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
            if (_driver.FindElements(By.ClassName("Weapon")).Count > 0)
                d4BuildExport.Weapon = _driver.FindElement(By.ClassName("Weapon")).FindElements(By.ClassName("filled")).Select(e => e.Text).ToList();
            if (_driver.FindElements(By.ClassName("Offhand")).Count > 0)
                d4BuildExport.Weapon = _driver.FindElement(By.ClassName("Offhand")).FindElements(By.ClassName("filled")).Select(e => e.Text).ToList();
            if (_driver.FindElements(By.ClassName("RangedWeapon")).Count > 0)
                d4BuildExport.Weapon = _driver.FindElement(By.ClassName("RangedWeapon")).FindElements(By.ClassName("filled")).Select(e => e.Text).ToList();
            if (_driver.FindElements(By.ClassName("BludgeoningWeapon")).Count > 0)
                d4BuildExport.Weapon = _driver.FindElement(By.ClassName("BludgeoningWeapon")).FindElements(By.ClassName("filled")).Select(e => e.Text).ToList();
            if (_driver.FindElements(By.ClassName("SlashingWeapon")).Count > 0)
                d4BuildExport.Weapon = _driver.FindElement(By.ClassName("SlashingWeapon")).FindElements(By.ClassName("filled")).Select(e => e.Text).ToList();
            if (_driver.FindElements(By.ClassName("WieldWeapon1")).Count > 0)
                d4BuildExport.Weapon = _driver.FindElement(By.ClassName("WieldWeapon1")).FindElements(By.ClassName("filled")).Select(e => e.Text).ToList();
            if (_driver.FindElements(By.ClassName("WieldWeapon2")).Count > 0)
                d4BuildExport.Weapon = _driver.FindElement(By.ClassName("WieldWeapon2")).FindElements(By.ClassName("filled")).Select(e => e.Text).ToList();

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
    }
}
