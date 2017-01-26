using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ItemButton : MonoBehaviour {

    public ItemID itemID;
    private GameObject infoBox;

    // Lerp variables

    private float lerpTime;
    private float speed;
    public bool lerping;
    public bool hidden;
    private Vector3 startPoint;

    void Start () {
        lerping = false;
        hidden = true;
        speed = 2;
        GameObject playerGiant = GameObject.Find("Giant Player");
        infoBox = transform.GetChild(0).gameObject;
        startPoint = infoBox.transform.localPosition;
        infoBox.transform.localScale = Vector3.zero;
        infoBox.transform.localPosition = Vector3.zero;
        infoBox.transform.GetChild(0).GetComponent<Text>().text = "<b><i>" + Items.itemList[itemID].name + "</i></b>" + Environment.NewLine + Items.itemList[itemID].description;
        infoBox.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate () { playerGiant.GetComponent<Giant>().camp.GetComponent<Camp>().Craft(itemID); });
    }

	public void Move()
    {
        lerping = true;
        GetComponentInParent<CraftingButton>().RetractChildren();
    }

    void Update()
    {
        if (lerping)
        {
            lerpTime += Time.deltaTime * speed;
            if (hidden)
            {
                infoBox.transform.localPosition = Vector3.Lerp(Vector3.zero, startPoint, 1 - Curve(lerpTime));
                infoBox.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, 1 - Curve(lerpTime));
            }
            else
            {
                infoBox.transform.localPosition = Vector3.Lerp(startPoint, Vector3.zero, 1 - Curve(lerpTime));
                infoBox.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, 1 - Curve(lerpTime));
            }
        }
        if (lerpTime >= 1)
        {
            lerping = false;
            hidden = !hidden;
            lerpTime = 0;
        }
    }

    float Curve(float time)
    {
        return (0.7f * time * time) - (1.7f * time) + 1;
    }
}
