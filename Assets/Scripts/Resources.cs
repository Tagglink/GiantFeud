using UnityEngine;
using System.Collections;

public enum ResourceType { NONE, WOOD, WHEAT, MEAT, WATER, STONE };

public struct Resources {

    // static methods
    public static ResourceType TileToResource(TileType tile)
    {
        ResourceType ret = ResourceType.NONE;

        switch (tile)
        {
            case TileType.CAMP:
                ret = ResourceType.NONE;
                break;
            case TileType.GIANTS:
                ret = ResourceType.NONE;
                break;
            case TileType.NONE:
                ret = ResourceType.NONE;
                break;
            case TileType.CATTLE:
                ret = ResourceType.MEAT;
                break;
            case TileType.CROPS:
                ret = ResourceType.WHEAT;
                break;
            case TileType.STONE:
                ret = ResourceType.STONE;
                break;
            case TileType.WATER:
                ret = ResourceType.WATER;
                break;
            case TileType.WOODS:
                ret = ResourceType.WOOD;
                break;
        }

        return ret;
    }

    // properties
    public int wood;
    public int wheat;
    public int meat;
    public int water;
    public int stone;

    // regular methods
    public Resources(int _wood, int _wheat, int _meat, int _water, int _stone)
    {
        wood = _wood;
        wheat = _wheat;
        meat = _meat;
        water = _water;
        stone = _stone;
    }

    public void Add(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.NONE:
                break;
            case ResourceType.MEAT:
                meat += amount;
                break;
            case ResourceType.STONE:
                stone += amount;
                break;
            case ResourceType.WATER:
                water += amount;
                break;
            case ResourceType.WHEAT:
                wheat += amount;
                break;
            case ResourceType.WOOD:
                wood += amount;
                break;
        }
    }

    // operators
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

    public static bool operator >=(Resources resources1, int amount)
    {
        return (
            resources1.wood >= amount &&
            resources1.wheat >= amount &&
            resources1.meat >= amount &&
            resources1.water >= amount &&
            resources1.stone >= amount
            );
    }

    public static bool operator <=(Resources resources1, int amount)
    {
        return (
            resources1.wood <= amount &&
            resources1.wheat <= amount &&
            resources1.meat <= amount &&
            resources1.water <= amount &&
            resources1.stone <= amount
            );
    }

    public static Resources operator -(Resources resources1, Resources resources2)
    {
        Resources ret = new Resources();
        ret.wood = resources1.wood - resources2.wood;
        ret.wheat = resources1.wheat - resources2.wheat;
        ret.meat = resources1.meat - resources2.meat;
        ret.water = resources1.water - resources2.water;
        ret.stone = resources1.stone - resources2.stone;
        return ret;
    }


}
