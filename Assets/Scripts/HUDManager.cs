﻿using UnityEngine;
using System.Collections.Generic;

public class HUDManager : MonoBehaviour {

    public GameObject weaponButton;     // inspector set
    public GameObject armourButton;     // inspector set
    public GameObject consumableButton; // inspector set
    
    public Camp playerCamp;             // inspector set

    void Update()
    {
        
    }

    public void ToggleState(GameObject parent)
    {
        //List<GameObject> children = GetChildren(parent);

        //foreach (GameObject c in children)
        //{

        //}

        ToggleOtherParents(parent);
        ToggleMovement(parent);
    }

    void ToggleMovement(GameObject parent)
    {
        parent.GetComponent<CraftingButton>().ToggleMovement();
        parent.transform.SetAsLastSibling();
    }

    //List<GameObject> GetChildren(GameObject parent)
    //{
    //    List<GameObject> children = new List<GameObject>();
    //    for (int i = 0; i < children.Count; i++)
    //    {
    //        children.Add(parent.transform.GetChild(i).gameObject);
    //    }
    //    return children;
    //}

    void ToggleOtherParents(GameObject parent)
    {
        if (parent == weaponButton)
        {
            if (consumableButton.GetComponent<CraftingButton>().state != displayState.HIDDEN)
                ToggleMovement(consumableButton);
            if (armourButton.GetComponent<CraftingButton>().state != displayState.HIDDEN)
                ToggleMovement(armourButton);
        }
        else if (parent == armourButton)
        {
            if (weaponButton.GetComponent<CraftingButton>().state != displayState.HIDDEN)
                ToggleMovement(weaponButton);
            if (consumableButton.GetComponent<CraftingButton>().state != displayState.HIDDEN)
                ToggleMovement(consumableButton);
        }
        else if (parent == consumableButton)
        {
            if (weaponButton.GetComponent<CraftingButton>().state != displayState.HIDDEN)
                ToggleMovement(weaponButton);
            if (armourButton.GetComponent<CraftingButton>().state != displayState.HIDDEN)
                ToggleMovement(armourButton);
        }
    }
}
