using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDCraftingProgressMeter : MonoBehaviour {

    public Camp playerCamp; // inspector set

    Image progressBar;
    Image progressBarFrame;
    Image itemIcon;
    Text percentageText;

	// Use this for initialization
	void Start () {
        Image[] childImages = GetComponentsInChildren<Image>();
        progressBar = childImages[1];
        itemIcon = childImages[2];
        progressBarFrame = GetComponent<Image>();
        percentageText = GetComponentInChildren<Text>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (playerCamp.isCrafting)
        {
            percentageText.text = (int)(playerCamp.craftingProgress * 100) + "%";
            progressBar.fillAmount = playerCamp.craftingProgress;
        }
	}

    public void Enabled(bool flip)
    {
        percentageText.enabled = flip;
        progressBar.enabled = flip;
        progressBarFrame.enabled = flip;
        itemIcon.enabled = flip;

        if (flip)
        {
            itemIcon.sprite = playerCamp.currentlyCraftingItem.icon;
        }
    }
}
