using UnityEngine;
using System.Collections;

public class Consumable : Item {

    public Stats stats;

    public Consumable(string _name, string _description, Resources _resourceCost, Sprite _icon, Stats _stats) : base(_name, _description, _resourceCost, _icon)
    {
        stats = _stats;
    }

}
