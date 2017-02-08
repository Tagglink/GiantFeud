using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public GameObject playerObject;
    private static Giant player;

	void Start () {
        player = playerObject.GetComponent<Giant>();
	}

    public static void EndGame(Giant giant)
    {
        if (giant == player)
        {
            // Play Defeat Animation
        }
        else
        {
            // Play Victory Animation
        }
    }
}
