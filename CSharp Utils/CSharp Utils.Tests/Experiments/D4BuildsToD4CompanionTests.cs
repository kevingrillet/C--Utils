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

        [OneTimeSetUp]
        public void AA_OneTimeSetUp()
        {
            // From: https://github.com/josdemmers/Diablo4Companion
            _affixInfos = JsonHelpers<List<AffixInfo>>.Load("Ressources/D4Companion/Affixes.enUS.json") ?? [];
            _aspectInfos = JsonHelpers<List<AspectInfo>>.Load("Ressources/D4Companion/Aspects.enUS.json") ?? [];
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
