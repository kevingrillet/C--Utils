using CSharp_Utils.Helpers;
using CSharp_Utils.Tests.Entities;
using CSharp_Utils.Tests.Entities.D4Companion;
using FuzzySharp;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace CSharp_Utils.Tests.Experiments
{
    [TestFixture]
    internal class D4BuildsToD4CompanionTests
    {
        private List<AffixInfo> _affixInfos;
        private AffixPreset _affixPreset;
        private List<AspectInfo> _aspectInfos;
        private D4BuildsExport _d4BuildExport;
        private List<D4ItemType> _itemTypes;

        [OneTimeSetUp]
        public void AA_OneTimeSetUp()
        {
            // From: https://github.com/josdemmers/Diablo4Companion
            _affixInfos = JsonHelpers<List<AffixInfo>>.Load("Ressources/D4Companion/Affixes.enUS.json") ?? [];
            _aspectInfos = JsonHelpers<List<AspectInfo>>.Load("Ressources/D4Companion/Aspects.enUS.json") ?? [];
            // From: https://github.com/blizzhackers/d4data
            _itemTypes = JsonHelpers<List<D4ItemType>>.Load("Ressources/d4data/ItemTypes.json") ?? [];
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
                Assert.That(_affixInfos, Is.Not.Null);
                Assert.That(_aspectInfos, Is.Not.Null);
                Assert.That(_d4BuildExport, Is.Not.Null);
                Assert.That(_affixPreset, Is.Not.Null);
            });
        }

        [Test]
        public void Test_20_Affixes()
        {
            BuildAffixes(_d4BuildExport.Helm, "helm", "Helm");
            BuildAffixes(_d4BuildExport.ChestArmor, "chest", "ChestArmor");
            BuildAffixes(_d4BuildExport.Gloves, "gloves", "Gloves");
            BuildAffixes(_d4BuildExport.Pants, "pants", "Legs");
            BuildAffixes(_d4BuildExport.Boots, "boots", "Boots");
            BuildAffixes(_d4BuildExport.Amulet, "amulet", "Amulet");
            BuildAffixes(_d4BuildExport.Rings, "ring", "Ring");
            BuildAffixes(_d4BuildExport.Weapons, "weapon", "Weapon");
            BuildAffixes(_d4BuildExport.Offhand, "offhand", "Weapon");
            BuildAffixes(_d4BuildExport.RangedWeapon, "ranged", "Weapon");

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
            BuildAspects(_d4BuildExport.Aspects);

            Assert.That(_affixPreset.ItemAspects, Has.Count.EqualTo(_d4BuildExport.Aspects.Count()));
        }

        [Test]
        public void Test_30_Save()
        {
            JsonHelpers<AffixPreset>.Save("Ressources/d4builds_export.json", _affixPreset, new JsonSerializerOptions() { WriteIndented = true });
            Assert.Pass();
        }

        private void BuildAffixes(IEnumerable<string> affixes, string type, string itemType = null)
        {
            var description = string.Empty;
            foreach (var affix in affixes)
            {
                if (string.IsNullOrWhiteSpace(itemType))
                {
                    description = Process.ExtractOne(
                        affix,
                        _affixInfos.Where(aa =>
                            aa.AllowedForPlayerClass[(int)_d4BuildExport.D4Class] == 1
                        ).Select(aa => aa.Description)).Value;
                }
                if (itemType == "Weapon")
                {
                    description = Process.ExtractOne(
                        affix,
                        _affixInfos.Where(aa =>
                            aa.AllowedForPlayerClass[(int)_d4BuildExport.D4Class] == 1
                            && aa.AllowedItemLabels.Exists(ai => _itemTypes.Where(i => i.IsWeapon).SelectMany(i => i.ItemLabels).Distinct().Contains(ai))
                        ).Select(aa => aa.Description)).Value;
                }
                else
                {
                    description = Process.ExtractOne(
                        affix,
                        _affixInfos.Where(aa =>
                            aa.AllowedForPlayerClass[(int)_d4BuildExport.D4Class] == 1
                            && aa.AllowedItemLabels.Exists(ai => _itemTypes.Find(i => i.TypeName == itemType).ItemLabels.Contains(ai))
                        ).Select(aa => aa.Description)).Value;
                }
                _affixPreset.ItemAffixes.Add(new ItemAffix()
                {
                    Id = _affixInfos.Find(a => a.Description == description).IdName,
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
