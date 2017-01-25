using UnityEngine;
using System.Collections.Generic;

public class HUDManager : MonoBehaviour {

    public GameObject weaponButton;
    public GameObject armourButton;
    public GameObject consumableButton;

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
            if (!consumableButton.GetComponent<CraftingButton>().retracted)
                ToggleMovement(consumableButton);
            if (!armourButton.GetComponent<CraftingButton>().retracted)
                ToggleMovement(armourButton);
        }
        else if (parent == armourButton)
        {
            if (!weaponButton.GetComponent<CraftingButton>().retracted)
                ToggleMovement(weaponButton);
            if (!consumableButton.GetComponent<CraftingButton>().retracted)
                ToggleMovement(consumableButton);
        }
        else if (parent == consumableButton)
        {
            if (!weaponButton.GetComponent<CraftingButton>().retracted)
                ToggleMovement(weaponButton);
            if (!armourButton.GetComponent<CraftingButton>().retracted)
                ToggleMovement(armourButton);
        }
    }
}
