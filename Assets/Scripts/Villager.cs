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

    /// <summary>
    /// Sets the villager's state to IDLE as well as moves it out of camera view
    /// </summary>
    void Escape()
    {
        state = VillagerState.IDLE;
        MoveOutOfBounds();
    }

    /// <summary>
    /// Moves the villager into the camp's spawn position.
    /// </summary>
    void MoveIntoBounds()
    {
        Vector3 spawnPosition = camp.homeTile.transform.position + feetPositionOffset + tileCenterPositionOffset;
        transform.position = spawnPosition;
    }

    /// <summary>
    /// Moves the villager outside of the camera's view
    /// </summary>
    void MoveOutOfBounds()
    {
        transform.position = new Vector3(-30, -30, 0);
    }

    /// <summary>
    /// Sends a villager to gather resources at the given tile
    /// </summary>
    /// <param name="tile">The tile to gather resources at</param>
    public void Gather(GameObject tile)
    {
        Tile tileScript = tile.GetComponent<Tile>();
        tileScript.occupied = true;
        MoveIntoBounds();
        resource = Resources.TileToResource(tileScript.type);
        Pathfind(tile, VillagerArriveAction.GATHER_RESOURCE);
    }

    /// <summary>
    /// Sends a villager to the Giant in order to use an item
    /// </summary>
    /// <param name="id">The id of the item to use</param>
    public void UseItem(ItemID id)
    {
        MoveIntoBounds();
        item = id; // set the item property to id so that the correct item is used on arrival.
        MoveTo(camp.giantTile, VillagerArriveAction.USE_ITEM);
    }

    /// <summary>
    /// The iterating function that is called once every update while the villager is moving.
    /// </summary>
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

    /// <summary>
    /// Checks if there is a position to move to and whether the current position exists.
    /// </summary>
    /// <returns>True if there is a position and the current position exists</returns>
    bool ValidateMovement()
    {
        return movePositions.Length > 0 && currentTargetPositionIndex < movePositions.Length;
    }

    /// <summary>
    /// Checks if the last position reached was the final position.
    /// </summary>
    /// <returns>True if the last position reached was the final position</returns>
    bool CheckIfArrived()
    {
        return movePositions.Length == currentTargetPositionIndex;
    }

    /// <summary>
    /// Called when a villager arrives at its destination after moving.
    /// Selects which function to call from the arriveAction property.
    /// </summary>
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

    /// <summary>
    /// Starts the gathering process when the villager has arrived at a resource tile.
    /// </summary>
    /// <param name="at">The tile the villager is standing on</param>
    void StartGather(Tile at)
    {
        if (resource == ResourceType.NONE)
            return;

        state = VillagerState.GATHERING;
        StartCoroutine(WaitForGather(at));
    }

    /// <summary>
    /// Called when the villager arrives at the Giant tile.
    /// Tells the giant to use the item given by the item property.
    /// </summary>
    void ApplyItemToGiant()
    {
        if (item == ItemID.NULL)
            return;

        Item itemObject = Items.itemList[item];
        camp.giant.GetComponent<Giant>().UseItem(itemObject);
    }

    /// <summary>
    /// Called when a villager arrives back at the camp after gathering a resource.
    /// Adds the gathered resource to the camp.
    /// </summary>
    void GiveCampResource()
    {
        // in lack of a proper event system, this is used to advance the tutorial in the right conditions
        if (tutorial.currentStep == 1 && !opponent && resource == ResourceType.WOOD)
            tutorial.AdvanceTutorial();

        camp.resources.Add(resource, resourcesCarried);
        resource = ResourceType.NONE;
        resourcesCarried = 0;
    }

    /// <summary>
    /// A coroutine that waits for gathering to finish.
    /// Sends the villager back to camp once the gathering has finished.
    /// </summary>
    /// <param name="at">The tile the villager is gathering on</param>
    /// <returns></returns>
    IEnumerator WaitForGather(Tile at)
    {
        yield return new WaitForSeconds(gatherTime);
        at.occupied = false;
        resourcesCarried = efficiency;
        Pathfind(camp.homeTile, VillagerArriveAction.LEAVE_RESOURCE);
    }

    /// <summary>
    /// The main function used to move a villager to a certain tile.
    /// </summary>
    /// <param name="tile">The tile to move to</param>
    /// <param name="actionAtArrival">The action to perform at arrival</param>
    void Pathfind(GameObject tile, VillagerArriveAction actionAtArrival)
    {
        tile.GetComponent<Tile>().occupied = true;

        GameObject cornerTileUpper = Map.tiles[2][1]; // select the tile above the Giant area
        GameObject cornerTileLower = Map.tiles[12][1]; // select the tile below the Giant area

        // get their respective centers
        Vector3 cornerUpperPos = cornerTileUpper.transform.position + tileCenterPositionOffset; 
        Vector3 cornerLowerPos = cornerTileLower.transform.position + tileCenterPositionOffset;

        Vector2 dstTileCenter = tile.transform.position + tileCenterPositionOffset;
        Vector2 currentTileCenter = transform.position - feetPositionOffset;
        Vector3 directionToTile = dstTileCenter - currentTileCenter;
        Ray2D rayToTile = new Ray2D(currentTileCenter, directionToTile);

        GameObject tileNext;

        // if the straight line to the destination intersects with the line between cornerTileUpper and cornerTileLower,
        // select the one of the two which is closest to the point of intersection and move to it first, then move to the destination.
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

    /// <summary>
    /// Checks whether a Ray intersects a line between two points
    /// </summary>
    /// <param name="ray">The ray to check</param>
    /// <param name="lineEnd1">The first endpoint of the line</param>
    /// <param name="lineEnd2">The second endpoint of the line</param>
    /// <param name="maxDistance">The maximum distance to check with the ray</param>
    /// <returns>True if the ray intersects</returns>
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

    /// <summary>
    /// Gets the point of intersection between two finite lines.
    /// </summary>
    /// <param name="p1">Endpoint 1 for line 1</param>
    /// <param name="p2">Endpoint 2 for line 1</param>
    /// <param name="p3">Endpoint 1 for line 2</param>
    /// <param name="p4">Endpoint 2 for line 2</param>
    /// <returns>The point of the intersection</returns>
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

    /// <summary>
    /// Tries to move the villager to the given tile with the fastest possible path.
    /// </summary>
    /// <param name="tile">The destination tile</param>
    /// <param name="actionAtArrival">The action to perform at arrival</param>
    void MoveTo(GameObject tile, VillagerArriveAction actionAtArrival)
    {
        Tile tileScript = tile.GetComponent<Tile>();

        arriveAction = actionAtArrival;

        movePositions = FindTilePathTo(tile);

        state = VillagerState.WALKING;
    }

    /// <summary>
    /// Find which of two specified GameObjects are closer to the Ray2D at any point along the Ray.
    /// </summary>
    /// <param name="ray">The Ray2D to check</param>
    /// <param name="obj1">The first GameObject</param>
    /// <param name="obj2">The second GameObject</param>
    /// <returns>The closest GameObject</returns>
    GameObject FindClosestToRay(Ray2D ray, GameObject obj1, GameObject obj2)
    {
        Vector2 pos1 = obj1.transform.position;
        Vector2 pos2 = obj2.transform.position;

        if (ShortestDistanceFromRayToPoint(ray, pos1) > ShortestDistanceFromRayToPoint(ray, pos2))
            return obj2;
        else
            return obj1;
    }

    /// <summary>
    /// Gets the shortest distance from a Ray to a point at any given point along the Ray.
    /// </summary>
    /// <param name="ray">The Ray to check</param>
    /// <param name="p">The point to check</param>
    /// <returns>The shortest distance</returns>
    float ShortestDistanceFromRayToPoint(Ray2D ray, Vector2 p)
    {
        Vector2 rayPoint = ray.GetPoint(1.0f);
        float x1 = ray.origin.x, y1 = ray.origin.y;
        float x2 = rayPoint.x, y2 = rayPoint.y;

        return Mathf.Abs(((y2 - y1) * p.x) - ((x2 - x1) * p.y) + (x2 * y1) - (y2 * x1)) / Mathf.Sqrt(Mathf.Pow((y2 - y1), 2) + Mathf.Pow((x2 - x1), 2));
    }

    /// <summary>
    /// Used by the MoveTo function to create the fastest possible path to a destination tile and return a list of
    /// the travel points. Does not check for obstacles along the way.
    /// </summary>
    /// <param name="tile">The destination tile</param>
    /// <returns>An array of the Transforms to travel to, in order from first to last</returns>
    Transform[] FindTilePathTo(GameObject tile)
    {
        List<Transform> ret = new List<Transform>();

        RaycastHit2D hit;
        Vector2 dstTileCenter = tile.transform.position + tileCenterPositionOffset;
        Vector2 currentTileCenter = transform.position - feetPositionOffset;
        Vector2 directionToTile = dstTileCenter - currentTileCenter;

        Ray2D rayToTile = new Ray2D(currentTileCenter, directionToTile);
        float[] angles = new float[] { 45.0f, 135.0f, 225.0f, 315.0f };

        // get a list of possible angles, ordered by angles that point closest to the destination first
        Ray2D[] moveOptions = RayAngleAdjust(rayToTile, angles); 
        int hitCount;

        Collider2D lastCollider = null;

        // get the layer mask to only check for hits on tiles that are walkable
        var walkableTileLayerMask = 1 << 8; 

        do
        {
            hitCount = angles.Length;
            // check all possible directions until we hit a tile
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

    /// <summary>
    /// Using a Ray2D and a list of possible angles, returns a list of rays which point in the
    /// possible angles, ordered with the ray closest to the origin ray first.
    /// </summary>
    /// <param name="originRay">The base ray to match</param>
    /// <param name="angles">The possible angles, 0 &lt;= x &lt; 360, with the smallest angle first</param>
    /// <returns></returns>
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

    /// <summary>
    /// Sorts a list of angle values with closest to the pivot angle first.
    /// </summary>
    /// <param name="nums">The list of angles to sort, 0 &lt;= x &lt; 360, with the smallest angle first</param>
    /// <param name="pivot">The angle to sort after</param>
    /// <returns></returns>
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

    /// <summary>
    /// Gets the closest angle to the pivot angle out of two other angles.
    /// </summary>
    /// <param name="pivot">The angle to compare to</param>
    /// <param name="a1">The first angle</param>
    /// <param name="a2">The second angle</param>
    /// <returns>The closest angle</returns>
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
