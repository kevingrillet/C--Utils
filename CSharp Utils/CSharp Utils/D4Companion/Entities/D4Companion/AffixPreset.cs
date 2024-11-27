using System.Collections.Generic;

namespace CSharp_Utils.D4Companion.Entities.D4Companion;

/// <summary>
/// Source: <see href="https://github.com/josdemmers/Diablo4Companion"/>
/// </summary>
public class AffixPreset
{
    public List<ItemAffix> ItemAffixes { get; set; } = [];
    public List<ItemAffix> ItemAspects { get; set; } = [];
    public List<ItemAffix> ItemSigils { get; set; } = [];
    public string Name { get; set; } = string.Empty;
}
