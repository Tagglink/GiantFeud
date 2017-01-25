using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ItemButton : MonoBehaviour {

    public ItemID itemID;
    private GameObject infoBox;

	void Start () {
        GameObject playerGiant = GameObject.Find("Giant Player");
        infoBox = transform.GetChild(0).gameObject;
        infoBox.transform.localScale = Vector3.zero;
        infoBox.transform.GetChild(0).GetComponent<Text>().text = "<b><i>" + Items.itemList[itemID].name + "</i></b>" + Environment.NewLine + Items.itemList[itemID].description;
        infoBox.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate () { playerGiant.GetComponent<Giant>().camp.GetComponent<Camp>().Craft(itemID); });
	}

	public void ScaleUp()
    {
        infoBox.transform.localScale = Vector3.one;
    }
}
