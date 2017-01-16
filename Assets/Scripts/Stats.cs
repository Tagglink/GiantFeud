using UnityEngine;
using System.Collections;

public struct Stats {
    public int atk;
    public float atkspd;
    public int def;
    public int maxHP;
    public int hp;
    public int duration;

    public static Stats operator+(Stats stats1, Stats stats2)
    {
        Stats ret = new Stats();
        ret.atk = stats1.atk + stats2.atk;
        ret.atkspd = stats1.atkspd + stats2.atkspd;
        ret.def = stats1.def + stats2.def;
        ret.maxHP = stats1.maxHP + stats2.maxHP;
        ret.hp = stats1.hp + stats2.hp;
        ret.duration = stats1.duration + stats2.duration;
        return ret;
    }
}