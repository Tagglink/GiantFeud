using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
///   The Camp manages villagers and resources, and takes care of
///   crafting.
/// </summary>
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
    /// <returns>
    ///   false if there are no idle villagers to send 
    ///   true if a villager was sent.
    /// </returns>
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
    ///   given ItemId.
    /// </summary>
    /// <param name="itemId">The ItemID of the item to use.</param>
    /// <returns>
    ///   false if no available villager is idle, true if the item is used.
    /// </returns>
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
    ///   current resources and required resources of a specified item, 
    ///   including the resources that this camp's villagers are currently 
    ///   gathering.
    /// </summary>
    /// <param name="item">
    ///   The ItemID of the item to compare with
    /// </param>
    /// <returns>
    ///   A resource object where each property is the difference between
    ///   effective current resources and required resources.
    /// </returns>
    /// <remarks>
    ///   Used by the bot to calculate how much more 
    ///   it should gather.
    /// </remarks>
    /// <see cref="AI"/>
    /// <see cref="Villager.efficiency"/>
    /// <see cref="Resources"/>
    public Resources CalculateRequiredResources(ItemID item)
    {
        Resources itemCost = Items.itemList[item].resourceCost;
        Resources effectiveResources = resources;
        Resources ret = new Resources();

        // create the effectiveResources by looking at what
        // the villagers are currently gathering.
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

        // subtract the item cost with the effective resources to
        // get the return value.
        ret = itemCost - effectiveResources;

        return ret;
    }

    /// <summary>
    ///   If possible, craft an item.
    /// </summary>
    /// <param name="itemId">
    ///   The ItemID of the item to craft
    /// </param>
    /// <returns>
    ///   false if there are not enough resources, or
    ///   if a craft is already underway. 
    ///   true if the crafting is started.
    /// </returns>
    /// <see cref="isCrafting"/>
    /// <see cref="StartCraft(Item, ItemID)"/>
    /// <see cref="resources"/>
    public bool Craft(ItemID itemId)
    {
        Item item = Items.itemList[itemId];

        // check so that the camp isn't already crafting
        // and has enough resources
        if (!isCrafting && item.resourceCost <= resources)
        {
            // if so, start the crafting
            StartCraft(item, itemId);
            return true;
        }
        else if (isCrafting)
        {
            // if there was already a craft underway, tell the player
            DisplayError("Du craftar redan något!");

            return false;
        }
        else
        {
            // if there wasn't enough resources, tell the player.
            DisplayError("Du har inte tillräckligt med resurser för det!");

            return false;
        }
    }

    /// <summary>
    ///   Start a crafting process.
    /// </summary>
    /// <param name="item">The Item object to craft</param>
    /// <param name="itemId">The ItemID of the item</param>
    /// <see cref="craftingStartTime"/>
    /// <see cref="currentlyCraftingItem"/>
    /// <see cref="WaitForCraft(ItemID, Item)"/>
    void StartCraft(Item item, ItemID itemId)
    {
        // prepare the crafting variables
        craftingStartTime = Time.realtimeSinceStartup;
        isCrafting = true;
        currentlyCraftingItem = item;

        // pay the resources
        resources -= item.resourceCost;

        // start the crafting
        StartCoroutine(WaitForCraft(itemId, item));
    }

    /// <summary>
    ///   If possible, uses an already crafted item.
    /// </summary>
    /// <param name="id">
    ///   The ItemID of the item to be used.
    /// </param>
    /// <returns>
    ///   false if failure, true if successful use of item
    /// </returns>
    /// <remarks>
    ///   The function will return false in 3 cases:
    ///     1. The item is not in the item stash.
    ///     2. The item is giantUse and there is not enough 
    ///        idle villagers to go and give the item to the giant.
    ///     3. The function tried to use something other
    ///        than a Consumable as non-giantUse.
    /// </remarks>
    /// <see cref="Item.giantUse"/>
    /// <see cref="itemStash"/>
    public bool UseItem(ItemID id)
    {
        ItemID currentItem;
        Item item;
        int i;
        
        // loop through the item stash to look for the item
        for (i = 0; i < itemStash.Count; i++) { 

            currentItem = itemStash[i];

            if (currentItem == id)
            {
                // if found, get the proper Item object from the
                // static item list
                item = Items.itemList[id];
                if (item.giantUse)
                {
                    // if the item should be given to the giant, 
                    // try to send a non-idle villager to do so.
                    if (SendVillagerToUseItem(id))
                    {
                        itemStash.RemoveAt(i);
                    }
                    else
                    {
                        // if it fails, return false
                        return false;
                    }
                }
                else
                {
                    // if the item should be used at the camp,
                    // try to use the item
                    if (UseItemAtCamp(item))
                    {
                        itemStash.RemoveAt(i);
                    }
                    else
                    {
                        // and return false if it failed
                        return false;
                    }
                }
                
                // return true if the item was found and no
                // errors were encountered during the usage
                return true;
            }
        }
        
        // return false if the item was not found
        // in the item stash
        return false;
    }

    /// <summary>
    ///   Use an item right away.
    /// </summary>
    /// <param name="item">
    ///   The Item object to use.
    /// </param>
    /// <returns>
    ///   false if the Item is an Equipment (the item is not used),
    ///   true if the Item is a Consumable (the item is used)
    /// </returns>
    /// <see cref="Consumable"/>
    /// <see cref="Equipment"/>
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

    /// <summary>
    ///   Uses a Consumable's reverseAction after the consumable's
    ///   duration.
    /// </summary>
    /// <param name="consumable">
    ///   The Consumable that is used
    /// </param>
    /// <remarks>
    ///   Should be called by StartCoroutine()
    /// </remarks>
    /// <see cref="Consumable.duration"/>
    IEnumerator DelayTemporaryBuff(Consumable consumable)
    {
        yield return new WaitForSeconds(consumable.duration);
        consumable.reverseAction(giantScript);
    }

    /// <summary>
    ///   Waits for the item's craftingTime and then adds the item
    ///   to the item stash and resets the crafting process.
    /// </summary>
    /// <param name="id">The id of the item to be crafted</param>
    /// <param name="item">The Item object to be crafted</param>
    /// <remarks>
    ///   Contains a tutorial advancement
    /// </remarks>
    IEnumerator WaitForCraft(ItemID id, Item item)
    {
        yield return new WaitForSeconds(item.craftingTime);

        // after the crafting time, add the item to the item stash
        itemStash.Add(id);

        // and reset the crafting variables
        isCrafting = false;
        currentlyCraftingItem = null;
        craftingProgress = 0.0f;

        // if the correct conditions are met, advance the tutorial at this time.
        if (!enemyCamp && tutorial.currentStep == 3 && id == ItemID.APPLE)
            tutorial.AdvanceTutorial();
    }

    /// <summary>
    ///   Show the player an error.
    /// </summary>
    /// <param name="message">
    ///   The contents of the error box.
    /// </param>
    /// <see cref="coroutineError"/>
    void DisplayError(string message)
    {
        // if the enemy camp triggers an error, ignore it
        if (enemyCamp)
            return;
        
        if (coroutineError != null)
        {
            // If there is an error currently showing, stop that coroutine
            StopCoroutine(coroutineError);
        }

        // Show the error and save the coroutine so that it can be
        // cancelled.
        coroutineError = StartCoroutine(CoroutineError(message));
    }

    /// <summary>
    ///   The coroutine that shows an error to the player, 
    ///   then waits for a certain duration and terminates the error.
    /// </summary>
    /// <param name="message">
    ///   The contents of the error box.
    /// </param>
    /// <see cref="errorShowDuration"/>
    IEnumerator CoroutineError(string message)
    {
        errorText.text = message;
        errorImage.enabled = true;
        yield return new WaitForSeconds(errorShowDuration);
        errorText.text = "";
        errorImage.enabled = false;
    }
}
