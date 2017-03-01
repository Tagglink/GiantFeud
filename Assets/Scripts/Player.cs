using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;


public class Player : MonoBehaviour {

    public GameObject map;
    public GameObject arrow;

    [HideInInspector]
    public Camp camp;

    private List<Transform> centerOfTiles;

    private Color highlight;
    private int changeValue;
    private bool arrowDirection;
    private float arrowOffset;

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

        arrowDirection = false;
        arrowOffset = 1f;

        // testing

        camp.resources = new Resources(99, 99, 99, 99, 99);
	}

	void Update ()
    {
        UpdateCycles();
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Transform mouseTile = FindNearestTransform(centerOfTiles.ToArray(), mousePosition);
        Tile mouseTileScript = null;


        if (mouseTile != null) {
            mouseTile.GetComponent<SpriteRenderer>().color = highlight;
            mouseTileScript = mouseTile.GetComponent<Tile>();

            // Arrow Offset
            arrow.transform.position = new Vector3(mouseTile.position.x, mouseTile.position.y + arrowOffset);
        }
        else
        {
            arrow.transform.position = new Vector3(0, 5);
        }

        if (Input.GetMouseButtonDown(1)) // Right-click
        {
            if (mouseTile && mouseTileScript && mouseTileScript.type != TileType.GIANTS && mouseTileScript.type != TileType.CAMP)
                camp.SendVillagerToGather(mouseTile.gameObject);
        }
    }

    void UpdateCycles()
    {
        UpdateColorCycle();
        UpdateArrowCycle();
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

    void UpdateArrowCycle()
    {
        if (arrowDirection)
            arrowOffset += 0.01f;
        else
            arrowOffset -= 0.01f;
        if (arrowOffset < 0.6f)
            arrowDirection = !arrowDirection;
        else if (arrowOffset > 1f)
            arrowDirection = !arrowDirection;
    }

    Transform FindNearestTransform(Transform[] transforms, Vector3 position)
    {
        Transform closestTransform = null;
        Vector3 closestPoint = new Vector3(0.25f,0.25f,0);
        Vector3 relativePoint;

        foreach (Transform t in transforms)
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
