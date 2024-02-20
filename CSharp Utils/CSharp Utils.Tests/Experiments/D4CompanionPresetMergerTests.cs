using CSharp_Utils.Entities.D4Companion;
using CSharp_Utils.Experiments;
using CSharp_Utils.Helpers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CSharp_Utils.Tests.Experiments
{
    [TestFixture]
    internal class D4CompanionPresetMergerTests
    {
        private List<AffixPreset> _affixePresets;
        private D4CompanionPresetMerger _d4CompanionPresetMerger;

        [OneTimeSetUp]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1869:Mettre en cache et réutiliser les instances « JsonSerializerOptions »", Justification = "<En attente>")]
        public void OneTimeSetUp()
        {
            var json = File.ReadAllText("Ressources/D4Companion.Rob's Bone Spear (S3).full.json");
            var jsonSerializerOptions = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                WriteIndented = true,
            };
            jsonSerializerOptions.Converters.Add(new CustomColorConverter());
            _affixePresets = JsonSerializer.Deserialize<List<AffixPreset>>(json, jsonSerializerOptions) ?? [];
            _affixePresets = [.. _affixePresets.OrderByDescending(a => a.Name)];

            _d4CompanionPresetMerger = new();
        }

        [Test]
        public void Test_MergeAuto()
        {
            _d4CompanionPresetMerger.Mode = D4CompanionPresetMergerMode.AUTO;
            var result = _d4CompanionPresetMerger.Merge("Rob's Bone Spear (S3)", _affixePresets);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Name, Is.Not.Empty);

                Assert.That(result.ItemAffixes, Is.Not.Empty);
                Assert.That(result.ItemAffixes.Count(i => i.Color.ToArgb() == Color.Gold.ToArgb()), Is.EqualTo(28), "Affixes Gold");
                Assert.That(result.ItemAffixes.Count(i => i.Color.ToArgb() == Color.Green.ToArgb()), Is.EqualTo(0), "Affixes Green");
                Assert.That(result.ItemAffixes.Count(i => i.Color.ToArgb() == Color.Blue.ToArgb()), Is.EqualTo(0), "Affixes Blue");

                Assert.That(result.ItemAspects, Is.Not.Empty);
                Assert.That(result.ItemAspects.Count(i => i.Color.ToArgb() == Color.Gold.ToArgb()), Is.EqualTo(7), "Affixes Gold");
                Assert.That(result.ItemAspects.Count(i => i.Color.ToArgb() == Color.Green.ToArgb()), Is.EqualTo(2), "Affixes Green");
                Assert.That(result.ItemAspects.Count(i => i.Color.ToArgb() == Color.Blue.ToArgb()), Is.EqualTo(1), "Affixes Blue");

                Assert.That(result.ItemSigils, Is.Empty);
            });
        }

        [Test]
        public void Test_MergeColor()
        {
            _d4CompanionPresetMerger.Mode = D4CompanionPresetMergerMode.COLOR;
            var result = _d4CompanionPresetMerger.Merge("Rob's Bone Spear (S3)", _affixePresets, [Color.Yellow, Color.Lime, Color.AliceBlue], Color.Orange);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Name, Is.Not.Empty);

                Assert.That(result.ItemAffixes, Is.Not.Empty);
                Assert.That(result.ItemAffixes.Count(i => i.Color.ToArgb() == Color.Yellow.ToArgb()), Is.EqualTo(8), "Affixes Yellow");
                Assert.That(result.ItemAffixes.Count(i => i.Color.ToArgb() == Color.Lime.ToArgb()), Is.EqualTo(0), "Affixes Lime");
                Assert.That(result.ItemAffixes.Count(i => i.Color.ToArgb() == Color.AliceBlue.ToArgb()), Is.EqualTo(0), "Affixes AliceBlue");
                Assert.That(result.ItemAffixes.Count(i => i.Color.ToArgb() == Color.Orange.ToArgb()), Is.EqualTo(20), "Affixes Orange");

                Assert.That(result.ItemAspects, Is.Not.Empty);
                Assert.That(result.ItemAspects.Count(i => i.Color.ToArgb() == Color.Yellow.ToArgb()), Is.EqualTo(1), "Affixes Yellow");
                Assert.That(result.ItemAspects.Count(i => i.Color.ToArgb() == Color.Lime.ToArgb()), Is.EqualTo(2), "Affixes Lime");
                Assert.That(result.ItemAspects.Count(i => i.Color.ToArgb() == Color.AliceBlue.ToArgb()), Is.EqualTo(1), "Affixes AliceBlue");
                Assert.That(result.ItemAspects.Count(i => i.Color.ToArgb() == Color.Orange.ToArgb()), Is.EqualTo(6), "Affixes Orange");

                Assert.That(result.ItemSigils, Is.Empty);
            });
        }
    }
}
