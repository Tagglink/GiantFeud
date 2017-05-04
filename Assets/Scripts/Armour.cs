using UnityEngine;
using System.Collections;

/// <summary>
///   The armour generally buffs the Giant's defense
///   and other defensive stats.
/// </summary>
public class Armour : Equipment {
    
    public Armour(string _name, string _description, Resources _resourceCost, Sprite _icon, float _craftingTime, bool _giantUse, Stats _stats, int _reinforcementCount, Stats _reinforcementStats)
        : base(_name, _description, _resourceCost, _icon, _craftingTime, _giantUse, _stats, _reinforcementCount, _reinforcementStats)
    {

    }
    
    /// <summary>
    ///   Copies the given Armour as a new Armour
    /// </summary>
    /// <param name="_armour"></param>
    public Armour(Armour _armour) : base(_armour)
    {

    }
}
