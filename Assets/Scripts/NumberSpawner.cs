using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NumberSpawner : MonoBehaviour {

    public GameObject numberObject; // inspector set

    public void SpawnNumber(Giant giant, bool damage, int value)
    {
        Color color;

        GameObject instantiatedNumber = Instantiate(numberObject, giant.transform) as GameObject;

        instantiatedNumber.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 1);

        if (damage)
            color = Color.red;
        else
            color = Color.green;

        Text instantiatedNumberText = instantiatedNumber.GetComponent<Text>();

        instantiatedNumberText.color = color;
        instantiatedNumberText.text = value.ToString();
    }

}
