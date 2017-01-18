using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

    public GameObject targetGiant;

	void Start () {
        StartCoroutine(Wait());
	}

    IEnumerator Wait()
    {
        yield return new WaitForEndOfFrame();
        targetGiant.GetComponent<Giant>().UseItem(Items.itemList[ItemID.MEATSTEW]);
        targetGiant.GetComponent<Giant>().UseItem(Items.itemList[ItemID.BREADLOAF]);
        targetGiant.GetComponent<Giant>().UseItem(Items.itemList[ItemID.ELIXIR]);
    }
}
