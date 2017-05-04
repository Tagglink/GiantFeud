using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Camp : MonoBehaviour {

    /// <summary>
    ///   The maximum amount of villagers a player can have
    ///   at one time in the camp.
    /// </summary>
    public static int maxVillagers;

    /// <summary>
    ///   A list representing the items the player has crafted
    ///   but not yet used.
    /// </summary>
    public List<ItemID> itemStash;

    /// <summary>
    ///   A list of the villagers belonging to this camp.
    /// </summary>
    /// <remarks>
    ///   The capacity of the list should always be equal to
    ///   the maximum amount of villagers in the camp.
    /// </remarks>
    /// <see cref="maxVillagers"/> 
    public List<GameObject> villagers;

    /// <summary>
    ///   An object representing the camp's current resources.
    /// </summary>
    public Resources resources;

    /// <summary>
    ///   Is true on the camp which is not the player.
    /// </summary>
    /// <remarks>
    ///   inspector set
    /// </remarks>
    public bool enemyCamp;

    /// <summary>
    ///   The giant belonging to this camp.
    /// </summary>
    /// <remarks>
    ///   inspector set
    /// </remarks>
    public GameObject giant;

    /// <summary>
    ///   The tile to which villagers travel to when
    ///   going back to the camp.
    /// </summary>
    /// <remarks>
    ///   inspector set
    /// </remarks>
    public GameObject homeTile;

    /// <summary>
    ///   The tile to which villagers travel to when leaving
    ///   items to the giant.
    /// </summary>
    /// <remarks>
    ///   inspector set
    /// </remarks>
    public GameObject giantTile;

    /// <summary>
    ///   A reference to the tutorial manager in the HUD.
    /// </summary>
    /// <remarks>
    ///   HUD object
    ///   inspector set
    /// </remarks>
    public HUDTutorial tutorial;
    
    /// <summary>
    ///   A reference to the text object showing the population
    ///   of the village.
    /// </summary>
    /// <remarks>
    ///   HUD object
    ///   inspector set
    /// </remarks>
    public Text populationText;

    /// <summary>
    ///   A reference to the text object showing error messages.
    /// </summary>
    /// <remarks>
    ///   HUD object
    ///   inspector set
    /// </remarks>
    public Text errorText;

    /// <summary>
    ///   A reference to the Image object showing the box
    ///   around the error message.
    /// </summary>
    /// <remarks>
    ///   HUD object
    ///   inspector set
    /// </remarks>
    public Image errorImage;

    /// <summary>
    ///   A reference to the behavior script of the Giant
    ///   that belongs to this camp.
    /// </summary>
    /// <see cref="Giant"/> 
    [HideInInspector]
    public Giant giantScript;

    /// <summary>
    ///   The number of villagers which are currently
    ///   available for use to the player.
    /// </summary>
    public int villagerCount;

    /// <summary>
    ///   Is true if the player is currently crafting.
    /// </summary>
    public bool isCrafting;

    /// <summary>
    ///   A float which moves from 0f --> ~1.0f representing
    ///   the camp's progress with the item being currently crafted.
    /// </summary>
    public float craftingProgress;

    /// <summary>
    ///   The point in time when the last crafting process began,
    ///   expressed in seconds since the game started.
    /// </summary>
    public float craftingStartTime;

    /// <summary>
    ///   A reference to the item currently being crafted.
    /// </summary>
    /// <remarks>
    ///   Is null when the camp is not crafting.
    /// </remarks>
    public Item currentlyCraftingItem;

    /// <summary>
    ///   The coroutine that hides the error box after showing it.
    /// </summary>
    private Coroutine coroutineError;

    /// <summary>
    ///   The amount of time in seconds an error box is shown on the 
    ///   screen before it disappears.
    /// </summary>
    /// <see cref="DisplayError(string)"/> 
    private float errorShowDuration;
    
    // Initialization for MonoBehaviors
    void Start () {
        maxVillagers = 6;
        villagerCount = 2;
        isCrafting = false;
        craftingProgress = 0.0f;
        errorShowDuration = 5.0f;
        giantScript = giant.GetComponent<Giant>();
        itemStash = new List<ItemID>();
        villagers = new List<GameObject>();
        resources = new Resources();

        villagers.Capacity = maxVillagers;

        // This assumes the villagers are already instantiated in the scene
        // as children of the camp.
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

    /// <summary>
    ///   If possible, sends a villager to gather resources at the given tile.
    /// </summary>
    /// <param name="tile">The tile to gather at.</param>
    /// <returns>false if there are no idle villagers to send, true if a villager was sent.</returns>
    public bool SendVillagerToGather(GameObject tile)
    {
        List<GameObject> idleVillagers = GetIdleVillagers();
        Villager villager;

        // if the list of idle villagers is not empty, get the first
        // villager in the list
        if (idleVillagers.Count > 0)
            villager = idleVillagers[0].GetComponent<Villager>();
        else
        {
            // if it is empty, display an error to the player
            DisplayError("Inga bybor är lediga.");
            return false;
        }

        // send the villager to gather
        villager.Gather(tile);
        return true;
    }

    /// <summary>
    ///   If possible, uses an already crafted item with the
    ///   given itemId.
    /// </summary>
    /// <param name="itemId">The ItemID of the item to use.</param>
    /// <returns>false if no available villager is idle, true if the item is used.</returns>
    public bool SendVillagerToUseItem(ItemID itemId)
    {
        List<GameObject> idleVillagers = GetIdleVillagers();
        Villager villager;

        // advance the tutorial if the conditions are met
        if (tutorial.currentStep == 4 && itemId == ItemID.APPLE)
            tutorial.AdvanceTutorial();

        // if the list of idle villagers is not empty, get the first villager
        // and send it to use the item
        if (idleVillagers.Count > 0)
        {
            villager = idleVillagers[0].GetComponent<Villager>();
            villager.UseItem(itemId);
            return true;
        }
        else
        {
            // if the list is empty, show the player an error message.
            DisplayError("Alla bybor är upptagna!");

            return false;
        }
    }

    /// <summary>
    ///   Searches the list of villagers and returns a list of the ones that
    ///   are idle.
    /// </summary>
    /// <returns>
    ///   A list of idle and available villagers.
    /// </returns>
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

    /// <summary>
    ///   Returns a resource object representing the difference in 
    ///   current resources and required resources, including the 
    ///   resources that villagers are currently gathering in the equation.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>
    ///   A resource object where each property is the difference between
    ///   effective current resources and required resources.
    /// </returns>
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

        // check if the camp is already crafting or does not have
        // enough resources
        if (!isCrafting && item.resourceCost <= resources)
        {
            StartCraft(item, itemId);
            return true;
        }
        else if (isCrafting)
        {
            DisplayError("Du craftar redan något!");

            return false;
        }
        else
        {
            DisplayError("Du har inte tillräckligt med resurser för det!");

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

    // returns false if failed
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
                {
                    if (SendVillagerToUseItem(id))
                    {
                        itemStash.RemoveAt(i);
                    }
                    else
                        return false;
                }
                else
                {
                    if (UseItemAtCamp(item))
                        itemStash.RemoveAt(i);
                    else
                        return false;
                }
                
                return true;
            }
        }
        return false;
    }

    bool UseItemAtCamp(Item item)
    {
        if (item is Equipment)
        {
            // error: camp can't use equipments
            return false;
        }

        Consumable cons = item as Consumable;
        cons.action(giantScript);

        StartCoroutine(DelayTemporaryBuff(cons));

        return true;
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

    void DisplayError(string message)
    {
        if (enemyCamp)
            return;

        if (coroutineError != null)
        {
            StopCoroutine(coroutineError);
        }

        coroutineError = StartCoroutine(CoroutineError(message));
    }

    IEnumerator CoroutineError(string message)
    {
        errorText.text = message;
        errorImage.enabled = true;
        yield return new WaitForSeconds(errorShowDuration);
        errorText.text = "";
        errorImage.enabled = false;
    }
}
