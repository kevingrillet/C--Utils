using System.Collections.Generic;
using System.Linq;

namespace CSharp_Utils.Entities
{
    public class D4BuildsExport
    {
        public IEnumerable<string> Amulet { get; set; }
        public IEnumerable<string> Aspects { get; set; }
        public IEnumerable<string> BludgeoningWeapon { get; set; }
        public IEnumerable<string> Boots { get; set; }
        public IEnumerable<string> ChestArmor { get; set; }
        public D4Class D4Class { get; set; }
        public IEnumerable<string> Gloves { get; set; }
        public IEnumerable<string> Helm { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Offhand { get; set; }
        public IEnumerable<string> Pants { get; set; }
        public IEnumerable<string> RangedWeapon { get; set; }
        public IEnumerable<string> Ring1 { get; set; }
        public IEnumerable<string> Ring2 { get; set; }

        public IEnumerable<string> Rings
        { get { return Ring1.Concat(Ring2); } }

        public IEnumerable<string> SlashingWeapon { get; set; }
        public IEnumerable<string> Weapon { get; set; }

        public IEnumerable<string> Weapons
        { get { return BludgeoningWeapon.Concat(SlashingWeapon).Concat(Weapon).Concat(WieldWeapon1).Concat(WieldWeapon2); } }

        public IEnumerable<string> WieldWeapon1 { get; set; }
        public IEnumerable<string> WieldWeapon2 { get; set; }

        public D4BuildsExport()
        {
            Amulet = Enumerable.Empty<string>();
            Aspects = Enumerable.Empty<string>();
            BludgeoningWeapon = Enumerable.Empty<string>();
            Boots = Enumerable.Empty<string>();
            ChestArmor = Enumerable.Empty<string>();
            Gloves = Enumerable.Empty<string>();
            Helm = Enumerable.Empty<string>();
            Offhand = Enumerable.Empty<string>();
            Pants = Enumerable.Empty<string>();
            RangedWeapon = Enumerable.Empty<string>();
            Ring1 = Enumerable.Empty<string>();
            Ring2 = Enumerable.Empty<string>();
            SlashingWeapon = Enumerable.Empty<string>();
            Weapon = Enumerable.Empty<string>();
            WieldWeapon1 = Enumerable.Empty<string>();
            WieldWeapon2 = Enumerable.Empty<string>();
        }
    }
}
