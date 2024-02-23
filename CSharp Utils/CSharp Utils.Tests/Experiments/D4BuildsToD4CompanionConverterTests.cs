using CSharp_Utils.Entities;
using CSharp_Utils.Entities.D4Companion;
using CSharp_Utils.Experiments;
using CSharp_Utils.Helpers;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Linq;
using System.Text.Json;

namespace CSharp_Utils.Tests.Experiments
{
    [TestFixture, Parallelizable]
    internal class D4BuildsToD4CompanionConverterTests : D4BuildsToD4CompanionConverter
    {
        private AffixPreset _affixPreset;
        private D4BuildsExport _d4BuildExport;

        [OneTimeSetUp]
        public void AA_OneTimeSetUp()
        {
            // Generated with https://raw.githubusercontent.com/kevingrillet/Userscripts/main/user.js/[D4Builds]%20JsonExporterForDiablo4Companion.user.js
            _d4BuildExport = JsonHelpers<D4BuildsExport>.Load("Ressources/D4Builds.Rob's Bone Spear (S3).json") ?? new();

            _affixPreset = new()
            {
                Name = _d4BuildExport.Name
            };
        }

        [Test]
        public void Test_0_Init()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_d4BuildExport, Is.Not.Null);
                Assert.That(_affixPreset, Is.Not.Null);
            });
        }

        [Test]
        public void Test_20_Affixes()
        {
            _affixPreset.ItemAffixes.AddRange(BuildAffixes(_d4BuildExport.D4Class, _d4BuildExport.Helm, "helm", "Helm"));
            _affixPreset.ItemAffixes.AddRange(BuildAffixes(_d4BuildExport.D4Class, _d4BuildExport.ChestArmor, "chest", "ChestArmor"));
            _affixPreset.ItemAffixes.AddRange(BuildAffixes(_d4BuildExport.D4Class, _d4BuildExport.Gloves, "gloves", "Gloves"));
            _affixPreset.ItemAffixes.AddRange(BuildAffixes(_d4BuildExport.D4Class, _d4BuildExport.Pants, "pants", "Legs"));
            _affixPreset.ItemAffixes.AddRange(BuildAffixes(_d4BuildExport.D4Class, _d4BuildExport.Boots, "boots", "Boots"));
            _affixPreset.ItemAffixes.AddRange(BuildAffixes(_d4BuildExport.D4Class, _d4BuildExport.Amulet, "amulet", "Amulet"));
            _affixPreset.ItemAffixes.AddRange(BuildAffixes(_d4BuildExport.D4Class, _d4BuildExport.Rings, "ring", "Ring"));
            _affixPreset.ItemAffixes.AddRange(BuildAffixes(_d4BuildExport.D4Class, _d4BuildExport.Weapons, "weapon", "Weapon"));
            _affixPreset.ItemAffixes.AddRange(BuildAffixes(_d4BuildExport.D4Class, _d4BuildExport.Offhand, "offhand", "Weapon"));
            _affixPreset.ItemAffixes.AddRange(BuildAffixes(_d4BuildExport.D4Class, _d4BuildExport.RangedWeapon, "ranged", "Weapon"));

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
        public void Test_21_Aspects()
        {
            _affixPreset.ItemAspects.AddRange(BuildAspects(_d4BuildExport.D4Class, _d4BuildExport.Aspects));

            Assert.That(_affixPreset.ItemAspects, Has.Count.EqualTo(_d4BuildExport.Aspects.Count()));
        }

        [Test]
        public void Test_30_Save()
        {
            JsonHelpers<AffixPreset>.Save("Ressources/d4builds_export.json", _affixPreset, new JsonSerializerOptions() { WriteIndented = true });
            Assert.Pass();
        }
    }
}
