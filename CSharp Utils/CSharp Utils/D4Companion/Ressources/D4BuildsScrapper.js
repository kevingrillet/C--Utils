function getAllAffixes(category) {
    var res = [];
    document.querySelectorAll(`:scope .${category} .filled`).forEach((e) => res.push(e.innerText));
    return res;
}

function getAllAspects() {
    var res = [];
    document.querySelectorAll(`.builder__gear__name`).forEach((e) => res.push(e.innerText));
    res = res.filter(function (e) {
        return e.includes('Aspect');
    });
    return res;
}

function getClass() {
    var cls = document.querySelector('.builder__header__description').lastChild.textContent;
    switch (cls) {
        case 'Sorcerer':
            return 0;

        case 'Druid':
            return 1;

        case 'Barbarian':
            return 2;

        case 'Rogue':
            return 3;

        case 'Necromancer':
            return 4;

        default:
            return null;
    }
}

function getName() {
    var buildName = document.querySelector('#renameBuild').value;
    var variantName = document.querySelector('.variant__button.active').firstElementChild.firstElementChild.value;
    return `${buildName} - ${variantName}`;
}

var result = {};

// Name
result.Name = getName();

// Class
result.D4Class = getClass();

// Aspects
result.Aspects = getAllAspects();

// Armor
result.Helm = getAllAffixes('Helm');
result.ChestArmor = getAllAffixes('ChestArmor');
result.Gloves = getAllAffixes('Gloves');
result.Pants = getAllAffixes('Pants');
result.Boots = getAllAffixes('Boots');

// Accessories
result.Amulet = getAllAffixes('Amulet');
result.Ring1 = getAllAffixes('Ring1');
result.Ring2 = getAllAffixes('Ring2');

// Weapons
result.Weapon = getAllAffixes('Weapon');
result.Offhand = getAllAffixes('Offhand');
result.RangedWeapon = getAllAffixes('RangedWeapon');
result.BludgeoningWeapon = getAllAffixes('BludgeoningWeapon');
result.SlashingWeapon = getAllAffixes('SlashingWeapon');
result.WieldWeapon1 = getAllAffixes('WieldWeapon1');
result.WieldWeapon2 = getAllAffixes('WieldWeapon2');

// console.debug(JSON.stringify(result, null, 2));
return JSON.stringify(result, null, 2);
