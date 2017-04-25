using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

public enum displayState { HIDDEN, LERPING, DISPLAYING }

public class HUDItemButton : MonoBehaviour {

    public Camp playerCamp; // inspector set
    public HUDItemInfoBox itemInfoBox; // inspector set

    public ItemID itemID;
    public EventTrigger eventTrigger;

    //private GameObject infoBox;

    // Lerp variables

    public displayState state;
    public Vector3 displayPoint;

    private float lerpTime;
    private float speed;
    private Vector3 hiddenPoint;
    private Vector3 startPoint;
    private Vector3 endPoint;
    private Vector3 startScale;
    private Vector3 endScale;

    private Button button;
    private Image itemImage;
    private Image progressImage;

    void Start () {
        state = displayState.HIDDEN;
        speed = 3;
        //infoBox = transform.GetChild(0).gameObject;
        hiddenPoint = Vector3.zero;
        //infoBox.transform.localScale = hiddenPoint;
        //infoBox.transform.localPosition = Vector3.zero;
        //infoBox.transform.GetChild(0).GetComponent<Text>().text = "<b><i>" + Items.itemList[itemID].name + "</i></b>" + Environment.NewLine + Items.itemList[itemID].description;

        button = GetComponent<Button>();
        eventTrigger = GetComponent<EventTrigger>();

        Image[] images = GetComponentsInChildren<Image>();
        itemImage = images[1];
        progressImage = images[2];

        button.onClick.AddListener(Craft(itemID));
        eventTrigger.enabled = false;
        progressImage.enabled = false;

        // TODO: make it change to 'Reinforce' when an equipment has been crafted once.
    }

    UnityAction Craft(ItemID id)
    {
        return new UnityAction(() => {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(Use(itemID));
            if (playerCamp.Craft(id))
                progressImage.enabled = true;
        });
    }

    UnityAction Use(ItemID id)
    {
        return new UnityAction(() => {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(Craft(itemID));
            if (playerCamp.UseItem(id))
            {
                progressImage.enabled = false;
                progressImage.fillAmount = 0;
            }
        });
    }

	public void Move()
    {
        /*
        StartLerping();
        GetComponentInParent<CraftingButton>().RetractChildren(gameObject);*/

        itemInfoBox.Show(itemID);
    }

    public void StartLerping()
    {
        switch (state)
        {
            case displayState.DISPLAYING:
                endPoint = hiddenPoint;
                startPoint = displayPoint;
                startScale = Vector3.one;
                endScale = Vector3.zero;
                break;
            case displayState.HIDDEN:
                startPoint = hiddenPoint;
                endPoint = displayPoint;
                startScale = Vector3.zero;
                endScale = Vector3.one;
                break;
        }
        state = displayState.LERPING;
    }

    public bool RetractIfLerping()
    {
        if (state == displayState.LERPING)
        {
            //startPoint = infoBox.transform.localPosition;
            endPoint = hiddenPoint;
            //startScale = infoBox.transform.localScale;
            endScale = Vector3.zero;
            return true;
        }
        else
            return false;
    }

    void Update()
    {
        if (progressImage.enabled && playerCamp.isCrafting)
        {
            foreach (KeyValuePair<ItemID, Item> pair in Items.itemList)
            {
                if (pair.Value.name == playerCamp.currentlyCraftingItem.name && pair.Key == itemID)
                    progressImage.fillAmount = playerCamp.craftingProgress;
            }
        }

        if (state == displayState.LERPING)
        {
            lerpTime += Time.deltaTime * speed;
            //infoBox.transform.localPosition = Vector3.Lerp(startPoint, endPoint, 1 - Curve(lerpTime));
            //infoBox.transform.localScale = Vector3.Lerp(startScale, endScale, 1 - Curve(lerpTime));
        }
        if (lerpTime >= 1)
        {
            /*if (infoBox.transform.localPosition == hiddenPoint)
            {
                state = displayState.HIDDEN;
            }
            else
            {
                state = displayState.DISPLAYING;
            }
            lerpTime = 0;*/
        }
    }

    float Curve(float time)
    {
        return (0.7f * time * time) - (1.7f * time) + 1;
    }
}
