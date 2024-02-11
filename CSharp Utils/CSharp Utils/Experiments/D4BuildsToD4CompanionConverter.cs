using CSharp_Utils.Entities;
using CSharp_Utils.Entities.D4Companion;
using CSharp_Utils.Helpers;
using FuzzySharp;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharp_Utils.Experiments
{
    public class D4BuildsToD4CompanionConverter
    {
        private readonly List<AffixInfo> _affixInfos;
        private readonly List<AspectInfo> _aspectInfos;
        private readonly List<D4ItemType> _itemTypes;

        public D4BuildsToD4CompanionConverter()
        {
            // From: https://github.com/josdemmers/Diablo4Companion
            _affixInfos = JsonHelpers<List<AffixInfo>>.Load("Ressources/D4Companion/Affixes.enUS.json") ?? [];
            _aspectInfos = JsonHelpers<List<AspectInfo>>.Load("Ressources/D4Companion/Aspects.enUS.json") ?? [];
            // From: https://github.com/blizzhackers/d4data
            _itemTypes = JsonHelpers<List<D4ItemType>>.Load("Ressources/d4data/ItemTypes.json") ?? [];
        }

        public AffixPreset Convert(D4BuildsExport d4BuildsExport)
        {
            ArgumentNullException.ThrowIfNull(d4BuildsExport);

            var result = new AffixPreset
            {
                Name = d4BuildsExport.Name,
            };

            result.ItemAspects.AddRange(BuildAspects(d4BuildsExport.D4Class, d4BuildsExport.Aspects));

            result.ItemAffixes.AddRange(BuildAffixes(d4BuildsExport.D4Class, d4BuildsExport.Helm, "helm", "Helm"));
            result.ItemAffixes.AddRange(BuildAffixes(d4BuildsExport.D4Class, d4BuildsExport.ChestArmor, "chest", "ChestArmor"));
            result.ItemAffixes.AddRange(BuildAffixes(d4BuildsExport.D4Class, d4BuildsExport.Gloves, "gloves", "Gloves"));
            result.ItemAffixes.AddRange(BuildAffixes(d4BuildsExport.D4Class, d4BuildsExport.Pants, "pants", "Legs"));
            result.ItemAffixes.AddRange(BuildAffixes(d4BuildsExport.D4Class, d4BuildsExport.Boots, "boots", "Boots"));
            result.ItemAffixes.AddRange(BuildAffixes(d4BuildsExport.D4Class, d4BuildsExport.Amulet, "amulet", "Amulet"));
            result.ItemAffixes.AddRange(BuildAffixes(d4BuildsExport.D4Class, d4BuildsExport.Rings, "ring", "Ring"));
            result.ItemAffixes.AddRange(BuildAffixes(d4BuildsExport.D4Class, d4BuildsExport.Weapons, "weapon", "Weapon"));
            result.ItemAffixes.AddRange(BuildAffixes(d4BuildsExport.D4Class, d4BuildsExport.Offhand, "offhand", "Weapon"));
            result.ItemAffixes.AddRange(BuildAffixes(d4BuildsExport.D4Class, d4BuildsExport.RangedWeapon, "ranged", "Weapon"));

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
            var description = string.Empty;
            foreach (var affix in affixes)
            {
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
            var Name = string.Empty;
            foreach (var aspect in aspects)
            {
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
