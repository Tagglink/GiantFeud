using UnityEngine;
using System.Collections;
using System;

public class Consumable : Item {

    public Stats stats;
    public float duration;
    public Action<Giant> action;
    public Action<Giant> reverseAction;

    public Consumable(string _name, string _description, Resources _resourceCost, Sprite _icon, float _craftingTime, Stats _stats, float _duration, Action<Giant> _action, Action<Giant> _reverseAction) : 
        base(_name, _description, _resourceCost, _icon, _craftingTime)
    {
        stats = _stats;
        duration = _duration;
        action = _action;
        reverseAction = _reverseAction;
    }

}
