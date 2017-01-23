using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour {

    public Camp camp;

    private ItemID goalItem;


    void Start()
    {
        
    }

	void Update () {
        if (goalItem == ItemID.NULL)
        {
            GatherRequiredResources();
        }
        else
        {
            DetermineGoalItem();
        }
	}

    void GatherRequiredResources()
    {
        Resources requiredResources = camp.CalculateRequiredResources(goalItem);
        ResourceType resourceToGather = ResourceType.NONE;
        GameObject targetTile;
        
        while (requiredResources >= 1)
        {
            // TODO: determine resourceToGather

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
                camp.SendVillagerToGather(targetTile);
        }
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

    void DetermineGoalItem()
    {

    }

    
}
