using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDItemInfoBox : MonoBehaviour {

    public Camp playerCamp; // inspector set
    public Giant playerGiant; // inspector set
    public HUDGauge[] resourceGauges; // inspector set
    public HUDGauge[] statGauges; // inspector set

    public Text itemName; // inspector set
    public Text itemDesc; // inspector set
    public Text consumableEffect; // inspector set
    public Image itemImage; // inspector set
    
    Color itemGaugeColor;
    Color playerGaugeColor;

	// Use this for initialization
	void Start () {
        itemGaugeColor = new Color(0.5f, 0.5f, 0f, 0.5f);
        playerGaugeColor = new Color(0f, 0.5f, 0.5f, 0.5f);

        ChangeGaugeColors(resourceGauges, playerGaugeColor, 0);
        ChangeGaugeColors(resourceGauges, itemGaugeColor, 1);

        ChangeGaugeColors(statGauges, playerGaugeColor, 0);
        ChangeGaugeColors(statGauges, itemGaugeColor, 1);

        enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Show(ItemID itemID)
    {
        Item item = Items.itemList[itemID];

        InjectValues(item);

        enabled = true;
    }

    public void Hide()
    {
        enabled = false;
    }

    void InjectValues(Item item)
    {
        Resources cost = item.resourceCost;
        Stats itemStats;

        itemName.text = item.name;
        itemDesc.text = item.description;

        itemImage.sprite = item.icon;

        resourceGauges[0].SetValue(playerCamp.resources.meat, 0);
        resourceGauges[1].SetValue(playerCamp.resources.stone, 0);
        resourceGauges[2].SetValue(playerCamp.resources.water, 0);
        resourceGauges[3].SetValue(playerCamp.resources.wheat, 0);
        resourceGauges[4].SetValue(playerCamp.resources.wood, 0);
        
        resourceGauges[0].SetValue(cost.meat, 1, true);
        resourceGauges[1].SetValue(cost.stone, 1, true);
        resourceGauges[2].SetValue(cost.water, 1, true);
        resourceGauges[3].SetValue(cost.wheat, 1, true);
        resourceGauges[4].SetValue(cost.wood, 1, true);

        if (item is Consumable)
        {
            // consumableEffect.text = ...
        }
        else if (item is Equipment)
        {
            itemStats = (item as Equipment).stats;
            
            statGauges[0].SetValue(playerGiant.stats.atk, 0);
            statGauges[1].SetValue(playerGiant.stats.atkspd, 0);
            statGauges[2].SetValue(playerGiant.stats.def, 0);
            statGauges[3].SetValue(playerGiant.stats.maxHP, 0);
            statGauges[4].SetValue(playerGiant.stats.hpPerSec, 0);

            statGauges[0].SetValue(itemStats.atk, 1, true);
            statGauges[1].SetValue(itemStats.atkspd, 1, true);
            statGauges[2].SetValue(itemStats.def, 1, true);
            statGauges[3].SetValue(itemStats.maxHP, 1, true);
            statGauges[4].SetValue(itemStats.hpPerSec, 1, true);
        }
    }

    void ChangeGaugeColors(HUDGauge[] gauges, Color color, int index)
    {
        foreach (HUDGauge gauge in gauges)
            gauge.SetColor(color, index);
    }
}
