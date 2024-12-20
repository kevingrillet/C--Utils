﻿using System.Collections.Generic;

namespace CSharp_Utils.D4Companion.Entities.D4Companion;

/// <summary>
/// Source: <see href="https://github.com/josdemmers/Diablo4Companion"/>
/// </summary>
public class AffixAttribute
{
    public string Localisation { get; set; } = string.Empty;
    public string LocalisationId { get; set; } = string.Empty;
    public uint LocalisationParameter { get; set; }
}

public class AffixInfo
{
    public List<AffixAttribute> AffixAttributes { get; set; } = [];
    public List<int> AllowedForPlayerClass { get; set; } = [];
    public List<int> AllowedItemLabels { get; set; } = [];
    public string Description { get; set; } = string.Empty;
    public string IdName { get; set; } = string.Empty;
    public int IdSno { get; set; }

    /// <summary>
    /// None: 0 (Affixes)
    /// Legendary: 1 (Aspects)
    /// Unique: 2
    /// Test: 3
    /// </summary>
    public int MagicType { get; set; }
}
