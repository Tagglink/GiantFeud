using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public GameObject playerObject;
    public GameObject victoryScreen;
    private static Giant player;

	void Start () {
        player = playerObject.GetComponent<Giant>();
	}

    public void EndGame(Giant giant)
    {
        bool loss;
        if (giant == player)
        {
            // Play Defeat Animation
            loss = true;
        }
        else
        {
            // Play Victory Animation
            loss = false;
        }
        PlayEndAnimation(loss);
    }

    void PlayEndAnimation(bool loss)
    {
        if (loss)
        {
            
        }
        else
        {

        }
    }
}
