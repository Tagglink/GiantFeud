using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

    public GameObject targetGiant;
    public GameObject camp;

	void Start () {
        StartCoroutine(Wait());
        camp.GetComponent<Camp>().resources = new Resources(1, 0, 0, 0, 1);
	}

    IEnumerator Wait()
    {
        yield return new WaitForEndOfFrame();
        //targetGiant.GetComponent<Crafting>().Craft(Items.itemList[ItemID.KNIFE], camp);
        //targetGiant.GetComponent<Giant>().UseItem(Items.itemList[ItemID.AXE]);
        //targetGiant.GetComponent<Giant>().UseItem(Items.itemList[ItemID.ELIXIR]);
    }
}
