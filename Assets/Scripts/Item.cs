using UnityEngine;
using System.Collections;

public class Item {

    public string name;
    public string description;
    public Resources resourceCost;
    public Sprite icon;
    public float craftingTime;
    public bool giantUse; // if false, do not send villager to giant on use

    public Item(string _name, string _description, Resources _resourceCost, Sprite _icon, float _craftingTime, bool _giantUse)
    {
        name = _name;
        description = _description;
        resourceCost = _resourceCost;
        icon = _icon;
        craftingTime = _craftingTime;
        giantUse = _giantUse;
    }
}