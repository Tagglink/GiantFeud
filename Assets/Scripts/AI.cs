using UnityEngine;
using System.Collections.Generic;

public enum AIAction { NONE, BUILD_PATH, HEAL }

public class AI : MonoBehaviour {

    public Giant giant; // inspector set
    public Camp camp; // inspector set
    public bool active;

    private ItemID goalItem;
    private AIAction action;
    private bool craftFlag;

    private ItemID[] buildPath;
    private int buildPathIndex;

    void Start()
    {
        goalItem = ItemID.NULL;
        action = AIAction.NONE;
        craftFlag = false;
        active = false;
        buildPathIndex = 0;

        buildPath = new ItemID[]
        {
            ItemID.SPEAR, ItemID.LEATHERARMOUR, ItemID.AXE, ItemID.SWORD, ItemID.STONEARMOUR, ItemID.HIDEARMOUR
        };
    }

	void Update () {
        if (active)
        {
            action = DetermineAction();
            DetermineGoalItem();

            if (craftFlag)
            {
                // attempt to craft goalItem until success
                craftFlag = !camp.Craft(goalItem);
                if (!craftFlag)
                {
                    goalItem = ItemID.NULL; // if success, set item back to null
                    if (action == AIAction.BUILD_PATH && buildPathIndex < buildPath.Length - 1)
                        buildPathIndex++;
                }
            }
            else
            {
                // craftFlag is true once enough villagers have been sent for goalItem's cost
                craftFlag = GatherRequiredResources();
            }

            // if there is an item in the stash, use it right away
            if (camp.itemStash.Count > 0)
                camp.UseItem(camp.itemStash[0]);
        }
    }

    void DetermineGoalItem()
    {
        switch (action)
        {
            case AIAction.HEAL:
                BuildHeal();
                break;
            case AIAction.BUILD_PATH:
                BuildNextOnPath();
                break;
            case AIAction.NONE:
                // do nothing!
                break;
            default:
                break;
        }
    }

    void BuildHeal()
    {
        goalItem = ItemID.MEATSTEW;
    }

    void BuildNextOnPath()
    {
        goalItem = buildPath[buildPathIndex];
    }

    AIAction DetermineAction()
    {
        if (giant.stats.hp < giant.stats.maxHP / 2)
        {
            return AIAction.HEAL;
        }
        else
        {
            return AIAction.BUILD_PATH;
        }
    }

    bool GatherRequiredResources()
    {
        Resources requiredResources = camp.CalculateRequiredResources(goalItem);
        int idleVillagerCount = camp.GetIdleVillagers().Count;
        ResourceType resourceToGather;
        GameObject targetTile;

        while (!(requiredResources <= 0) && idleVillagerCount > 0)
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
}
