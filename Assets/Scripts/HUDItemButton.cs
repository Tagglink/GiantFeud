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
    private Image highlightImage;

    void Start () {
        state = displayState.HIDDEN;
        speed = 3;
        hiddenPoint = Vector3.zero;

        button = GetComponent<Button>();
        eventTrigger = GetComponent<EventTrigger>();

        Image[] images = GetComponentsInChildren<Image>();
        highlightImage = images[1];
        itemImage = images[2];
        progressImage = images[3];

        button.onClick.AddListener(Craft(itemID));
        highlightImage.enabled = false;
        eventTrigger.enabled = false;
        progressImage.enabled = false;
    }

    /// <summary>
    /// Craft an item.
    /// </summary>
    /// <param name="id">The item id to craft</param>
    /// <returns></returns>
    UnityAction Craft(ItemID id)
    {
        return new UnityAction(() => {
            if (playerCamp.Craft(id))
            {
                progressImage.enabled = true;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(Use(itemID));
            }
        });
    }

    /// <summary>
    /// Use a crafted item.
    /// </summary>
    /// <param name="id">The item id to use</param>
    /// <returns></returns>
    UnityAction Use(ItemID id)
    {
        return new UnityAction(() => {
            if (playerCamp.UseItem(id))
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(Craft(itemID));
                progressImage.enabled = false;
                progressImage.fillAmount = 0;
            }
        });
    }

    /// <summary>
    /// Called when the mouse hovers over this itembutton.
    /// Shows the item information in the itemInfoBox.
    /// </summary>
	public void Move()
    {
        itemInfoBox.Show(itemID);
    }

    /// <summary>
    /// Start moving inwards or outwards depending on the state
    /// </summary>
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

    /// <summary>
    /// Sets the endPoint and endScale to the rectract values if the current state is LERPING.
    /// </summary>
    /// <returns>returns true if the current state is LERPING, otherwise false</returns>
    public bool RetractIfLerping()
    {
        if (state == displayState.LERPING)
        {
            endPoint = hiddenPoint;
            endScale = Vector3.zero;
            return true;
        }
        else
            return false;
    }

    void Update()
    {
        if (playerCamp.resources >= Items.itemList[itemID].resourceCost)
            highlightImage.enabled = true;
        else
            highlightImage.enabled = false;
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
        }
    }
}
