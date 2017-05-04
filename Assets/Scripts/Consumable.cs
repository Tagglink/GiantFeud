using UnityEngine;
using System.Collections;
using System;

/// <summary>
///   The Consumable is an item with a special effect.
///   For example multiplying all stats by 2,
///   healing the Giant for x hp, or adding
///   another villager available to gather for you.
/// </summary>
/// <see cref="Item"/>
public class Consumable : Item {
    
    /// <summary>
    ///   The duration that the effect lasts.
    ///   Set to 0f if the effect is permanent.
    /// </summary>
    public float duration;

    /// <summary>
    ///   The function that applies the effects of the consumable
    ///   the instant it is used.
    /// </summary>
    public Action<Giant> action;

    /// <summary>
    ///   The function that applies the anti-effects of the consumable
    ///   when the duration has passed.
    /// </summary>
    public Action<Giant> reverseAction;
    
    // Constructor
    public Consumable(string _name, string _description, Resources _resourceCost, Sprite _icon, float _craftingTime, bool _giantUse, float _duration, Action<Giant> _action, Action<Giant> _reverseAction) : 
        base(_name, _description, _resourceCost, _icon, _craftingTime, _giantUse)
    {
        duration = _duration;
        action = _action;
        reverseAction = _reverseAction;
    }

}
