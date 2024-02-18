using CSharp_Utils.Entities.D4Companion;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CSharp_Utils.Experiments
{
    public enum D4CompanionPressetMergerMode
    {
        AUTO = 0,
        COLOR = 1,
    }

    /// <summary>
    /// The <c>D4CompanionPressetMerger</c> class is responsible for merging multiple <c>AffixPreset</c> objects into a single <c>AffixPreset</c> object based on different merging modes.
    /// </summary>
    public class D4CompanionPressetMerger
    {
        /// <summary>
        /// The current merging mode, which can be <c>AUTO</c> or <c>COLOR</c>.
        /// </summary>
        public D4CompanionPressetMergerMode Mode { get; set; }

        /// <summary>
        /// Initializes a new instance of the <c>D4CompanionPressetMerger</c> class with the default merging mode set to <c>AUTO</c>.
        /// </summary>
        public D4CompanionPressetMerger()
        {
            Mode = D4CompanionPressetMergerMode.AUTO;
        }

        /// <summary>
        /// Merges the specified <c>AffixPreset</c> objects into a single <c>AffixPreset</c> object based on the current merging mode.
        /// </summary>
        /// <param name="name">The name of the merged <c>AffixPreset</c> object.</param>
        /// <param name="affixPresets">The list of <c>AffixPreset</c> objects to merge.</param>
        /// <param name="pressetColors">The list of colors corresponding to each <c>AffixPreset</c> object (required only in <c>COLOR</c> mode).</param>
        /// <param name="commonColor">The common color to use for duplicate objects (required only in <c>COLOR</c> mode).</param>
        /// <returns>The merged <c>AffixPreset</c> object.</returns>
        public AffixPreset Merge(string name, IList<AffixPreset> affixPresets, IList<Color> pressetColors = null, Color? commonColor = null)
        {
            return Mode switch
            {
                D4CompanionPressetMergerMode.AUTO => MergeAuto(name, affixPresets),
                D4CompanionPressetMergerMode.COLOR => MergeColor(name, affixPresets, pressetColors, commonColor.Value),
                _ => throw new NotImplementedException(),
            };
        }

        /// <summary>
        /// Merges the specified <c>AffixPreset</c> objects in <c>AUTO</c> mode.
        /// </summary>
        /// <param name="name">The name of the merged <c>AffixPreset</c> object.</param>
        /// <param name="affixPresets">The list of <c>AffixPreset</c> objects to merge.</param>
        /// <returns>The merged <c>AffixPreset</c> object.</returns>
        protected static AffixPreset MergeAuto(string name, IList<AffixPreset> affixPresets)
        {
            var affixPreset = new AffixPreset()
            {
                Name = name,
            };

            foreach (var preset in affixPresets)
            {
                MergeItems(preset.ItemAffixes, affixPreset.ItemAffixes);
                MergeItems(preset.ItemAspects, affixPreset.ItemAspects);
                MergeItems(preset.ItemSigils, affixPreset.ItemSigils);
            }

            return affixPreset;
        }

        /// <summary>
        /// Merges the specified <c>AffixPreset</c> objects in <c>COLOR</c> mode.
        /// </summary>
        /// <param name="name">The name of the merged <c>AffixPreset</c> object.</param>
        /// <param name="affixPresets">The list of <c>AffixPreset</c> objects to merge.</param>
        /// <param name="pressetColors">The list of colors corresponding to each <c>AffixPreset</c> object.</param>
        /// <param name="commonColor">The common color to use for duplicate objects.</param>
        /// <returns>The merged <c>AffixPreset</c> object.</returns>
        protected static AffixPreset MergeColor(string name, IList<AffixPreset> affixPresets, IList<Color> pressetColors, Color commonColor)
        {
            var affixPreset = new AffixPreset()
            {
                Name = name,
            };

            for (var i = 0; i < affixPresets.Count; i++)
            {
                var preset = affixPresets[i];
                var color = pressetColors[i];

                MergeItemsWithColor(preset.ItemAffixes, affixPreset.ItemAffixes, color, commonColor);
                MergeItemsWithColor(preset.ItemAspects, affixPreset.ItemAspects, color, commonColor);
                MergeItemsWithColor(preset.ItemSigils, affixPreset.ItemSigils, color, commonColor);
            }

            return affixPreset;
        }

        /// <summary>
        /// Merges the specified list of items into the target list, skipping duplicates.
        /// </summary>
        /// <param name="source">The source list of items.</param>
        /// <param name="target">The target list of items.</param>
        protected static void MergeItems(List<ItemAffix> source, List<ItemAffix> target)
        {
            foreach (var item in source)
            {
                if (!target.Exists(i => i.Id == item.Id && i.Type == item.Type))
                {
                    target.Add(item);
                }
            }
        }

        /// <summary>
        /// Merges the specified list of items into the target list, updating the color of duplicate items.
        /// </summary>
        /// <param name="source">The source list of items.</param>
        /// <param name="target">The target list of items.</param>
        /// <param name="color">The color to set for new items.</param>
        /// <param name="commonColor">The common color to set for duplicate items.</param>
        protected static void MergeItemsWithColor(List<ItemAffix> source, List<ItemAffix> target, Color color, Color commonColor)
        {
            foreach (var item in source)
            {
                if (!target.Exists(i => i.Id == item.Id && i.Type == item.Type))
                {
                    target.Add(new ItemAffix { Color = color, Id = item.Id, Type = item.Type });
                }
                else
                {
                    target.Find(i => i.Id == item.Id).Color = commonColor;
                }
            }
        }
    }
}
