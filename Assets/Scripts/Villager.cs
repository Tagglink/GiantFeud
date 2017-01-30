using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum VillagerState { NONE, IDLE, WALKING, GATHERING }
public enum VillagerArriveAction { NONE, LEAVE_RESOURCE, USE_ITEM, GATHER_RESOURCE, ESCAPE, WALK }

public class Villager : MonoBehaviour {

    public VillagerState state;
    public ResourceType resource;
    public ItemID item;
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

    VillagerArriveAction arriveActionNext;
    GameObject targetTileNext;

    // Use this for initialization
    void Start () {
        currentTargetPositionIndex = 0;
        speed = 2;
        gatherTime = 5.0f;
        resourcesCarried = 0;
        efficiency = 1;
        resource = ResourceType.NONE;
        state = VillagerState.IDLE;
        arriveAction = VillagerArriveAction.NONE;
        arriveActionNext = VillagerArriveAction.NONE;
        targetTileNext = null;
        camp = GetComponentInParent<Camp>();
        animator = GetComponent<Animator>();
        tileCenterPositionOffset = new Vector3(0, 0.25f, 0);
        feetPositionOffset = new Vector3(0, 0.265f, 0);
	}
	
	// Update is called once per frame
	void Update () {
        SetDepth();

	    if (state == VillagerState.WALKING)
        {
            Move();

            if (CheckIfArrived())
                Arrived();
        }
	}

    void SetDepth()
    {
        Vector3 targetPosition = transform.position;
        targetPosition.z = transform.position.y;
        transform.position = targetPosition;
    }

    void Escape()
    {
        state = VillagerState.IDLE;
        MoveOutOfBounds();
        animator.SetTrigger("idle");
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
        Pathfind(tile, VillagerArriveAction.GATHER_RESOURCE);
    }

    public void UseItem(ItemID id)
    {
        MoveIntoBounds();
        item = id;
        Pathfind(camp.giantTile, VillagerArriveAction.USE_ITEM);
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
                GiveCampResource();
                Escape();
                break;
            case VillagerArriveAction.USE_ITEM:
                ApplyItemToGiant();
                MoveTo(camp.homeTile, VillagerArriveAction.ESCAPE);
                break;
            case VillagerArriveAction.ESCAPE:
                Escape();
                break;
            case VillagerArriveAction.WALK:
                MoveTo(targetTileNext, arriveActionNext);
                targetTileNext = null;
                arriveActionNext = VillagerArriveAction.NONE;
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

    void ApplyItemToGiant()
    {
        if (item == ItemID.NULL)
            return;

        Item itemObject = Items.itemList[item];
        camp.giant.GetComponent<Giant>().UseItem(itemObject);
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
        Pathfind(camp.homeTile, VillagerArriveAction.LEAVE_RESOURCE);
    }

    void Pathfind(GameObject tile, VillagerArriveAction actionAtArrival)
    {
        
        GameObject cornerTileUpper = Map.tiles[2][1];
        GameObject cornerTileLower = Map.tiles[12][1];

        Vector3 cornerUpperPos = cornerTileUpper.transform.position + tileCenterPositionOffset;
        Vector3 cornerLowerPos = cornerTileLower.transform.position + tileCenterPositionOffset;

        Vector2 dstTileCenter = tile.transform.position + tileCenterPositionOffset;
        Vector2 currentTileCenter = transform.position - feetPositionOffset;
        Vector3 directionToTile = dstTileCenter - currentTileCenter;
        Ray2D rayToTile = new Ray2D(currentTileCenter, directionToTile);

        GameObject tileNext;

        if (LineIntersect(rayToTile, cornerUpperPos, cornerLowerPos))
        {
            arriveActionNext = actionAtArrival;
            targetTileNext = tile;

            tileNext = FindClosestToRay(rayToTile, cornerTileUpper, cornerTileLower);
            MoveTo(tileNext, VillagerArriveAction.WALK);
        }
        else
        {
            MoveTo(tile, actionAtArrival);
        }
    }

    bool LineIntersect(Ray2D ray, Vector2 lineEnd1, Vector2 lineEnd2)
    {
        float k = ray.direction.y / ray.direction.x;
        float x = lineEnd1.x - ray.origin.x;
        float y = k * x + ray.origin.y;

        return (y < lineEnd1.y && y > lineEnd2.y) || (y < lineEnd2.y && y > lineEnd1.y);
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

    GameObject FindClosestToRay(Ray2D ray, GameObject obj1, GameObject obj2)
    {
        Vector2 pos1 = obj1.transform.position;
        Vector2 pos2 = obj2.transform.position;

        if (ShortestDistanceFromRayToPoint(ray, pos1) > ShortestDistanceFromRayToPoint(ray, pos2))
            return obj2;
        else
            return obj1;
    }

    float ShortestDistanceFromRayToPoint(Ray2D ray, Vector2 p)
    {
        float x1 = ray.origin.x, y1 = ray.origin.y;
        float x2 = ray.direction.x, y2 = ray.direction.y;

        return Mathf.Abs(((y2 - y1) * p.x) - ((x2 - x1) * p.y) + (x2 * y1) - (y2 * x1)) / Mathf.Sqrt(Mathf.Pow((y2 - y1), 2) + Mathf.Pow((x2 - x1), 2));
    }
}
