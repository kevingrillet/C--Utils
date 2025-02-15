﻿using System.Collections.Generic;

namespace CSharp_Utils.D4Companion.Entities.D4Companion;

/// <summary>
/// Source: <see href="https://github.com/josdemmers/Diablo4Companion"/>
/// </summary>
internal class AspectInfo
{
    public List<int> AllowedForPlayerClass { get; set; } = [];
    public List<int> AllowedItemLabels { get; set; } = [];
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Dungeon { get; set; } = string.Empty;
    public string IdName { get; set; } = string.Empty;
    public int IdSno { get; set; }
    public bool IsCodex { get; set; } = false;
    public bool IsSeasonal { get; set; } = false;
    public string Localisation { get; set; } = string.Empty;

    /// <summary>
    /// None: 0 (Affixes)
    /// Legendary: 1 (Aspects)
    /// Unique: 2
    /// Test: 3
    /// </summary>
    public int MagicType { get; set; }

    public string Name { get; set; } = string.Empty;
}
