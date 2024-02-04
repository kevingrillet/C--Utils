using CSharp_Utils.Tests.Entities.D4Companion;
using CSharp_Utils.Tests.Entities;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CSharp_Utils.Helpers;
using OpenQA.Selenium.Chrome;
using System.Text.Json;
using FuzzySharp;

namespace CSharp_Utils.Tests.Experiments
{
    [TestFixture, Category("NotOnGitHub")]
    internal class D4BuildsScrapperToD4CompanionTests
    {
        private List<AffixInfo> _affixInfos;
        private AffixPreset _affixPreset;
        private List<AspectInfo> _aspectInfos;
        private D4BuildsExport _d4BuildExport;
        private WebDriver _driver;
        protected virtual bool Headless { get; set; } = true;

        [OneTimeSetUp]
        public void AA_OneTimeSetUp()
        {
            // From: https://github.com/josdemmers/Diablo4Companion
            _affixInfos = JsonHelpers<List<AffixInfo>>.Load("Ressources/D4Companion/Affixes.enUS.json") ?? [];
            _aspectInfos = JsonHelpers<List<AspectInfo>>.Load("Ressources/D4Companion/Aspects.enUS.json") ?? [];

            _affixPreset = new();

            // Create Driver
            AA_CreateDriver();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(10 * 1000);
        }

        [OneTimeTearDown]
        public void AA_TearDown()
        {
            // Close & Destroy driver
            _driver?.Close();
            _driver?.Dispose();
            _driver?.Quit();
        }

        [Test, Ignore("Just to slow between 5s and 2 min for no reason...")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2925:\"Thread.Sleep\" should not be used in tests", Justification = "<En attente>")]
        public void Test_10_GetPageSelenium()
        {
            D4BuildsExport d4BuildExport = new();
            _driver.Navigate().GoToUrl("https://d4builds.gg/builds/660881f7-cb6a-4162-be62-29f0afeb37bf/");
            Thread.Sleep(2500);

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

            Assert.Multiple(() =>
            {
                Assert.That(d4BuildExport.Name, Is.Not.Empty);
                Assert.That(d4BuildExport.D4Class, Is.EqualTo(D4Class.Necromancer));
                Assert.That(d4BuildExport.Aspects, Is.Not.Empty);
                Assert.That(d4BuildExport.Helm, Is.Empty);
                Assert.That(d4BuildExport.ChestArmor, Is.Not.Empty);
                Assert.That(d4BuildExport.Gloves, Is.Not.Empty);
                Assert.That(d4BuildExport.Pants, Is.Not.Empty);
                Assert.That(d4BuildExport.Boots, Is.Not.Empty);
                Assert.That(d4BuildExport.Amulet, Is.Not.Empty);
                Assert.That(d4BuildExport.Ring1, Is.Not.Empty);
                Assert.That(d4BuildExport.Ring2, Is.Not.Empty);
                Assert.That(d4BuildExport.Weapon, Is.Not.Empty);
                Assert.That(d4BuildExport.Offhand, Is.Empty);
                Assert.That(d4BuildExport.RangedWeapon, Is.Empty);
                Assert.That(d4BuildExport.BludgeoningWeapon, Is.Empty);
                Assert.That(d4BuildExport.SlashingWeapon, Is.Empty);
                Assert.That(d4BuildExport.WieldWeapon1, Is.Empty);
                Assert.That(d4BuildExport.WieldWeapon2, Is.Empty);
            });
        }

        [Test]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2925:\"Thread.Sleep\" should not be used in tests", Justification = "<En attente>")]
        public void Test_11_GetPageSeleniumJS()
        {
            _driver.Navigate().GoToUrl("https://d4builds.gg/builds/660881f7-cb6a-4162-be62-29f0afeb37bf/");
            Thread.Sleep(2500);

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

            Assert.That(res, Is.Not.Null);
            _d4BuildExport = JsonSerializer.Deserialize<D4BuildsExport>(res);

            Assert.Multiple(() =>
            {
                Assert.That(_d4BuildExport.Name, Is.Not.Empty);
                Assert.That(_d4BuildExport.D4Class, Is.EqualTo(D4Class.Necromancer));
                Assert.That(_d4BuildExport.Aspects, Is.Not.Empty);
                Assert.That(_d4BuildExport.Helm, Is.Empty);
                Assert.That(_d4BuildExport.ChestArmor, Is.Not.Empty);
                Assert.That(_d4BuildExport.Gloves, Is.Not.Empty);
                Assert.That(_d4BuildExport.Pants, Is.Not.Empty);
                Assert.That(_d4BuildExport.Boots, Is.Not.Empty);
                Assert.That(_d4BuildExport.Amulet, Is.Not.Empty);
                Assert.That(_d4BuildExport.Ring1, Is.Not.Empty);
                Assert.That(_d4BuildExport.Ring2, Is.Not.Empty);
                Assert.That(_d4BuildExport.Weapon, Is.Not.Empty);
                Assert.That(_d4BuildExport.Offhand, Is.Empty);
                Assert.That(_d4BuildExport.RangedWeapon, Is.Empty);
                Assert.That(_d4BuildExport.BludgeoningWeapon, Is.Empty);
                Assert.That(_d4BuildExport.SlashingWeapon, Is.Empty);
                Assert.That(_d4BuildExport.WieldWeapon1, Is.Empty);
                Assert.That(_d4BuildExport.WieldWeapon2, Is.Empty);
            });
        }

        [Test]
        public void Test_20_Base()
        {
            _affixPreset.Name = _d4BuildExport.Name;

            Assert.That(_affixPreset.Name, Is.EqualTo(_d4BuildExport.Name));
        }

        [Test]
        public void Test_21_Affixes()
        {
            BuildAffixes(_d4BuildExport.Helm, "helm");
            BuildAffixes(_d4BuildExport.ChestArmor, "chest");
            BuildAffixes(_d4BuildExport.Gloves, "gloves");
            BuildAffixes(_d4BuildExport.Pants, "pants");
            BuildAffixes(_d4BuildExport.Boots, "boots");
            BuildAffixes(_d4BuildExport.Amulet, "amulet");
            BuildAffixes(_d4BuildExport.Rings, "ring");
            BuildAffixes(_d4BuildExport.Weapons, "weapon");
            BuildAffixes(_d4BuildExport.Offhand, "offhand");
            BuildAffixes(_d4BuildExport.RangedWeapon, "ranged");

            Assert.Multiple(() =>
            {
                Assert.That(_affixPreset.ItemAffixes.Count(i => i.Type == "helm"), Is.EqualTo(_d4BuildExport.Helm.Count()));
                Assert.That(_affixPreset.ItemAffixes.Count(i => i.Type == "chest"), Is.EqualTo(_d4BuildExport.ChestArmor.Count()));
                Assert.That(_affixPreset.ItemAffixes.Count(i => i.Type == "gloves"), Is.EqualTo(_d4BuildExport.Gloves.Count()));
                Assert.That(_affixPreset.ItemAffixes.Count(i => i.Type == "pants"), Is.EqualTo(_d4BuildExport.Pants.Count()));
                Assert.That(_affixPreset.ItemAffixes.Count(i => i.Type == "boots"), Is.EqualTo(_d4BuildExport.Boots.Count()));
                Assert.That(_affixPreset.ItemAffixes.Count(i => i.Type == "amulet"), Is.EqualTo(_d4BuildExport.Amulet.Count()));
                Assert.That(_affixPreset.ItemAffixes.Count(i => i.Type == "ring"), Is.EqualTo(_d4BuildExport.Rings.Count()));
                Assert.That(_affixPreset.ItemAffixes.Count(i => i.Type == "weapon"), Is.EqualTo(_d4BuildExport.Weapons.Count()));
                Assert.That(_affixPreset.ItemAffixes.Count(i => i.Type == "ranged"), Is.EqualTo(_d4BuildExport.RangedWeapon.Count()));
                Assert.That(_affixPreset.ItemAffixes.Count(i => i.Type == "offhand"), Is.EqualTo(_d4BuildExport.Offhand.Count()));
            });
        }

        [Test]
        public void Test_22_Aspects()
        {
            BuildAspects(_d4BuildExport.Aspects);

            Assert.That(_affixPreset.ItemAspects, Has.Count.EqualTo(_d4BuildExport.Aspects.Count()));
        }

        [Test]
        public void Test_30_Save()
        {
            JsonHelpers<AffixPreset>.Save("Ressources/d4buildsscrapper_export.json", _affixPreset, new JsonSerializerOptions() { WriteIndented = true });
            Assert.Pass();
        }

        private void AA_CreateDriver()
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

        private void BuildAffixes(IEnumerable<string> affixes, string type)
        {
            foreach (var affix in affixes)
            {
                _affixPreset.ItemAffixes.Add(new ItemAffix()
                {
                    Id = _affixInfos.Find(a => a.Description == Process.ExtractOne(affix, _affixInfos.Where(aa => aa.AllowedForPlayerClass[(int)_d4BuildExport.D4Class] == 1).Select(aa => aa.Description)).Value).IdName,
                    Type = type
                });
            }
        }

        private void BuildAspects(IEnumerable<string> aspects, string type = "aspect")
        {
            foreach (var aspect in aspects)
            {
                _affixPreset.ItemAspects.Add(new ItemAffix()
                {
                    Id = _aspectInfos.Find(a => a.Name == Process.ExtractOne(aspect, _aspectInfos.Where(aa => aa.AllowedForPlayerClass[(int)_d4BuildExport.D4Class] == 1).Select(a => a.Name)).Value).IdName,
                    Type = type
                });
            }
        }
    }
}
