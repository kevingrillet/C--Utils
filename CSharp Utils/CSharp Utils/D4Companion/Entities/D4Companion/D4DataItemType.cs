using System.Collections.Generic;

namespace CSharp_Utils.D4Companion.Entities.D4Companion;

/// <summary>
/// Source: <see href="https://github.com/josdemmers/Diablo4Companion"/>
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Styles d'affectation de noms", Justification = "<En attente>")]
public class D4DataItemType
{
    public string __fileName__ { get; set; }
    public IEnumerable<int> arItemLabels { get; set; }
    public int eWeaponClass { get; set; }

    public D4DataItemType()
    {
        arItemLabels = [];
    }
}
