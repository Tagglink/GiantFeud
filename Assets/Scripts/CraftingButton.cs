using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class CraftingButton : MonoBehaviour {

    private List<GameObject> children;
    private List<Vector3> startPositions;
    private List<Vector3> startPoints;
    private List<Vector3> endPoints;
    private Vector3 startScale;
    private Vector3 endScale;
    public displayState state;
    private float lerpTime;
    private float speed;

	void Start () {
        lerpTime = 0;
        speed = 3;
        state = displayState.HIDDEN;
        children = GetChildren();
        startPositions = MakePositionsCircular(children, 130f);
        HardRetract(children);
        startPoints = new List<Vector3>(startPositions);
        endPoints = new List<Vector3>(startPositions);

        MakeInfoBoxesCircular(children, 160f);
    }
	
	void Update () {
	    if (state == displayState.LERPING)
        {
            Move();

            if (lerpTime >= 1)
            {
                if (children[0].transform.localPosition == Vector3.zero)
                {
                    state = displayState.HIDDEN;
                    SetEventTriggerEnabled(false);
                }
                else
                {
                    state = displayState.DISPLAYING;
                    SetEventTriggerEnabled(true);
                }
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
            
            rectTransform.localPosition = Vector3.Lerp(startPoints[i], endPoints[i], 1 - Curve(lerpTime));
            rectTransform.localScale = Vector3.Lerp(startScale, endScale, 1 - Curve(lerpTime));
        }
    }

    public void ToggleMovement()
    {
        if (state == displayState.DISPLAYING)
        {
            startPoints = new List<Vector3>(startPositions);
            for (int i = 0; i < children.Count; i++)
            {
                endPoints[i] = Vector3.zero;
            }
            startScale = Vector3.one;
            endScale = Vector3.zero;
        }
        else if (state == displayState.HIDDEN)
        {
            RetractChildren(gameObject);
            endPoints = new List<Vector3>(startPositions);
            for (int i = 0; i < children.Count; i++)
            {
                startPoints[i] = Vector3.zero;
            }
            startScale = Vector3.zero;
            endScale = Vector3.one;
        }
        else if (state == displayState.LERPING)
        {
            RetractChildren(gameObject);
            for (int i = 0; i < children.Count; i++)
            {
                endPoints[i] = Vector3.zero;
            }
            startPoints = new List<Vector3>(startPositions);
            lerpTime = 0.2f;
            startScale = Vector3.one;
            endScale = Vector3.zero;
        }
        state = displayState.LERPING;
    }

    public void RetractChildren(GameObject origin)
    {
        foreach (GameObject c in children)
        {
            HUDItemButton itemButton = c.GetComponent<HUDItemButton>();
            if (itemButton.state != displayState.HIDDEN && itemButton.gameObject != origin)
            {
                if (!itemButton.RetractIfLerping())
                    itemButton.StartLerping();
            }
        }
    }

    List<GameObject> GetChildren()
    {
        var ret = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            ret.Add(transform.GetChild(i).gameObject);
        }
        return ret;
    }

    List<Vector3> GetPositions(List<GameObject> gameObjects)
    {
        var ret = new List<Vector3>();
        foreach (GameObject a in gameObjects)
        {
            ret.Add(a.GetComponent<RectTransform>().localPosition);
        }
        return ret;
    }

    List<Vector3> MakePositionsCircular(List<GameObject> gameObjects, float radius)
    {
        float angleStep = 360.0f / gameObjects.Count;
        List<Vector3> ret = new List<Vector3>();
        Vector3 pos;

        ret.Capacity = gameObjects.Count;

        for (int i = 0; i < gameObjects.Count; i++)
        {
            pos = new Vector3(Mathf.Cos(Mathf.Deg2Rad * (angleStep * i + 90)) * radius, Mathf.Sin(Mathf.Deg2Rad * (angleStep * i + 90)) * radius);
            ret.Add(pos);
            gameObjects[i].GetComponent<RectTransform>().localPosition = pos;
        }

        return ret;
    }

    void MakeInfoBoxesCircular(List<GameObject> gameObjects, float radius)
    {
        float angleStep = 360.0f / gameObjects.Count;
        Vector3 pos;
        int i;

        for (i = 0; i < gameObjects.Count; i++)
        {
            pos = new Vector3(Mathf.Cos(Mathf.Deg2Rad * (angleStep * i + 90)) * radius, Mathf.Sin(Mathf.Deg2Rad * (angleStep * i + 90)) * radius);
            gameObjects[i].GetComponent<HUDItemButton>().displayPoint = pos;
        }
    }

    List<GameObject> GetChildrenOf(List<GameObject> gameObjects, int index)
    {
        List<GameObject> ret = new List<GameObject>();

        foreach (GameObject obj in gameObjects)
            ret.Add(obj.transform.GetChild(index).gameObject);

        return ret;
    }
    
    void SetEventTriggerEnabled(bool val)
    {
        foreach (GameObject child in children)
        {
            child.GetComponent<EventTrigger>().enabled = val;
        }
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
