using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum VillagerState { NONE, IDLE, WALKING, GATHERING }
public enum VillagerArriveAction { NONE, LEAVE_RESOURCE, USE_ITEM, GATHER_RESOURCE, ESCAPE, WALK }

public class Villager : MonoBehaviour {

    public HUDTutorial tutorial; // inspector set
    public bool opponent;        // inspector set

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
        item = ItemID.NULL;

        targetTileNext = null;

        camp = GetComponentInParent<Camp>();

        tileCenterPositionOffset = new Vector3(0, 0.25f, 0);
        feetPositionOffset = new Vector3(0, 0, 0);
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
        Tile tileScript = tile.GetComponent<Tile>();
        tileScript.occupied = true;
        MoveIntoBounds();
        resource = Resources.TileToResource(tileScript.type);
        Pathfind(tile, VillagerArriveAction.GATHER_RESOURCE);
    }

    public void UseItem(ItemID id)
    {
        MoveIntoBounds();
        item = id;
        MoveTo(camp.giantTile, VillagerArriveAction.USE_ITEM);
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
    }

    void StartGather(Tile at)
    {
        if (resource == ResourceType.NONE)
            return;

        state = VillagerState.GATHERING;
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
        if (tutorial.currentStep == 1 && !opponent && resource == ResourceType.WOOD)
            tutorial.AdvanceTutorial();

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
        tile.GetComponent<Tile>().occupied = true;

        GameObject cornerTileUpper = Map.tiles[2][1];
        GameObject cornerTileLower = Map.tiles[12][1];

        Vector3 cornerUpperPos = cornerTileUpper.transform.position + tileCenterPositionOffset;
        Vector3 cornerLowerPos = cornerTileLower.transform.position + tileCenterPositionOffset;

        Vector2 dstTileCenter = tile.transform.position + tileCenterPositionOffset;
        Vector2 currentTileCenter = transform.position - feetPositionOffset;
        Vector3 directionToTile = dstTileCenter - currentTileCenter;
        Ray2D rayToTile = new Ray2D(currentTileCenter, directionToTile);

        GameObject tileNext;

        if (LineIntersect(rayToTile, cornerUpperPos, cornerLowerPos, Vector2.Distance(dstTileCenter, currentTileCenter)))
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

    bool LineIntersect(Ray2D ray, Vector2 lineEnd1, Vector2 lineEnd2, float maxDistance)
    {
        Vector2 point = ray.GetPoint(maxDistance);

        float lineTilt = (lineEnd1.y - lineEnd2.y) / (lineEnd1.x - lineEnd2.x);
        float rayTilt = ray.direction.y / ray.direction.x;

        if (lineTilt == rayTilt)
            return false;

        Vector2 intersection = GetLineIntersection(ray.origin, point, lineEnd1, lineEnd2);

        return ray.origin.x > point.x ? intersection.x < ray.origin.x && intersection.x > point.x : intersection.x > ray.origin.x && intersection.x < point.x;
    }

    Vector2 GetLineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        Vector2 ret = new Vector2();

        float divisor = ((p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x));
        float factor1 = (p1.x * p2.y - p1.y * p2.x);
        float factor2 = (p3.x * p4.y - p3.y * p4.x);

        ret.x = (factor1 * (p3.x - p4.x) - (p1.x - p2.x) * factor2) / divisor;
        ret.y = (factor1 * (p3.y - p4.y) - (p1.y - p2.y) * factor2) / divisor;

        return ret;
    }

    void MoveTo(GameObject tile, VillagerArriveAction actionAtArrival)
    {
        Tile tileScript = tile.GetComponent<Tile>();

        arriveAction = actionAtArrival;

        movePositions = FindTilePathTo(tile);

        state = VillagerState.WALKING;
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
        Vector2 rayPoint = ray.GetPoint(1.0f);
        float x1 = ray.origin.x, y1 = ray.origin.y;
        float x2 = rayPoint.x, y2 = rayPoint.y;

        return Mathf.Abs(((y2 - y1) * p.x) - ((x2 - x1) * p.y) + (x2 * y1) - (y2 * x1)) / Mathf.Sqrt(Mathf.Pow((y2 - y1), 2) + Mathf.Pow((x2 - x1), 2));
    }

    Transform[] FindTilePathTo(GameObject tile)
    {
        List<Transform> ret = new List<Transform>();

        RaycastHit2D hit;
        Vector2 dstTileCenter = tile.transform.position + tileCenterPositionOffset;
        Vector2 currentTileCenter = transform.position - feetPositionOffset;
        Vector2 directionToTile = dstTileCenter - currentTileCenter;

        Ray2D rayToTile = new Ray2D(currentTileCenter, directionToTile);
        float[] angles = new float[] { 45.0f, 135.0f, 225.0f, 315.0f };
        Ray2D[] moveOptions = RayAngleAdjust(rayToTile, angles);
        int hitCount;

        Collider2D lastCollider = null;

        var walkableTileLayerMask = 1 << 8;

        do
        {
            hitCount = angles.Length;
            do
            {
                hit = Physics2D.Raycast(moveOptions[angles.Length - hitCount].origin, moveOptions[angles.Length - hitCount].direction, 0.7f, walkableTileLayerMask);
            } while (!hit && --hitCount != 0);

            if (lastCollider)
                lastCollider.enabled = true; // re-enable the collider as we have passed it

            if (hit)
            {
                ret.Add(hit.transform);
                hit.collider.enabled = false; // disable the collider as to not hit it again
                lastCollider = hit.collider; // cache the collider to re-enable later

                rayToTile.origin = hit.transform.position + tileCenterPositionOffset;
                rayToTile.direction = dstTileCenter - rayToTile.origin;
                moveOptions = RayAngleAdjust(rayToTile, angles);
            }
        } while (hit && hit.transform.gameObject != tile); // if the destination tile is hit, we're done here.

        if (lastCollider)
            lastCollider.enabled = true;

        return ret.ToArray();
    }

    // assuming the list of angles is sorted by lowest first
    // and the angles are limited to 0 <= x < 360
    // returns a list of rays with a length equal to the length of angles,
    // with directions adjusted to the angles.
    // the ray with a direction closest to originRay is first in the list, and so on
    Ray2D[] RayAngleAdjust(Ray2D originRay, float[] angles)
    {
        Ray2D[] ret = new Ray2D[angles.Length];

        float angle = Mathf.Rad2Deg * Mathf.Atan2(originRay.direction.y, originRay.direction.x);
        float a;

        while (angle < 0)
            angle += 360;

        angles = SortAnglesByPivot(angles, angle);

        for (int i = 0; i < angles.Length; i++)
        {
            a = angles[i];
            ret[i].origin = originRay.origin;
            ret[i].direction = new Vector2(Mathf.Cos(Mathf.Deg2Rad * a), Mathf.Sin(Mathf.Deg2Rad * a));
        }

        return ret;
    }

    // sorts a list of angles by closest to pivot.
    float[] SortAnglesByPivot(float[] nums, float pivot)
    {
        List<float> sortedNums = new List<float>();
        float num_current;

        sortedNums.Add(nums[0]);

        for (int i = 1; i < nums.Length; i++)
        {
            num_current = nums[i];

            for (int j = 0; j < sortedNums.Count; j++)
            {
                if (sortedNums[j] != ClosestAngle(pivot, num_current, sortedNums[j])) // if it's closer, place before
                {
                    sortedNums.Insert(j, num_current);
                    break;
                }
                else if (j == sortedNums.Count - 1) // if it's the furthest away number, place at end
                {
                    sortedNums.Add(num_current);
                    break;
                }
            }
        }

        return sortedNums.ToArray();
    }

    float ClosestAngle(float pivot, float a1, float a2)
    {
        int pivotStage = (int)Mathf.Round(pivot / 360);
        int a1Stage = (int)Mathf.Round(a1 / 360);
        int a2Stage = (int)Mathf.Round(a2 / 360);

        float b1 = a1 + (pivotStage - a1Stage) * 360;
        float b2 = a2 + (pivotStage - a2Stage) * 360;

        if (Mathf.Abs(pivot - b1) <= Mathf.Abs(pivot - b2))
            return a1;
        else
            return a2;
    }
}
