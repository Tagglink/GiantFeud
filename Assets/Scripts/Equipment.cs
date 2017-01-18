﻿using UnityEngine;
using System.Collections;

public class Equipment : Item {

    public Stats stats;
    public int reinforcementCount;
    public Stats reinforcementStats;

    public Equipment(string _name, string _description, Resources _resourceCost, Sprite _icon, Stats _stats, int _reinforcementCount, Stats _reinforcementStats)
        : base(_name, _description, _resourceCost, _icon)
    {
        stats = _stats;
        reinforcementCount = _reinforcementCount;
        reinforcementStats = _reinforcementStats;
    }

}