using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

    public GameObject playerGiant;
    public GameObject compGiant;
    public GameObject camp;

	void Start () {
        StartCoroutine(Wait());
        camp.GetComponent<Camp>().resources = new Resources(1, 0, 0, 0, 1);

	}

    IEnumerator Wait()
    {
        yield return new WaitForEndOfFrame();
        //targetGiant.GetComponent<Crafting>().Craft(Items.itemList[ItemID.KNIFE], camp);
        //playerGiant.GetComponent<Giant>().UseItem(Items.itemList[ItemID.AXE]);
        //playerGiant.GetComponent<Giant>().UseItem(Items.itemList[ItemID.ELIXIR]);
        //compGiant.GetComponent<Giant>().UseItem(Items.itemList[ItemID.STONEARMOUR]);
        //compGiant.GetComponent<Giant>().UseItem(Items.itemList[ItemID.SPEAR]);
    }
}
