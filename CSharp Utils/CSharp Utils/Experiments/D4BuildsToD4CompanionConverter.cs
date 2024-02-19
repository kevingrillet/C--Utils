using CSharp_Utils.Entities;
using CSharp_Utils.Entities.D4Companion;
using FuzzySharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CSharp_Utils.Experiments
{
    /// <summary>
    /// Converts D4BuildsExport objects to AffixPreset objects for the D4Companion application.
    /// </summary>
    /// <remarks>
    /// This class is responsible for converting D4BuildsExport objects, which represent Diablo 4 builds, into AffixPreset objects, which are used by the D4Companion application.
    /// The conversion process involves extracting relevant information from the D4BuildsExport objects and mapping them to the corresponding properties of the AffixPreset objects.
    /// The class uses lists of AffixInfo, AspectInfo, and D4ItemType objects to perform the conversion.
    /// </remarks>
    public class D4BuildsToD4CompanionConverter
    {
        private readonly List<AffixInfo> _affixInfos;
        private readonly List<AspectInfo> _aspectInfos;
        private readonly List<D4ItemType> _itemTypes;

        public D4BuildsToD4CompanionConverter()
        {
            // From: https://github.com/josdemmers/Diablo4Companion
            _affixInfos = JsonSerializer.Deserialize<List<AffixInfo>>(File.ReadAllText("Ressources/D4Companion/Affixes.enUS.json")) ?? [];
            _aspectInfos = JsonSerializer.Deserialize<List<AspectInfo>>(File.ReadAllText("Ressources/D4Companion/Aspects.enUS.json")) ?? [];
            // From: https://github.com/blizzhackers/d4data
            _itemTypes = JsonSerializer.Deserialize<List<D4ItemType>>(File.ReadAllText("Ressources/d4data/ItemTypes.json")) ?? [];
        }

        public AffixPreset Convert(D4BuildsExport d4BuildsExport)
        {
            ArgumentNullException.ThrowIfNull(d4BuildsExport);

            var result = new AffixPreset
            {
                Name = d4BuildsExport.Name,
                ItemAspects = BuildAspects(d4BuildsExport.D4Class, d4BuildsExport.Aspects).ToList(),
                ItemAffixes = BuildAffixes(d4BuildsExport.D4Class, d4BuildsExport.Helm, "helm", "Helm")
                    .Concat(BuildAffixes(d4BuildsExport.D4Class, d4BuildsExport.ChestArmor, "chest", "ChestArmor"))
                    .Concat(BuildAffixes(d4BuildsExport.D4Class, d4BuildsExport.Gloves, "gloves", "Gloves"))
                    .Concat(BuildAffixes(d4BuildsExport.D4Class, d4BuildsExport.Pants, "pants", "Legs"))
                    .Concat(BuildAffixes(d4BuildsExport.D4Class, d4BuildsExport.Boots, "boots", "Boots"))
                    .Concat(BuildAffixes(d4BuildsExport.D4Class, d4BuildsExport.Amulet, "amulet", "Amulet"))
                    .Concat(BuildAffixes(d4BuildsExport.D4Class, d4BuildsExport.Rings, "ring", "Ring"))
                    .Concat(BuildAffixes(d4BuildsExport.D4Class, d4BuildsExport.Weapons, "weapon", "Weapon"))
                    .Concat(BuildAffixes(d4BuildsExport.D4Class, d4BuildsExport.Offhand, "offhand", "Weapon"))
                    .Concat(BuildAffixes(d4BuildsExport.D4Class, d4BuildsExport.RangedWeapon, "ranged", "Weapon"))
                    .ToList()
            };

            return result;
        }

        public IEnumerable<AffixPreset> ConvertAll(IEnumerable<D4BuildsExport> d4BuildsExports)
        {
            foreach (var d4BuildsExport in d4BuildsExports)
            {
                yield return Convert(d4BuildsExport);
            }
        }

        protected IEnumerable<ItemAffix> BuildAffixes(D4Class d4Class, IEnumerable<string> affixes, string type, string itemType = null)
        {
            foreach (var affix in affixes.Where(a => !string.IsNullOrWhiteSpace(a)))
            {
                var description = string.Empty;
                if (string.IsNullOrWhiteSpace(itemType))
                {
                    description = Process.ExtractOne(
                        affix,
                        _affixInfos.Where(aa =>
                            aa.AllowedForPlayerClass[(int)d4Class] == 1
                        ).Select(aa => aa.Description)).Value;
                }
                else if (itemType == "Weapon")
                {
                    description = Process.ExtractOne(
                        affix,
                        _affixInfos.Where(aa =>
                            aa.AllowedForPlayerClass[(int)d4Class] == 1
                            && aa.AllowedItemLabels.Exists(ai => _itemTypes.Where(i => i.IsWeapon).SelectMany(i => i.ItemLabels).Distinct().Contains(ai))
                        ).Select(aa => aa.Description)).Value;
                }
                else
                {
                    description = Process.ExtractOne(
                        affix,
                        _affixInfos.Where(aa =>
                            aa.AllowedForPlayerClass[(int)d4Class] == 1
                            && aa.AllowedItemLabels.Exists(ai => _itemTypes.Find(i => i.TypeName == itemType).ItemLabels.Contains(ai))
                        ).Select(aa => aa.Description)).Value;
                }
                yield return new ItemAffix()
                {
                    Id = _affixInfos.Find(a => a.Description == description).IdName,
                    Type = type
                };
            }
        }

        protected IEnumerable<ItemAffix> BuildAspects(D4Class d4Class, IEnumerable<string> aspects, string type = "aspect", string itemType = null)
        {
            foreach (var aspect in aspects.Where(a => !string.IsNullOrWhiteSpace(a)))
            {
                var Name = string.Empty;
                if (string.IsNullOrWhiteSpace(itemType))
                {
                    Name = Process.ExtractOne(
                        aspect,
                        _aspectInfos.Where(aa =>
                            aa.AllowedForPlayerClass[(int)d4Class] == 1
                        ).Select(a => a.Name)).Value;
                }
                else if (itemType == "Weapon")
                {
                    Name = Process.ExtractOne(
                        aspect,
                        _aspectInfos.Where(aa =>
                            aa.AllowedForPlayerClass[(int)d4Class] == 1
                            && aa.AllowedItemLabels.Exists(ai => _itemTypes.Where(i => i.IsWeapon).SelectMany(i => i.ItemLabels).Distinct().Contains(ai))
                        ).Select(aa => aa.Name)).Value;
                }
                else
                {
                    Name = Process.ExtractOne(
                        aspect,
                        _aspectInfos.Where(aa =>
                            aa.AllowedForPlayerClass[(int)d4Class] == 1
                            && aa.AllowedItemLabels.Exists(ai => _itemTypes.Find(i => i.TypeName == itemType).ItemLabels.Contains(ai))
                        ).Select(aa => aa.Name)).Value;
                }
                yield return new ItemAffix()
                {
                    Id = _aspectInfos.Find(a => a.Name == Name).IdName,
                    Type = type
                };
            }
        }
    }
}
