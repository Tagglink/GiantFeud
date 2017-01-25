﻿using UnityEngine;
using System.Collections;

public class Item {

    public string name;
    public string description;
    public Resources resourceCost;
    public Sprite icon;
    public float craftingTime;

    public Item(string _name, string _description, Resources _resourceCost, Sprite _icon, float _craftingTime)
    {
        name = _name;
        description = _description;
        resourceCost = _resourceCost;
        icon = _icon;
        craftingTime = _craftingTime;
    }
}