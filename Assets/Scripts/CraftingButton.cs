using UnityEngine;
using System.Collections.Generic;

public class CraftingButton : MonoBehaviour {

    private List<GameObject> children;
    private List<Vector3> startPositions;
    public bool retracted;
    public bool lerping;
    private float lerpTime;
    private float speed;

	void Start () {
        lerpTime = 0;
        speed = 2;
        retracted = true;
        lerping = false;
        children = GetChildren();
        startPositions = GetPositions(children);
        HardRetract(children);
    }
	
	void Update () {
	    if (lerping)
        {
            Move();

            if (lerpTime >= 1)
            {
                retracted = !retracted;
                lerping = false;
                lerpTime = 0;
            }
        }
	}

    void Move()
    {
        lerpTime += Time.deltaTime * speed;
        for (int i = 0; i < children.Count; i++)
        {
            GameObject child = children[i];
            RectTransform rectTransform = child.GetComponent<RectTransform>();

            if (retracted)
            {
                rectTransform.localPosition = Vector3.Lerp(Vector3.zero, startPositions[i], 1 - Curve(lerpTime));
                rectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, 1 - Curve(lerpTime));
            }
            else
            {
                rectTransform.localPosition = Vector3.Lerp(startPositions[i], Vector3.zero, 1 - Curve(lerpTime));
                rectTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, 1 - Curve(lerpTime));
            }
        }
    }

    public void ToggleMovement()
    {
        lerping = true;
        if (!retracted)
        {
            RetractChildren();
        }
    }

    public void RetractChildren()
    {
        foreach (GameObject c in children)
        {
            ItemButton itemButton = c.GetComponent<ItemButton>();
            if (!itemButton.hidden)
            {
                itemButton.lerping = true;
            }
        }
    }

    List<GameObject> GetChildren()
    {
        List<GameObject> ret = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            ret.Add(transform.GetChild(i).gameObject);
        }
        return ret;
    }

    List<Vector3> GetPositions(List<GameObject> gameObjects)
    {
        List<Vector3> ret = new List<Vector3>();
        foreach (GameObject a in gameObjects)
        {
            ret.Add(a.GetComponent<RectTransform>().localPosition);
        }
        return ret;
    }

    void HardRetract(List<GameObject> gameObjects)
    {
        foreach (GameObject a in gameObjects)
        {
            a.GetComponent<RectTransform>().localPosition = Vector3.zero;
            a.GetComponent<RectTransform>().localScale = Vector3.zero;
        }
    }

    float Curve(float time)
    {
        return (0.7f * time * time) - (1.7f * time) + 1;
    }
}
