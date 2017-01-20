using UnityEngine;
using System.Collections;

public enum ResourceType { NONE, WOOD, WHEAT, MEAT, WATER, STONE };

public struct Resources {

    public int wood;
    public int wheat;
    public int meat;
    public int water;
    public int stone;

    public static bool operator >=(Resources resources1, Resources resources2)
    {
        return (
            resources1.wood >= resources2.wood &&
            resources1.wheat >= resources2.wheat &&
            resources1.meat >= resources2.meat &&
            resources1.water >= resources2.water &&
            resources1.stone >= resources2.stone
            );
    }

    public static bool operator <=(Resources resources1, Resources resources2)
    {
        return (
            resources1.wood <= resources2.wood &&
            resources1.wheat <= resources2.wheat &&
            resources1.meat <= resources2.meat &&
            resources1.water <= resources2.water &&
            resources1.stone <= resources2.stone
            );
    }

    public Resources(int _wood, int _wheat, int _meat, int _water, int _stone)
    {
        wood = _wood;
        wheat = _wheat;
        meat = _meat;
        water = _water;
        stone = _stone;
    }

}
