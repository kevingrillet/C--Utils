﻿using CSharp_Utils.D4Companion.Entities;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;

namespace CSharp_Utils.D4Companion;

/// <summary>
/// Represents a class that is responsible for scraping D4 Builds and exporting them to D4 Companion format.
/// </summary>
public class D4BuildsToD4CompanionScrapper
{
    protected static readonly bool Headless = true;
    private static readonly int SleepNavigate = 5000;
    private static readonly int SleepVariant = 1000;
    private WebDriver _driver;
    private WebDriverWait _driverWait;
    private string JsExportScript { get; } = File.ReadAllText("D4Companion/Ressources/D4BuildsScrapper.js");

    public D4BuildsExport Export()
    {
        var scriptResult = (string)_driver.ExecuteScript(JsExportScript);

        return JsonSerializer.Deserialize<D4BuildsExport>(scriptResult);
    }

    public IEnumerable<D4BuildsExport> ExportAll()
    {
        _ = _driver.ExecuteScript($"document.getElementsByClassName('item__arrow__icon--variant')[0].click()");
        var cnt = _driver.FindElements(By.ClassName("dropdown__option")).Count;
        _ = _driver.ExecuteScript($"document.getElementsByClassName('item__arrow__icon--variant')[0].click()");
        for (int i = 0; i < cnt; i++)
        {
            _ = _driver.ExecuteScript($"document.getElementsByClassName('item__arrow__icon--variant')[0].click()");
            _ = _driver.ExecuteScript($"document.querySelectorAll('.dropdown__option')[{i}].click()");
            Thread.Sleep(SleepVariant);
            yield return Export();
        }
    }

    public D4BuildsExport ExportVanilla()
    {
        // Huge speed boost
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(0);

        D4BuildsExport d4BuildExport = new()
        {
            // Name
            Name = GetName(),

            // Class
            D4Class = GetClass(),

            // Aspects
            Aspects = GetAllAspects(),

            // Armor
            Helm = GetAllAffixes("Helm"),
            ChestArmor = GetAllAffixes("ChestArmor"),
            Gloves = GetAllAffixes("Gloves"),
            Pants = GetAllAffixes("Pants"),
            Boots = GetAllAffixes("Boots"),

            // Accessories
            Amulet = GetAllAffixes("Amulet"),
            Ring1 = GetAllAffixes("Ring1"),
            Ring2 = GetAllAffixes("Ring2"),

            // Weapons
            Weapon = GetAllAffixes("Weapon"),
            Offhand = GetAllAffixes("Offhand"),
            RangedWeapon = GetAllAffixes("RangedWeapon"),
            BludgeoningWeapon = GetAllAffixes("BludgeoningWeapon"),
            SlashingWeapon = GetAllAffixes("SlashingWeapon"),
            WieldWeapon1 = GetAllAffixes("WieldWeapon1"),
            WieldWeapon2 = GetAllAffixes("WieldWeapon2"),
        };

        // Reset Timeout
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(10 * 1000);

        return d4BuildExport;
    }

    public IEnumerable<D4BuildsExport> ExportVanillaAll()
    {
        _ = _driver.ExecuteScript($"document.getElementsByClassName('item__arrow__icon--variant')[0].click()");
        var cnt = _driver.FindElements(By.ClassName("dropdown__option")).Count;
        _ = _driver.ExecuteScript($"document.getElementsByClassName('item__arrow__icon--variant')[0].click()");
        for (int i = 0; i < cnt; i++)
        {
            _ = _driver.ExecuteScript($"document.getElementsByClassName('item__arrow__icon--variant')[0].click()");
            _ = _driver.ExecuteScript($"document.querySelectorAll('.dropdown__option')[{i}].click()");
            Thread.Sleep(SleepVariant);
            yield return ExportVanilla();
        }
    }

    public void Navigate(string buildId)
    {
        _driver.Navigate().GoToUrl($@"https://d4builds.gg/builds/{buildId}/?var=0");
        _driverWait.Until(e => !string.IsNullOrEmpty(e.FindElement(By.Id("renameBuild")).GetAttribute("value")));
        Thread.Sleep(SleepNavigate);
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

    protected IEnumerable<string> GetAllAffixes(string itemType)
    {
        try
        {
            return _driver.FindElement(By.ClassName(itemType)).FindElements(By.ClassName("filled")).Select(e => e.GetAttribute("innerText")).ToList();
        }
        catch (NoSuchElementException)
        {
            return [];
        }
    }

    protected IEnumerable<string> GetAllAspects()
    {
        try
        {
            return _driver.FindElements(By.ClassName("builder__gear__name")).Select(e => e.GetAttribute("innerText")).Where(e => e.Contains("Aspect")).ToList();
        }
        catch (NoSuchElementException)
        {
            return [];
        }
    }

    protected string GetName()
    {
        try
        {
            var buildName = _driver.FindElement(By.Id("renameBuild"))?.GetAttribute("value") ?? "Unnamed Build";
            var variantName = _driver.FindElement(By.ClassName("builder__variant__input"))?.GetAttribute("value") ?? "Default Variant";
            return $"{buildName} - {variantName}";
        }
        catch (NoSuchElementException)
        {
            return string.Empty;
        }
    }

    private D4Class GetClass()
    {
        try
        {
            var cls = _driver.FindElement(By.ClassName("builder__header__description")).GetAttribute("innerText").Split(" ")[0];
            return (D4Class)Enum.Parse(typeof(D4Class), cls);
        }
        catch (NoSuchElementException)
        {
            return default;
        }
    }
}
