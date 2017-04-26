using UnityEngine;
using System.Collections;

public class Weapon : Equipment {

    public Sprite weaponSprite { get; set; }

    public Weapon(string _name, string _description, Resources _resourceCost, Sprite _icon, float _craftingTime, bool _giantUse, Stats _stats, int _reinforcementCount, Stats _reinforcementStats)
        : base(_name, _description, _resourceCost, _icon, _craftingTime, _giantUse, _stats, _reinforcementCount, _reinforcementStats)
    {

    }

    public Weapon(Weapon weapon) : base(weapon)
    {
        weaponSprite = weapon.weaponSprite;
    }
}
