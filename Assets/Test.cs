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
        targetGiant.GetComponent<Giant>().UseItem(Items.itemList[ItemID.ASGARDMEAL]);
        //targetGiant.GetComponent<Giant>().UseItem(Items.itemList[ItemID.AXE]);
        //targetGiant.GetComponent<Giant>().UseItem(Items.itemList[ItemID.ELIXIR]);
    }
}
