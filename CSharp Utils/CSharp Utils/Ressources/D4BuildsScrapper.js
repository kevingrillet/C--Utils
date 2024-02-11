function getAllAffixes(category) {
    var res = [];
    document.querySelectorAll(`:scope .${category} .filled`).forEach((e) => res.push(e.innerText));
    return res;
}

function getAllAspects() {
    var res = [];
    document.querySelectorAll(`:scope .builder__gear__name`).forEach((e) => res.push(e.innerText));
    res = res.filter(function (e) {
        return e.includes('Aspect');
    });
    return res;
}

function getClass() {
    switch (document.querySelector('.builder__header__description').lastChild.textContent) {
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

var result = {};
result.Name = `${document.querySelector('#renameBuild').value} - ${document.querySelector('.variant__button.active').firstElementChild.firstElementChild.value}`;
result.D4Class = getClass();

result.Aspects = getAllAspects();

result.Helm = getAllAffixes('Helm');
result.ChestArmor = getAllAffixes('ChestArmor');
result.Gloves = getAllAffixes('Gloves');
result.Pants = getAllAffixes('Pants');
result.Boots = getAllAffixes('Boots');
result.Amulet = getAllAffixes('Amulet');
result.Ring1 = getAllAffixes('Ring1');
result.Ring2 = getAllAffixes('Ring2');

if (document.querySelector('.Weapon')) result.Weapon = getAllAffixes('Weapon');
if (document.querySelector('.Offhand')) result.Offhand = getAllAffixes('Offhand');
if (document.querySelector('.RangedWeapon')) result.Weapon = getAllAffixes('RangedWeapon');
if (document.querySelector('.BludgeoningWeapon')) result.BludgeoningWeapon = getAllAffixes('BludgeoningWeapon');
if (document.querySelector('.SlashingWeapon')) result.SlashingWeapon = getAllAffixes('SlashingWeapon');
if (document.querySelector('.WieldWeapon1')) result.WieldWeapon1 = getAllAffixes('WieldWeapon1');
if (document.querySelector('.WieldWeapon2')) result.WieldWeapon2 = getAllAffixes('WieldWeapon2');

return JSON.stringify(result, null, 2);
