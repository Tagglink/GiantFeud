using UnityEngine;
using System.Collections.Generic;

public class AI : MonoBehaviour {

    public Camp camp;

    private ItemID goalItem;
    private bool craftFlag;

    void Start()
    {
        goalItem = ItemID.NULL;
        craftFlag = false;
    }

	void Update () {
        if (goalItem == ItemID.NULL)
        {
            DetermineGoalItem();
        }
        else
        {
            if (craftFlag)
            {
                // attempt to craft goalItem until success
                craftFlag = !camp.Craft(goalItem);
            }
            else
            {
                // craftFlag is true once enough villagers have been sent for goalItem's cost
                craftFlag = GatherRequiredResources();
            }
        }
	}

    bool GatherRequiredResources()
    {
        Resources requiredResources = camp.CalculateRequiredResources(goalItem);
        int idleVillagerCount = camp.GetIdleVillagers().Count;
        ResourceType resourceToGather;
        GameObject targetTile;

        while (requiredResources >= 1 && idleVillagerCount > 0)
        {
            resourceToGather = GetFirstResourceOverZero(requiredResources);

            switch (resourceToGather)
            {
                case ResourceType.MEAT:
                    targetTile = FindHarvestableTile(TileType.CATTLE);
                    requiredResources.meat--;
                    break;
                case ResourceType.STONE:
                    targetTile = FindHarvestableTile(TileType.STONE);
                    requiredResources.stone--;
                    break;
                case ResourceType.WATER:
                    targetTile = FindHarvestableTile(TileType.WATER);
                    requiredResources.water--;
                    break;
                case ResourceType.WHEAT:
                    targetTile = FindHarvestableTile(TileType.CROPS);
                    requiredResources.wheat--;
                    break;
                case ResourceType.WOOD:
                    targetTile = FindHarvestableTile(TileType.WOODS);
                    requiredResources.wood--;
                    break;
                case ResourceType.NONE:
                    targetTile = null;
                    break;
                default:
                    targetTile = null;
                    break;
            }

            if (targetTile)
            {
                camp.SendVillagerToGather(targetTile);
                idleVillagerCount--;
            }
        }

        // if enough villagers were sent to satisfy the required resources, return true
        return requiredResources <= 0;
    }

    GameObject FindHarvestableTile(TileType type)
    {
        foreach (GameObject[] tileArr in Map.tiles)
        {
            for (int i = 0; i < tileArr.Length; i++)
            {
                GameObject tile = tileArr[tileArr.Length - i - 1];
                Tile tileScript = tile.GetComponent<Tile>();

                if (tileScript.type == type && tileScript.state == TileState.READY && !tileScript.occupied)
                {
                    return tile;
                }
            }
        }

        return null;
    }

    ResourceType GetFirstResourceOverZero(Resources res)
    {
        if (res.meat > 0)
            return ResourceType.MEAT;
        else if (res.stone > 0)
            return ResourceType.STONE;
        else if (res.water > 0)
            return ResourceType.WATER;
        else if (res.wheat > 0)
            return ResourceType.WHEAT;
        else if (res.wood > 0)
            return ResourceType.WOOD;

        return ResourceType.NONE;
    }

    void DetermineGoalItem()
    {

    }
}
