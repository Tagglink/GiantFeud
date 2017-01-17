using UnityEngine;
using System.Collections.Generic;

public class Camp : MonoBehaviour {

    public static int MAX_VILLAGERS;

    public List<Item> itemStash;
    public List<GameObject> villagers;
    public Resources resources;

    public int villagerCount;
    public bool isCrafting;
    public float craftingProgress;

	// Use this for initialization
	void Start () {
        MAX_VILLAGERS = 6;
        itemStash = new List<Item>();
        villagers = new List<GameObject>();
        resources = new Resources();
        villagerCount = 2;
        isCrafting = false;
        craftingProgress = 0.0f;
        
        // Villagers should already be instantiated in the scene as children of the camp and out of camera view.
        for (int i = 0; i < MAX_VILLAGERS; i++) 
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
            if (villagers[i].GetComponent<Villager>().state == VillagerState.IDLE)
            {
                ret.Add(villagers[i]);
            }
        }

        return ret;
    }
}
