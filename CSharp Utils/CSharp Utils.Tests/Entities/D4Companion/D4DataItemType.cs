using System.Collections.Generic;
using System.Linq;

namespace CSharp_Utils.Tests.Entities.D4Companion
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Styles d'affectation de noms", Justification = "<En attente>")]
    internal class D4DataItemType
    {
        public string __fileName__ { get; set; }
        public IEnumerable<int> arItemLabels { get; set; }
        public int eWeaponClass { get; set; }

        public D4DataItemType()
        {
            arItemLabels = Enumerable.Empty<int>();
        }
    }
}
