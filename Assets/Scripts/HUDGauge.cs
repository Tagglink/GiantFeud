using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class HUDGauge : MonoBehaviour {

    public float maxValue; // inspector set

    Image[] gauges;
    Text textValue;

    // Use this for initialization
    void Start() {
        gauges = GetGauges();
        textValue = transform.Find("Value").GetComponent<Text>();
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

        img.fillAmount = maxValue / value;

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
