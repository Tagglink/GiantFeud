using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum VillagerState { NONE, IDLE, WALKING, GATHERING }

public class Villager : MonoBehaviour {
    
    public AnimationClip walkAnimation; // set in the inspector

    public VillagerState state;
    public ResourceType resource;
    public int speed;

    float gatherTime;
    int currentTargetPositionIndex;
    Transform[] movePositions;
    Vector3 tilePositionOffset;

    Camp camp; // the camp the villager belongs to.
    Animator animator;

	// Use this for initialization
	void Start () {
        currentTargetPositionIndex = 0;
        speed = 2;
        gatherTime = 5.0f;
        resource = ResourceType.NONE;
        state = VillagerState.IDLE;
        camp = GetComponentInParent<Camp>();
        animator = GetComponent<Animator>();
        tilePositionOffset = new Vector3(0, 1, 0);
	}
	
	// Update is called once per frame
	void Update () {
	    if (state == VillagerState.WALKING)
        {
            Move();

            if (CheckIfArrived())
                Arrived();
        }
	}

    void MoveIntoBounds()
    {
        transform.position = Map.tiles[7][1].transform.position + tilePositionOffset;
    }

    void MoveOutOfBounds()
    {
        transform.position = new Vector3(-30, -30, 0);
    }

    public void Gather(GameObject tile)
    {
        MoveIntoBounds();
        MoveTo(tile);
    }

    Transform[] FindTilePathTo(GameObject tile)
    {
        List<Transform> ret = new List<Transform>();



        return ret.ToArray();
    }

    void Move()
    {
        if (ValidateMovement())
            transform.position = Vector2.MoveTowards(transform.position, movePositions[currentTargetPositionIndex].position, speed * Time.deltaTime);

        if ((Vector2)transform.position == (Vector2)movePositions[currentTargetPositionIndex].position)
        {
            currentTargetPositionIndex++;
        }
    }

    bool ValidateMovement()
    {
        return movePositions.Length > 0 && currentTargetPositionIndex < movePositions.Length;
    }

    bool CheckIfArrived()
    {
        return movePositions.Length == currentTargetPositionIndex;
    }

    void Arrived()
    {
        Tile tile = movePositions[movePositions.Length - 1].GetComponent<Tile>();
        if (resource == ResourceType.NONE)
        {
            state = VillagerState.GATHERING;
            StartCoroutine(WaitForGather(tile));
        }
        else
        {
            GiveCampResource();
            MoveOutOfBounds();
            state = VillagerState.IDLE;
        }
    }

    void GiveCampResource()
    {
        camp.AddResource(resource, 1);
        resource = ResourceType.NONE;
    }

    IEnumerator WaitForGather(Tile tile)
    {
        yield return new WaitForSeconds(gatherTime);
        PickUpResource(tile);
        MoveTo(camp.gameObject);
    }

    void MoveTo(GameObject tile)
    {
        movePositions = FindTilePathTo(tile);
        // TODO: start walking animation
        state = VillagerState.WALKING;
    }

    void PickUpResource(Tile tile)
    {
        switch (tile.type)
        {
            case TileType.CATTLE:
                resource = ResourceType.MEAT;
                break;
            case TileType.CROPS:
                resource = ResourceType.WHEAT;
                break;
            case TileType.STONE:
                resource = ResourceType.STONE;
                break;
            case TileType.WATER:
                resource = ResourceType.WATER;
                break;
            case TileType.WOODS:
                resource = ResourceType.WOOD;
                break;
        }
    }
}
