using UnityEngine;
using System.Collections.Generic;

public class HUDManager : MonoBehaviour {

    public GameObject weaponButton;
    public GameObject armourButton;
    public GameObject consumableButton;

    public void ShowChildren(GameObject parent)
    {
        List<GameObject> children = GetChildren(parent);

        foreach (GameObject c in children)
        {
            Animation cAnimation = c.GetComponent<Animation>();
            if (cAnimation)
            {
                cAnimation.Play();
            }
        }
    }

    List<GameObject> GetChildren(GameObject parent)
    {
        List<GameObject> children = new List<GameObject>();
        for (int i = 0; i < children.Count; i++)
        {
            children.Add(parent.transform.GetChild(i).gameObject);
        }
        return children;
    }

    void GetOtherParents(GameObject parent)
    {
        if (parent == weaponButton)
        {
            HideChildren(armourButton, consumableButton);
        }
        else if (parent == armourButton)
        {
            HideChildren(weaponButton, consumableButton);
        }
        else if (parent == consumableButton)
        {
            HideChildren(weaponButton, armourButton);
        }
    }

    void HideChildren(GameObject parent1, GameObject parent2)
    {
        List<GameObject> children = new List<GameObject>();
    }
}
