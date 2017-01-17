using UnityEngine;
using System.Collections;

public enum VillagerState { NONE, IDLE, WALKING, GATHERING }

public class Villager : MonoBehaviour {

    public VillagerState state;

    int speed;

    int currentTargetPositionIndex;
    Transform[] movePositions;

	// Use this for initialization
	void Start () {
        currentTargetPositionIndex = 0;
        speed = 2;

        state = VillagerState.IDLE;
	}
	
	// Update is called once per frame
	void Update () {
	    if (state == VillagerState.WALKING)
        {

        }
	}

    public void Gather(GameObject tile)
    {
        
    }

    void SetMovePath(Transform[] position)
    {
        
    }

    void Move()
    {
        if ((Vector2)transform.position == (Vector2)movePositions[currentTargetPositionIndex].position)
        {
            currentTargetPositionIndex++;
        }

        transform.position = Vector2.MoveTowards(transform.position, movePositions[currentTargetPositionIndex].position, speed * Time.deltaTime);
    }
}
