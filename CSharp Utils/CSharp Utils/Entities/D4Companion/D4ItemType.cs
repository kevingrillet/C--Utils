using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharp_Utils.Entities.D4Companion
{
    public class D4ItemType
    {
        internal static readonly char[] separator = ['/', '.'];
        public bool IsWeapon { get; set; }
        public List<int> ItemLabels { get; set; }
        public string TypeName { get; set; }

        public D4ItemType()
        {
            ItemLabels = [];
        }

        public D4ItemType(D4DataItemType d4DataItemType)
        {
            IsWeapon = d4DataItemType.eWeaponClass > -1;
            ItemLabels = d4DataItemType.arItemLabels.ToList();
            TypeName = d4DataItemType.__fileName__.Split(separator, StringSplitOptions.RemoveEmptyEntries)[^2];
        }
    }
}
