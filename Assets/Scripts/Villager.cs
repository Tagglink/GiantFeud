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
        Vector3 spawnPosition = tile.transform.position + feetPositionOffset + tileCenterPositionOffset;
        transform.position = spawnPosition;
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
        Vector2 dstTileCenter = tile.transform.position + tileCenterPositionOffset;
        Vector2 currentTileCenter = transform.position - feetPositionOffset;
        Vector3 directionToTile = dstTileCenter - currentTileCenter;
        Collider2D lastCollider = null;

        var walkableTileLayerMask = 1 << 8;

        do
        {
            hit = Physics2D.Raycast(currentTileCenter, directionToTile, 0.5f, walkableTileLayerMask);

            if (lastCollider)
                lastCollider.enabled = true; // re-enable the collider as we have passed it

            if (hit)
            {
                ret.Add(hit.transform);
                hit.collider.enabled = false; // disable the collider as to not hit it again
                lastCollider = hit.collider; // cache the collider to re-enable later

                currentTileCenter = hit.transform.position + tileCenterPositionOffset;
                directionToTile = dstTileCenter - currentTileCenter;
            }
        } while (hit && hit.transform.gameObject != tile); // if the destination tile is hit, we're done here.

        if (lastCollider)
            lastCollider.enabled = true;

        return ret.ToArray();
    }

    void Move()
    {
        if (ValidateMovement())
        {
            Vector3 destinationPos = movePositions[currentTargetPositionIndex].position + tileCenterPositionOffset + feetPositionOffset;
            transform.position = Vector2.MoveTowards(transform.position, destinationPos, speed * Time.deltaTime);

            // if position reached, iterate position list
            if ((Vector2)transform.position == (Vector2)destinationPos)
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
        currentTargetPositionIndex = 0;
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
        MoveTo(Map.tiles[7][1]);
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
