using UnityEngine;
using System;
using UnityEngine.UI;

public class HUDHealthDisplayer : MonoBehaviour {

    public GameObject giantObject;
    private Giant giant;

	void Start () {
        giant = giantObject.GetComponent<Giant>();
    }
	
	void Update () {
        GetComponentInChildren<Text>().text = giant.stats.hp + " / " + giant.stats.maxHP;
        float healthPercentage = (float)giant.stats.hp / giant.stats.maxHP;
        GetComponent<Image>().fillAmount = healthPercentage;
	}
}
