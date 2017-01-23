using UnityEngine;
using System.Collections.Generic;

public class Camp : MonoBehaviour {

    public static int maxVillagers;

    public List<Item> itemStash;
    public List<GameObject> villagers;
    public Resources resources;

    public GameObject giant;
    public GameObject homeTile; // set by inspector

    public int villagerCount;
    public bool isCrafting;
    public float craftingProgress;

	// Use this for initialization
	void Start () {
        maxVillagers = 6;
        itemStash = new List<Item>();
        villagers = new List<GameObject>();
        resources = new Resources();
        villagerCount = 2;
        isCrafting = false;
        craftingProgress = 0.0f;
        
        // Villagers should already be instantiated in the scene as children of the camp and out of camera view.
        for (int i = 0; i < maxVillagers; i++) 
        {
            villagers.Add(transform.GetChild(i).gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool SendVillagerToGather(GameObject tile)
    {
        List<GameObject> idleVillagers = GetIdleVillagers();
        Villager villager;

        if (idleVillagers.Count > 0)
            villager = idleVillagers[0].GetComponent<Villager>();
        else
            return false; // if no villagers are idle, return false

        villager.Gather(tile);
        return true;
    }

    List<GameObject> GetIdleVillagers()
    {
        List<GameObject> ret = new List<GameObject>();
        for (int i = 0; i < villagerCount; i++)
        {
            Villager v = villagers[i].GetComponent<Villager>();
            if (v.state == VillagerState.IDLE)
            {
                ret.Add(villagers[i]);
            }
        }

        return ret;
    }

    public Resources CalculateRequiredResources(ItemID item)
    {
        Resources itemCost = Items.itemList[item].resourceCost;
        Resources effectiveResources = resources;
        Resources ret = new Resources();

        foreach (GameObject vObj in villagers)
        {
            Villager v = vObj.GetComponent<Villager>();
            ResourceType vResource = v.resource;
            switch (vResource)
            {
                case ResourceType.MEAT:
                    effectiveResources.meat += v.resourcesCarried;
                    break;
                case ResourceType.STONE:
                    effectiveResources.stone += v.resourcesCarried;
                    break;
                case ResourceType.WATER:
                    effectiveResources.water += v.resourcesCarried;
                    break;
                case ResourceType.WHEAT:
                    effectiveResources.wheat += v.resourcesCarried;
                    break;
                case ResourceType.WOOD:
                    effectiveResources.wood += v.resourcesCarried;
                    break;
            }
        }

        ret = itemCost - effectiveResources;

        return ret;
    }
}
