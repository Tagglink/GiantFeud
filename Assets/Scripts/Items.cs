using UnityEngine;
using System.Collections.Generic;

public enum ItemID { NULL, SPEAR }

public class Items : MonoBehaviour {

    public static Dictionary <ItemID, Item> itemList;

	void Start () {
        itemList = new Dictionary<ItemID, Item> {
            { ItemID.SPEAR, new Weapon("Spjut", "Stock o sten", new Resources(2, 0, 0, 0, 1), null, new Stats(5, 0.5f, 0, 0, 0, 0, -1), 0, new Stats(2, 0.25f, 0, 0, 0, 0, -1)) }
        };

        Debug.Log("test");
	}
}
