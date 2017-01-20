using UnityEngine;
using System.Collections.Generic;

public class Camp : MonoBehaviour {

    public static int maxVillagers;

    public List<Item> itemStash;
    public List<GameObject> villagers;
    public Resources resources;

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

    public void SendVillagerTo(GameObject tile)
    {
        List<GameObject> idleVillagers = GetIdleVillagers();
        Villager villager;

        if (idleVillagers.Count > 0)
            villager = idleVillagers[0].GetComponent<Villager>();
        else
            return; // if no villagers are idle, return

        villager.Gather(tile);
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

    public void AddResource(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.NONE:
                break;
            case ResourceType.MEAT:
                resources.meat += amount;
                break;
            case ResourceType.STONE:
                resources.stone += amount;
                break;
            case ResourceType.WATER:
                resources.water += amount;
                break;
            case ResourceType.WHEAT:
                resources.wheat += amount;
                break;
            case ResourceType.WOOD:
                resources.wood += amount;
                break;
        }
    }
}
