using CSharp_Utils.D4Companion;
using CSharp_Utils.D4Companion.Entities;
using CSharp_Utils.D4Companion.Entities.D4Companion;
using CSharp_Utils.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CSharp_Utils.Tests.D4Companion;

[TestFixture, Parallelizable, Category("NotOnGitHub")]
internal class D4BuildsToD4CompanionScrapperTests
{
    private D4BuildsToD4CompanionConverter _converter;
    private D4BuildsToD4CompanionScrapper _scrapper;
    protected virtual bool Headless { get; set; } = true;

    [OneTimeSetUp]
    public void AA_OneTimeSetUp()
    {
        _converter = new();
        _scrapper = new();
        _scrapper.Start();
    }

    [OneTimeTearDown]
    public void AA_TearDown()
    {
        _scrapper.Stop();
    }

    [Test]
    public void Test_0_Init()
    {
        Assert.Multiple(() =>
        {
            Assert.That(_converter, Is.Not.Null);
            Assert.That(_scrapper, Is.Not.Null);
        });
    }

    [Test]
    public void Test_10_Navigate()
    {
        Assert.DoesNotThrow(() => { _scrapper.Navigate("660881f7-cb6a-4162-be62-29f0afeb37bf"); });
    }

    [Test]
    public void Test_11_Export()
    {
        var d4BuildExport = _scrapper.Export();
        var affixPreset = _converter.Convert(d4BuildExport);
        JsonHelpers<AffixPreset>.Save("D4Companion/Ressources/d4buildsscrapper_export.json", affixPreset, new JsonSerializerOptions() { WriteIndented = true });

        Assert.Multiple(() =>
        {
            // d4BuildExport
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

            // affixPreset
            Assert.That(affixPreset.Name, Is.EqualTo(d4BuildExport.Name));
            Assert.That(affixPreset.ItemAspects, Has.Count.EqualTo(d4BuildExport.Aspects.Count()));
            Assert.That(affixPreset.ItemAffixes.Count(i => i.Type == "helm"), Is.EqualTo(d4BuildExport.Helm.Count()));
            Assert.That(affixPreset.ItemAffixes.Count(i => i.Type == "chest"), Is.EqualTo(d4BuildExport.ChestArmor.Count()));
            Assert.That(affixPreset.ItemAffixes.Count(i => i.Type == "gloves"), Is.EqualTo(d4BuildExport.Gloves.Count()));
            Assert.That(affixPreset.ItemAffixes.Count(i => i.Type == "pants"), Is.EqualTo(d4BuildExport.Pants.Count()));
            Assert.That(affixPreset.ItemAffixes.Count(i => i.Type == "boots"), Is.EqualTo(d4BuildExport.Boots.Count()));
            Assert.That(affixPreset.ItemAffixes.Count(i => i.Type == "amulet"), Is.EqualTo(d4BuildExport.Amulet.Count()));
            Assert.That(affixPreset.ItemAffixes.Count(i => i.Type == "ring"), Is.EqualTo(d4BuildExport.Rings.Count()));
            Assert.That(affixPreset.ItemAffixes.Count(i => i.Type == "weapon"), Is.EqualTo(d4BuildExport.Weapons.Count()));
            Assert.That(affixPreset.ItemAffixes.Count(i => i.Type == "ranged"), Is.EqualTo(d4BuildExport.RangedWeapon.Count()));
            Assert.That(affixPreset.ItemAffixes.Count(i => i.Type == "offhand"), Is.EqualTo(d4BuildExport.Offhand.Count()));
        });
    }

    [Test]
    public void Test_12_ExportAll()
    {
        var d4BuildExport = _scrapper.ExportAll();
        var affixPreset = _converter.ConvertAll(d4BuildExport);
        JsonHelpers<List<AffixPreset>>.Save("D4Companion/Ressources/d4buildsscrapper_exportall.json", affixPreset.ToList(), new JsonSerializerOptions() { WriteIndented = true });

        Assert.Multiple(() =>
        {
            Assert.That(d4BuildExport.Count(), Is.EqualTo(affixPreset.Count()));
        });
    }

    [Test]
    public void Test_20_Navigate()
    {
        Assert.DoesNotThrow(() => { _scrapper.Navigate("660881f7-cb6a-4162-be62-29f0afeb37bf"); });
    }

    [Test]
    public void Test_21_ExportVanilla()
    {
        var d4BuildExport = _scrapper.ExportVanilla();
        var affixPreset = _converter.Convert(d4BuildExport);
        JsonHelpers<AffixPreset>.Save("D4Companion/Ressources/d4buildsscrapper_exportvanilla.json", affixPreset, new JsonSerializerOptions() { WriteIndented = true });

        Assert.Multiple(() =>
        {
            // d4BuildExport
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

            // affixPreset
            Assert.That(affixPreset.Name, Is.EqualTo(d4BuildExport.Name));
            Assert.That(affixPreset.ItemAspects, Has.Count.EqualTo(d4BuildExport.Aspects.Count()));
            Assert.That(affixPreset.ItemAffixes.Count(i => i.Type == "helm"), Is.EqualTo(d4BuildExport.Helm.Count()));
            Assert.That(affixPreset.ItemAffixes.Count(i => i.Type == "chest"), Is.EqualTo(d4BuildExport.ChestArmor.Count()));
            Assert.That(affixPreset.ItemAffixes.Count(i => i.Type == "gloves"), Is.EqualTo(d4BuildExport.Gloves.Count()));
            Assert.That(affixPreset.ItemAffixes.Count(i => i.Type == "pants"), Is.EqualTo(d4BuildExport.Pants.Count()));
            Assert.That(affixPreset.ItemAffixes.Count(i => i.Type == "boots"), Is.EqualTo(d4BuildExport.Boots.Count()));
            Assert.That(affixPreset.ItemAffixes.Count(i => i.Type == "amulet"), Is.EqualTo(d4BuildExport.Amulet.Count()));
            Assert.That(affixPreset.ItemAffixes.Count(i => i.Type == "ring"), Is.EqualTo(d4BuildExport.Rings.Count()));
            Assert.That(affixPreset.ItemAffixes.Count(i => i.Type == "weapon"), Is.EqualTo(d4BuildExport.Weapons.Count()));
            Assert.That(affixPreset.ItemAffixes.Count(i => i.Type == "ranged"), Is.EqualTo(d4BuildExport.RangedWeapon.Count()));
            Assert.That(affixPreset.ItemAffixes.Count(i => i.Type == "offhand"), Is.EqualTo(d4BuildExport.Offhand.Count()));
        });
    }

    [Test]
    public void Test_22_ExportVanillaAll()
    {
        var d4BuildExport = _scrapper.ExportVanillaAll();
        var affixPreset = _converter.ConvertAll(d4BuildExport);
        JsonHelpers<List<AffixPreset>>.Save("D4Companion/Ressources/d4buildsscrapper_exportvanillaall.json", affixPreset.ToList(), new JsonSerializerOptions() { WriteIndented = true });

        Assert.Multiple(() =>
        {
            Assert.That(d4BuildExport.Count(), Is.EqualTo(affixPreset.Count()));
        });
    }

    [Test]
    public void Test_30_CompareExportResults()
    {
        var export = CalculateHash("D4Companion/Ressources/d4buildsscrapper_export.json");
        var exportAll = CalculateHash("D4Companion/Ressources/d4buildsscrapper_exportall.json");
        var exportVanilla = CalculateHash("D4Companion/Ressources/d4buildsscrapper_exportvanilla.json");
        var exportVanillaAll = CalculateHash("D4Companion/Ressources/d4buildsscrapper_exportvanillaall.json");

        Assert.Multiple(() =>
        {
            Assert.That(export, Is.EqualTo(exportVanilla));
            Assert.That(exportAll, Is.EqualTo(exportVanillaAll));
            Assert.That(export, Is.Not.EqualTo(exportAll));
            Assert.That(exportVanilla, Is.Not.EqualTo(exportVanillaAll));
        });
    }

    private static string CalculateHash(string filePath)
    {
        var text = File.ReadAllText(filePath);

        byte[] inputBytes = Encoding.UTF8.GetBytes(text);
        byte[] hashBytes = MD5.HashData(inputBytes);

        StringBuilder sb = new();
        foreach (byte b in hashBytes)
        {
            sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }
}
