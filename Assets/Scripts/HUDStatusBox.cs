using UnityEngine;
using System.Collections;

public class HUDStatusBox : MonoBehaviour {

    public Camp playerCamp; // inspector set
    public Giant playerGiant; // inspector set

    public HUDGauge[] resourceGauges; // inspector set
    public HUDGauge[] statGauges; // inspector set

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        UpdateResourceGauges();
	}

    public void UpdateStatGauges()
    {
        statGauges[0].SetValue(playerGiant.stats.atk, 0, true);
        statGauges[1].SetValue(playerGiant.stats.atkspd, 0, true);
        statGauges[2].SetValue(playerGiant.stats.def, 0, true);
        statGauges[3].SetValue(playerGiant.stats.maxHP, 0, true);
        statGauges[4].SetValue(playerGiant.stats.hpPerSec, 0, true);
    }

    public void UpdateResourceGauges()
    {
        resourceGauges[0].SetValue(playerCamp.resources.meat, 0, true);
        resourceGauges[1].SetValue(playerCamp.resources.stone, 0, true);
        resourceGauges[2].SetValue(playerCamp.resources.water, 0, true);
        resourceGauges[3].SetValue(playerCamp.resources.wheat, 0, true);
        resourceGauges[4].SetValue(playerCamp.resources.wood, 0, true);
    }
}
