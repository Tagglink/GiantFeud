using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public GameObject playerObject;
    public GameObject victoryScreen;
    private static Giant player;

	void Start () {
        player = playerObject.GetComponent<Giant>();
        Screen.SetResolution(1920, 1080, true);
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
        victoryScreen.GetComponent<RectTransform>().localScale = Vector3.one;
        Transform text = victoryScreen.transform.GetChild(1);
        if (loss)
        {
            text.GetComponent<Text>().text = "A   LOSER   IS   YOU";
        }
        else
        {
            text.GetComponent<Text>().text = "A   WINNER   IS   YOU";
        }
    }
}
