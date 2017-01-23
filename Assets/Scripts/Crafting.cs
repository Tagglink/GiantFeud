using UnityEngine;
using System.Collections;

public class Crafting : MonoBehaviour {
    
    public void CraftFromPlayer(int id)
    {
        ItemID itemID = (ItemID)id;
        Item item = Items.itemList[itemID];
        GameObject camp = GameObject.Find("Base Player");
        Craft(item, camp);
    }

    public void Craft(Item item, GameObject camp)
    {
        Camp player = camp.GetComponent<Camp>();
        if (CheckRequiredResources(item, player) && !player.isCrafting)
        {
            player.isCrafting = true;
            StartCoroutine(WaitForCraftFinish(item, player));
        }
    }

    bool CheckRequiredResources(Item item, Camp player)
    {
        return player.resources >= item.resourceCost;
    }

    IEnumerator WaitForCraftFinish(Item item, Camp player)
    {
        yield return new WaitForSeconds(item.craftingTime);
        player.giant.GetComponent<Giant>().UseItem(item);
        player.isCrafting = false;
    }
}
