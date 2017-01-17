using UnityEngine;
using System.Collections;

public enum ResourceType { NONE, WOOD, WHEAT, MEAT, WATER, STONE };

public struct Resources {

    public int wood;
    public int wheat;
    public int meat;
    public int water;
    public int stone;

    public Resources(int _wood, int _wheat, int _meat, int _water, int _stone)
    {
        wood = _wood;
        wheat = _wheat;
        meat = _meat;
        water = _water;
        stone = _stone;
    }

}
