using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum VillagerState { NONE, IDLE, WALKING, GATHERING }
public enum VillagerArriveAction { NONE, LEAVE_RESOURCE, USE_ITEM, GATHER_RESOURCE }

public class Villager : MonoBehaviour {

    public VillagerState state;
    public ResourceType resource;
    public int speed;
    public int resourcesCarried;
    public int efficiency; // the amount of resources to pick up per Gather

    VillagerArriveAction arriveAction;
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
        resourcesCarried = 0;
        efficiency = 1;
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
        Vector3 spawnPosition = camp.homeTile.transform.position + feetPositionOffset + tileCenterPositionOffset;
        transform.position = spawnPosition;
    }

    void MoveOutOfBounds()
    {
        transform.position = new Vector3(-30, -30, 0);
    }

    public void Gather(GameObject tile)
    {
        MoveIntoBounds();
        resource = Resources.TileToResource(tile.GetComponent<Tile>().type);
        MoveTo(tile, VillagerArriveAction.GATHER_RESOURCE);
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
        Tile at = movePositions[movePositions.Length - 1].GetComponent<Tile>();
        currentTargetPositionIndex = 0;
        
        switch (arriveAction)
        {
            case VillagerArriveAction.NONE:
                // idk
                break;
            case VillagerArriveAction.GATHER_RESOURCE:
                StartGather(at);
                break;
            case VillagerArriveAction.LEAVE_RESOURCE:
                LeaveResourceAtCamp();
                break;
            case VillagerArriveAction.USE_ITEM:
                // TODO: Giant use item function
                break;
        }

        arriveAction = VillagerArriveAction.NONE;
    }

    void StartGather(Tile at)
    {
        if (resource == ResourceType.NONE)
            return;

        state = VillagerState.GATHERING;
        animator.SetTrigger("gathering");
        StartCoroutine(WaitForGather(at));
    }

    void LeaveResourceAtCamp()
    {
        GiveCampResource();
        MoveOutOfBounds();
        state = VillagerState.IDLE;
        animator.SetTrigger("idle");
    }

    void GiveCampResource()
    {
        camp.resources.Add(resource, resourcesCarried);
        resource = ResourceType.NONE;
        resourcesCarried = 0;
    }

    IEnumerator WaitForGather(Tile at)
    {
        yield return new WaitForSeconds(gatherTime);
        at.occupied = false;
        resourcesCarried = efficiency;
        MoveTo(camp.homeTile, VillagerArriveAction.LEAVE_RESOURCE);
    }

    void MoveTo(GameObject tile, VillagerArriveAction actionAtArrival)
    {
        Tile tileScript = tile.GetComponent<Tile>();

        arriveAction = actionAtArrival;
        tileScript.occupied = true;
        movePositions = FindTilePathTo(tile);
        state = VillagerState.WALKING;
        animator.SetTrigger("walking");
    }
}
