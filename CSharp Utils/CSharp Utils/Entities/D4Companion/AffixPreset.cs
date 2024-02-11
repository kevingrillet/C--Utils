using System.Collections.Generic;

// From https://github.com/josdemmers/Diablo4Companion
namespace CSharp_Utils.Entities.D4Companion
{
    public class AffixPreset
    {
        public List<ItemAffix> ItemAffixes { get; set; } = [];
        public List<ItemAffix> ItemAspects { get; set; } = [];
        public List<ItemAffix> ItemSigils { get; set; } = [];
        public string Name { get; set; } = string.Empty;
    }
}
