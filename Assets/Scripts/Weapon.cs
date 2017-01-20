using UnityEngine;
using System.Collections;

public class Weapon : Equipment {

	// Hi
    public Weapon(string _name, string _description, Resources _resourceCost, Sprite _icon, int _craftingTime, Stats _stats, int _reinforcementCount, Stats _reinforcementStats)
        : base(_name, _description, _resourceCost, _icon,_craftingTime, _stats, _reinforcementCount, _reinforcementStats)
    {

    }
}
