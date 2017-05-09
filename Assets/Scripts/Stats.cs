using UnityEngine;
using System.Collections;

public struct Stats {
    public int atk;
    public float atkspd;
    public int def;
    public int maxHP;
    public int hpPerSec;

    public static Stats operator+(Stats stats1, Stats stats2)
    {
        Stats ret = new Stats();
        ret.atk = stats1.atk + stats2.atk;
        ret.atkspd = stats1.atkspd + stats2.atkspd;
        ret.def = stats1.def + stats2.def;
        ret.maxHP = stats1.maxHP + stats2.maxHP;
        ret.hpPerSec = stats1.hpPerSec + stats2.hpPerSec;
        return ret;
    }

    public static Stats operator -(Stats stats1, Stats stats2)
    {
        Stats ret = new Stats();
        ret.atk = stats1.atk - stats2.atk;
        ret.atkspd = stats1.atkspd - stats2.atkspd;
        ret.def = stats1.def - stats2.def;
        ret.maxHP = stats1.maxHP - stats2.maxHP;
        ret.hpPerSec = stats1.hpPerSec - stats2.hpPerSec;
        if (ret.atkspd < 0)
            ret.atkspd = 0;
        return ret;
    }

    public static Stats operator /(Stats stats1, int number)
    {
        Stats ret = new Stats();
        ret.atk = stats1.atk / number;
        ret.atkspd = stats1.atkspd / number;
        ret.def = stats1.def / number;
        ret.maxHP = stats1.maxHP / number;
        ret.hpPerSec = stats1.hpPerSec / number;
        return ret;
    }

    public static Stats operator *(Stats stats1, int number)
    {
        Stats ret = new Stats();
        ret.atk = stats1.atk * number;
        ret.atkspd = stats1.atkspd * number;
        ret.def = stats1.def * number;
        ret.maxHP = stats1.maxHP * number;
        ret.hpPerSec = stats1.hpPerSec * number;
        return ret;
    }

    public static Stats operator /(Stats stats1, Stats stats2)
    {
        Stats ret = new Stats();
        ret.atk = stats1.atk / stats2.atk;
        ret.atkspd = stats1.atkspd / stats2.atkspd;
        ret.def = stats1.def / stats2.def;
        ret.maxHP = stats1.maxHP / stats2.maxHP;
        ret.hpPerSec = stats1.hpPerSec / stats2.hpPerSec;
        return ret;
    }

    public static Stats operator *(Stats stats1, Stats stats2)
    {
        Stats ret = new Stats();
        ret.atk = stats1.atk * stats2.atk;
        ret.atkspd = stats1.atkspd * stats2.atkspd;
        ret.def = stats1.def * stats2.def;
        ret.maxHP = stats1.maxHP * stats2.maxHP;
        ret.hpPerSec = stats1.hpPerSec * stats2.hpPerSec;
        return ret;
    }

    public Stats(int _atk, float _atkspd, int _def, int _maxHP, int _hpPerSec)
    {
        atk = _atk;
        atkspd = _atkspd;
        def = _def;
        maxHP = _maxHP;
        hpPerSec = _hpPerSec;
    }

    int CheckIfBelowZero(int value)
    {
        if (value < 0)
        {
            return 0;
        }
        else
        {
            return value;
        }
    }
}