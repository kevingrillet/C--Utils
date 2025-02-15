﻿using System.Collections.Generic;
using System.Linq;

namespace CSharp_Utils.D4Companion.Entities;

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
    public IEnumerable<string> Rings => Ring1.Concat(Ring2).Distinct();
    public IEnumerable<string> SlashingWeapon { get; set; }
    public IEnumerable<string> Weapon { get; set; }
    public IEnumerable<string> Weapons => BludgeoningWeapon.Concat(SlashingWeapon).Concat(Weapon).Concat(WieldWeapon1).Concat(WieldWeapon2).Distinct();
    public IEnumerable<string> WieldWeapon1 { get; set; }
    public IEnumerable<string> WieldWeapon2 { get; set; }

    public D4BuildsExport()
    {
        Amulet = [];
        Aspects = [];
        BludgeoningWeapon = [];
        Boots = [];
        ChestArmor = [];
        Gloves = [];
        Helm = [];
        Offhand = [];
        Pants = [];
        RangedWeapon = [];
        Ring1 = [];
        Ring2 = [];
        SlashingWeapon = [];
        Weapon = [];
        WieldWeapon1 = [];
        WieldWeapon2 = [];
    }
}
