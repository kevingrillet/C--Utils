using NUnit.Framework;
using System;
using System.Linq;
using CSharp_Utils.Helpers;
using System.Text.Json;
using CSharp_Utils.Entities.D4Companion;
using CSharp_Utils.Entities;
using CSharp_Utils.Experiments;

namespace CSharp_Utils.Tests.Experiments
{
    [TestFixture, Category("NotOnGitHub")]
    internal class D4BuildsToD4CompanionScrapperTests
    {
        private AffixPreset _affixPreset;
        private D4BuildsToD4CompanionConverter _converter;
        private D4BuildsExport _d4BuildExport;
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
        public void Test_10_Navigate()
        {
            _scrapper.Navigate("660881f7-cb6a-4162-be62-29f0afeb37bf");

            Assert.Pass();
        }

        [Test]
        public void Test_11_Export_V2_JS()
        {
            _d4BuildExport = _scrapper.Export();

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
        public void Test_20_Convert()
        {
            _affixPreset = _converter.Convert(_d4BuildExport);

            Assert.Multiple(() =>
            {
                Assert.That(_affixPreset.Name, Is.EqualTo(_d4BuildExport.Name));
                Assert.That(_affixPreset.ItemAspects, Has.Count.EqualTo(_d4BuildExport.Aspects.Count()));
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
        public void Test_30_Save()
        {
            JsonHelpers<AffixPreset>.Save("Ressources/d4buildsscrapper_export.json", _affixPreset, new JsonSerializerOptions() { WriteIndented = true });
            Assert.Pass();
        }
    }
}
