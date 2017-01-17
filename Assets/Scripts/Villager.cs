using UnityEngine;
using System.Collections;

public enum VillagerState { NONE, IDLE, WALKING, GATHERING }

public class Villager : MonoBehaviour {

    public VillagerState state;

	// Use this for initialization
	void Start () {
        state = VillagerState.IDLE;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void WalkTo(GameObject tile)
    {
        
    }
}
