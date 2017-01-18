using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum VillagerState { NONE, IDLE, WALKING, GATHERING }

public class Villager : MonoBehaviour {

    public VillagerState state;
    public ResourceType resource;
    public int speed;

    float gatherTime;
    int currentTargetPositionIndex;
    Transform[] movePositions;
    Vector3 tileCenterPositionOffset;
    Vector3 feetPositionOffset;

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
        tileCenterPositionOffset = new Vector3(0, 0.25f, 0);
        feetPositionOffset = new Vector3(0, 0.265f, 0);
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
        GameObject tile = Map.tiles[7][1];
        transform.position = tile.transform.position + feetPositionOffset + tileCenterPositionOffset;
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
        
        RaycastHit2D hit;
        Ray2D dstRay = new Ray2D(transform.position, tile.transform.position + tileCenterPositionOffset + feetPositionOffset - transform.position);

        var walkableTileLayerMask = 1 << 8;

        int debug_max_loops = 40;
        int loop_count = 0;

        do
        {
            loop_count++;

            if (loop_count > debug_max_loops)
                break;

            hit = Physics2D.Raycast(dstRay.origin, dstRay.direction, 0.26f, walkableTileLayerMask);

            if (hit)
            {
                ret.Add(hit.transform);
                dstRay.origin = hit.transform.position + tileCenterPositionOffset + feetPositionOffset;
                dstRay.direction = tile.transform.position - hit.transform.position;
            }
        } while (hit && hit.transform.gameObject != tile); // if the destination tile is hit, we're done here.

        return ret.ToArray();
    }

    void Move()
    {
        if (ValidateMovement())
        {
            transform.position = Vector2.MoveTowards(transform.position, movePositions[currentTargetPositionIndex].position, speed * Time.deltaTime);

            // if position reached, go on to the next one
            if ((Vector2)transform.position == (Vector2)movePositions[currentTargetPositionIndex].position)
            {
                currentTargetPositionIndex++;
            }
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
        if (resource == ResourceType.NONE) // is the villager carrying a resource?
        {
            state = VillagerState.GATHERING;
            animator.SetTrigger("gathering");
            StartCoroutine(WaitForGather(tile));
        }
        else
        {
            GiveCampResource();
            MoveOutOfBounds();
            state = VillagerState.IDLE;
            animator.SetTrigger("idle");
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
        state = VillagerState.WALKING;
        animator.SetTrigger("walking");
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
