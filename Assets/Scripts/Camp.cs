using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Camp : MonoBehaviour {

    public static int maxVillagers;

    public List<ItemID> itemStash;
    public List<GameObject> villagers;
    public Resources resources;

    public GameObject giant; // inspector set
    public GameObject homeTile; // inspector set
    public GameObject giantTile; // inspector set
    public HUDTutorial tutorial; // inspector set
    public Text populationText; // inspector set
    public bool enemyCamp; // inspector set

    [HideInInspector]
    public Giant giantScript;

    public int villagerCount; // number of unlocked villagers
    public bool isCrafting;
    public float craftingProgress;

    public float craftingStartTime;
    public Item currentlyCraftingItem;

    // Use this for initialization
    void Start () {
        maxVillagers = 6;
        itemStash = new List<ItemID>();
        villagers = new List<GameObject>();
        resources = new Resources();
        villagerCount = 2;
        isCrafting = false;
        craftingProgress = 0.0f;
        giantScript = giant.GetComponent<Giant>();
        
        // Villagers should already be instantiated in the scene as children of the camp and out of camera view.
        for (int i = 0; i < maxVillagers; i++) 
        {
            villagers.Add(transform.GetChild(i).gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
	    if (isCrafting)
        {
            craftingProgress = (Time.realtimeSinceStartup - craftingStartTime) / currentlyCraftingItem.craftingTime;
        }
	}

    public bool SendVillagerToGather(GameObject tile)
    {
        List<GameObject> idleVillagers = GetIdleVillagers();
        Villager villager;

        if (idleVillagers.Count > 0)
            villager = idleVillagers[0].GetComponent<Villager>();
        else
        {
            // TODO: Give the player some error message like "You don't have enough idle villagers for that!"
            // (do not send if it's the opponent's camp)
            return false;
        }

        villager.Gather(tile);
        return true;
    }

    public bool SendVillagerToUseItem(ItemID itemId)
    {
        List<GameObject> idleVillagers = GetIdleVillagers();
        Villager villager;

        if (tutorial.currentStep == 4 && itemId == ItemID.APPLE)
            tutorial.AdvanceTutorial();

        if (idleVillagers.Count > 0)
        {
            villager = idleVillagers[0].GetComponent<Villager>();
            villager.UseItem(itemId);
            return true;
        }
        else
        {
            // TODO: Give the player some error message like "You don't have enough idle villagers for that!"
            // (do not send if it's the opponent's camp)
            return false;
        }
    }

    public List<GameObject> GetIdleVillagers()
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
                    effectiveResources.meat += v.efficiency;
                    break;
                case ResourceType.STONE:
                    effectiveResources.stone += v.efficiency;
                    break;
                case ResourceType.WATER:
                    effectiveResources.water += v.efficiency;
                    break;
                case ResourceType.WHEAT:
                    effectiveResources.wheat += v.efficiency;
                    break;
                case ResourceType.WOOD:
                    effectiveResources.wood += v.efficiency;
                    break;
            }
        }

        ret = itemCost - effectiveResources;

        return ret;
    }

    public bool Craft(ItemID itemId)
    {
        Item item = Items.itemList[itemId];

        if (!isCrafting && item.resourceCost <= resources)
        {
            StartCraft(item, itemId);
            return true;
        }
        else
        {
            // TODO: show the player some error message like "You don't have enough resources to do that!"
            return false;
        }
    }

    void StartCraft(Item item, ItemID itemId)
    {
        craftingStartTime = Time.realtimeSinceStartup;
        isCrafting = true;
        resources -= item.resourceCost;
        currentlyCraftingItem = item;

        StartCoroutine(WaitForCraft(itemId, item));
    }

    // returns false if item was not found in the item stash
    public bool UseItem(ItemID id)
    {
        ItemID currentItem;
        Item item;
        int i;

        for (i = 0; i < itemStash.Count; i++) { 

            currentItem = itemStash[i];

            if (currentItem == id)
            {
                item = Items.itemList[id];
                if (item.giantUse)
                    SendVillagerToUseItem(id);
                else
                    UseItemAtCamp(item);

                itemStash.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    void UseItemAtCamp(Item item)
    {
        if (item is Equipment)
        {
            // error: camp can't use equipments
            return;
        }

        Consumable cons = item as Consumable;
        cons.action(giantScript);

        StartCoroutine(DelayTemporaryBuff(cons));
    }

    IEnumerator DelayTemporaryBuff(Consumable consumable)
    {
        yield return new WaitForSeconds(consumable.craftingTime);
        consumable.reverseAction(giantScript);
    }

    IEnumerator WaitForCraft(ItemID id, Item item)
    {
        yield return new WaitForSeconds(item.craftingTime);
        itemStash.Add(id);
        isCrafting = false;
        currentlyCraftingItem = null;
        craftingProgress = 0.0f;

        if (!enemyCamp && tutorial.currentStep == 3 && id == ItemID.APPLE)
            tutorial.AdvanceTutorial();
    }
}
