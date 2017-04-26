using UnityEngine;
using System.Collections;

public class Equipment : Item {

    public Stats stats;
    public int reinforcementCount;
    public Stats reinforcementStats;

    public Equipment(string _name, string _description, Resources _resourceCost, Sprite _icon, float _craftingTime, bool _giantUse, Stats _stats, int _reinforcementCount, Stats _reinforcementStats)
        : base(_name, _description, _resourceCost, _icon, _craftingTime, _giantUse)
    {
        stats = _stats;
        reinforcementCount = _reinforcementCount;
        reinforcementStats = _reinforcementStats;
    }

    public Equipment(Equipment _equipment) : base(_equipment.name, _equipment.description, _equipment.resourceCost, _equipment.icon, _equipment.craftingTime, _equipment.giantUse)
    {
        stats = _equipment.stats;
        reinforcementCount = _equipment.reinforcementCount;
        reinforcementStats = _equipment.reinforcementStats;
    }

}
