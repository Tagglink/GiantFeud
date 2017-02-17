using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class HUDGauge : MonoBehaviour {

    public float maxValue; // inspector set

    public Image[] gauges; // inspector set
    public Text textValue; // inspector set

    // Use this for initialization
    void Start() {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    Image[] GetGauges()
    {
        List<Image> imgs = new List<Image>();
        Image img;

        for (int i = 2; i < transform.childCount; i++)
        {
            img = transform.GetChild(i).GetComponent<Image>();

            if (img)
                imgs.Add(img);
        }

        return imgs.ToArray();
    }

    public void SetValue(float value, int gauge)
    {
        SetValue(value, gauge, false);
    }

    public void SetValue(float value, int gauge, bool text)
    {
        Image img;

        try
        {
            img = gauges[gauge];
        } catch (IndexOutOfRangeException e)
        {
            Debug.Log(e.Message);
            return;
        }

        img.fillAmount = value / maxValue;

        if (text)
            textValue.text = "" + value;
    }

    public void SetColor(Color color, int gauge)
    {
        Image img;

        try
        {
            img = gauges[gauge];
        } catch (IndexOutOfRangeException e)
        {
            Debug.Log(e.Message);
            return;
        }

        img.color = color;
    }
}
