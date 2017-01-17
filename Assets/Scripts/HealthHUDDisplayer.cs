using UnityEngine;
using System;
using UnityEngine.UI;

public class HealthHUDDisplayer : MonoBehaviour {

    public GameObject giantObject;
    private Giant giant;

	void Start () {
        giant = giantObject.GetComponent<Giant>();
    }
	
	void Update () {
        //currentHealth = int.Parse(sep(GetComponentInChildren<Text>().text));
        float healthPercentage = (float)giant.stats.hp / giant.stats.maxHP;
        GetComponent<Image>().fillAmount = healthPercentage;
	}

    //public static string sep(string s)
    //{
    //    int l = s.IndexOf(" /");
    //    if (l > 0)
    //    {
    //        return s.Substring(0, l);
    //    }
    //    return "";

    //}
}
