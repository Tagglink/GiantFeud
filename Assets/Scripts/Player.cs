using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;


public class Player : MonoBehaviour {

    public GameObject map;
    public GameObject arrow;

    public Sprite[] resourceIcons;

    [HideInInspector]
    public Camp camp;

    private List<Transform> centerOfTiles;

    private Color highlight;
    private int changeValue;
    private bool arrowDirection;

    private Vector3 offScale;

    public HUDTutorial tutorial; // inspector set

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

        // testing

        camp.resources = new Resources(99, 99, 99, 99, 99);
	}

	void Update ()
    {
        UpdateCycles();
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Transform mouseTile = FindNearestTransform(centerOfTiles.ToArray(), mousePosition);
        Tile mouseTileScript = null;


        if (mouseTile) {

            mouseTileScript = mouseTile.GetComponent<Tile>();

            if (mouseTileScript && mouseTileScript.type != TileType.GIANTS && mouseTileScript.type != TileType.CAMP && !mouseTileScript.occupied && camp.GetIdleVillagers().Count > 0)
            {
                mouseTile.GetComponent<SpriteRenderer>().color = highlight;

                // Arrow Offset
                arrow.transform.position = new Vector3(mouseTile.position.x, mouseTile.position.y + highlight.r);

                int iconNumber;

                switch(mouseTileScript.type)
                {
                    case TileType.CATTLE:
                        iconNumber = 0;
                        break;
                    case TileType.STONE:
                        iconNumber = 1;
                        break;
                    case TileType.WATER:
                        iconNumber = 2;
                        break;
                    case TileType.CROPS:
                        iconNumber = 3;
                        break;
                    case TileType.WOODS:
                        iconNumber = 4;
                        break;
                    default:
                        iconNumber = 0;
                        break;
                }

                arrow.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = resourceIcons[iconNumber];
            }
            else
            {
                arrow.transform.position = new Vector3(0, 5);
            }
        }
        else
        {
            arrow.transform.position = new Vector3(0, 5);
        }

        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            if (mouseTile && mouseTileScript && mouseTileScript.type != TileType.GIANTS && mouseTileScript.type != TileType.CAMP && !mouseTileScript.occupied)
            {
                camp.SendVillagerToGather(mouseTile.gameObject);
                /*
                if (mouseTileScript.type != TileType.NONE && tutorial.currentStep == 1)
                {
                    tutorial.AdvanceTutorial();
                }
                */
            }
        }
    }

    void UpdateCycles()
    {
        UpdateColorCycle();
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
