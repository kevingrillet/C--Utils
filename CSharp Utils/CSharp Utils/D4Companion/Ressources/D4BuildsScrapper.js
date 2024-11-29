function getAllAffixes(category) {
    let res = [];
    document.querySelectorAll(`:scope .${category} .filled`).forEach((e) => res.push(e.innerText));
    return res;
}

function getAllAspects() {
    let res = [];
    document.querySelectorAll(`.builder__gear__name`).forEach((e) => res.push(e.innerText));
    res = res.filter(function (e) {
        return e.includes('Aspect');
    });
    return res;
}

function getClass() {
    const classMapping = {
        'Sorcerer': 0,
        'Druid': 1,
        'Barbarian': 2,
        'Rogue': 3,
        'Necromancer': 4,
        'Spiritborn': 5
    };
    let cls = document.querySelector('.builder__header__description')?.innerText?.replace(' Build', '') || "Unknown Class";
    return classMapping[cls] ?? null;
}

function getName() {
    let buildName = document.querySelector('#renameBuild')?.value || "Unnamed Build";
    let variantName = document.querySelector('.builder__variant__input')?.value || "Default Variant";
    return `${buildName} - ${variantName}`;
}

try {
    let result = {};

    // Name
    result.Name = getName();

    // Class
    result.D4Class = getClass();

    // Aspects
    result.Aspects = getAllAspects();

    // Armor
    const armorCategories = ['Helm', 'ChestArmor', 'Gloves', 'Pants', 'Boots'];
    armorCategories.forEach(cat => result[cat] = getAllAffixes(cat));

    // Accessories
    const accessoryCategories = ['Amulet', 'Ring1', 'Ring2'];
    accessoryCategories.forEach(cat => result[cat] = getAllAffixes(cat));

    // Weapons
    const weaponCategories = ['Weapon', 'Offhand', 'RangedWeapon', 'BludgeoningWeapon', 'SlashingWeapon', 'WieldWeapon1', 'WieldWeapon2'];
    weaponCategories.forEach(cat => result[cat] = getAllAffixes(cat));

    // Log result for debugging
    //console.debug(JSON.stringify(result, null, 2));
    return JSON.stringify(result, null, 2);
} catch (error) {
    //console.error("An error occurred:", error);
    return JSON.stringify({ error: "An error occurred during processing." });
}
