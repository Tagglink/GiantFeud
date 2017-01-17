using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;


public class Player : MonoBehaviour {

    public GameObject map;

    [HideInInspector]
    public Camp camp;

    private List<Transform> centerOfTiles;

    private Color highlight;
    private int changeValue;

    private Vector3 offScale;

    void Start ()
    {
        camp = GetComponent<Camp>();

        offScale = new Vector3(0, 0.25f, 0);
        centerOfTiles = new List<Transform>();
        for (var i = 0; i < map.transform.childCount; i++)
        {
            centerOfTiles.Add(map.transform.GetChild(i));
        }
        changeValue = 1;
        highlight = Color.white;
	}

	void Update ()
    {
        UpdateColorCycle();
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Transform mouseTile = FindNearestTile(mousePosition);

        if (mouseTile != null)
            mouseTile.GetComponent<SpriteRenderer>().color = highlight;

        if (Input.GetMouseButtonDown(1)) // Right-click
        {
            if (mouseTile != null)
                camp.SendVillagerTo(mouseTile.gameObject);
        }
    }

    void UpdateColorCycle()
    {
        if (highlight.r == 1)
            changeValue = 1;
        if (highlight.r <= 0.6f)
            changeValue = -1;

        highlight.r -= 0.01f * changeValue;
        highlight.g -= 0.01f * changeValue;
        highlight.b -= 0.01f * changeValue;
    }

    Transform FindNearestTile(Vector3 position)
    {
        Transform closestTransform = null;
        Vector3 closestPoint = new Vector3(0.25f,0.25f,0);
        Vector3 relativePoint;
        foreach (Transform t in centerOfTiles)
        {
            t.GetComponent<SpriteRenderer>().color = Color.white;
            relativePoint = t.position + offScale - position;
            if (Mathf.Abs(relativePoint.x) + Mathf.Abs(relativePoint.y) < Mathf.Abs(closestPoint.x) + Mathf.Abs(closestPoint.y))
            {
                closestPoint = relativePoint;
                closestTransform = t;
            }
        }

        return closestTransform;
    }
}
