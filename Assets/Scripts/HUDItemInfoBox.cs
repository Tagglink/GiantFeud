using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDItemInfoBox : MonoBehaviour {

    public Camp playerCamp; // inspector set
    public Giant playerGiant; // inspector set

    int maxResourceGaugeHeight;
    int maxStatGaugeHeight;

    Text itemName;
    Text itemDesc;
    Text consumableEffect;
    Text[] resourceCostValues;
    Text[] statValues;

    Image itemIcon;
    Image[] requiredResourcesGauges;
    Image[] currentResourcesGauges;
    Image[] finalStatGauges;
    Image[] currentStatGauges;

    Color itemGaugeColor;
    Color playerGaugeColor;

	// Use this for initialization
	void Start () {
        Transform equipmentEffectObj = transform.Find("ItemEquipmentEffect");
        Transform resourceCostObj = transform.Find("ResourceCost");

        itemGaugeColor = new Color(0.5f, 0.5f, 0f, 0.5f);
        playerGaugeColor = new Color(0f, 0.5f, 0.5f, 0.5f);

        maxResourceGaugeHeight = 50;
        maxStatGaugeHeight = 74;

        itemIcon = transform.Find("ItemIconFrame/ItemIcon").GetComponent<Image>();

        itemName = transform.Find("ItemName").GetComponent<Text>();
        itemDesc = transform.Find("ItemDescription").GetComponent<Text>();
        resourceCostValues = GetTextValues(transform.Find("ResourceCost"));
        statValues = GetTextValues(transform.Find("ItemEquipmentEffect"));
        consumableEffect = transform.Find("ItemConsumableEffect/EffectDescription").GetComponent<Text>();
        
        currentResourcesGauges = GetGaugeComponents(resourceCostObj, 2);
        requiredResourcesGauges = GetGaugeComponents(resourceCostObj, 3);
        currentStatGauges = GetGaugeComponents(equipmentEffectObj, 2);
        finalStatGauges = GetGaugeComponents(equipmentEffectObj, 3);

        ChangeGaugeColors(currentResourcesGauges, playerGaugeColor);
        ChangeGaugeColors(currentStatGauges, playerGaugeColor);
        ChangeGaugeColors(requiredResourcesGauges, itemGaugeColor);
        ChangeGaugeColors(finalStatGauges, itemGaugeColor);

        enabled = true;
	}

    Text[] GetTextValues(Transform parent)
    {
        Text[] ret = new Text[parent.childCount];

        for (int i = 0; i < ret.Length; i++)
            ret[i] = parent.GetChild(i).GetComponentInChildren<Text>();

        return ret;
    }

    Image[] GetGaugeComponents(Transform parent, int child)
    {
        Image[] ret = new Image[parent.childCount];

        for (int i = 0; i < ret.Length; i++)
            ret[i] = parent.GetChild(i).GetChild(child).GetComponent<Image>();

        return ret;
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

        InjectTexts(resourceCostValues, cost);

        InjectGauges(requiredResourcesGauges, cost, maxResourceGaugeHeight);
        InjectGauges(currentResourcesGauges, playerCamp.resources, maxResourceGaugeHeight);

        if (item is Consumable)
        {
            // consumableEffect.text = ...
        }
        else if (item is Equipment)
        {
            itemStats = (item as Equipment).stats;

            InjectTexts(statValues, itemStats + playerGiant.stats);

            InjectGauges(currentStatGauges, playerGiant.stats, maxStatGaugeHeight);
            InjectGauges(finalStatGauges, itemStats + playerGiant.stats, maxStatGaugeHeight);
        }
    }

    void InjectGauges(Image[] images, Resources resources, int maxHeight)
    {
        images[0].fillAmount = (float)resources.meat / maxHeight;
        images[1].fillAmount = (float)resources.stone / maxHeight;
        images[2].fillAmount = (float)resources.water / maxHeight;
        images[3].fillAmount = (float)resources.wheat / maxHeight;
        images[4].fillAmount = (float)resources.wood / maxHeight;
    }

    void InjectGauges(Image[] images, Stats stats, int maxHeight)
    {
        images[0].fillAmount = (float)stats.atk / maxHeight;
        images[1].fillAmount = (float)stats.def / maxHeight;
        images[2].fillAmount = stats.atkspd / maxHeight;
        images[3].fillAmount = (float)stats.maxHP / maxHeight;
        images[4].fillAmount = (float)stats.hpPerSec / maxHeight;
    }

    void InjectTexts(Text[] texts, Stats stats)
    {
        texts[0].text = "" + stats.atk;
        texts[1].text = "" + stats.def;
        texts[2].text = "" + stats.atkspd;
        texts[3].text = "" + stats.maxHP;
        texts[4].text = "" + stats.hpPerSec;
    }

    void InjectTexts(Text[] texts, Resources resources)
    {
        texts[0].text = "" + resources.meat;
        texts[1].text = "" + resources.stone;
        texts[2].text = "" + resources.water;
        texts[3].text = "" + resources.wheat;
        texts[4].text = "" + resources.wood;
    }
    void ChangeGaugeColors(Image[] gauges, Color color)
    {
        foreach (Image img in gauges)
            img.color = color;
    }
}
