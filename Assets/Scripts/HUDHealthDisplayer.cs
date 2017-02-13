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
        GetComponentInChildren<Text>().text = giant.hp + " / " + giant.stats.maxHP;
        float healthPercentage = (float)giant.hp / giant.stats.maxHP;
        GetComponentsInChildren<Image>()[1].fillAmount = healthPercentage;
	}
}
