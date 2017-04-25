using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {

    public HUDTutorial tutorial;        // inspector set

    public GameObject weaponButton;     // inspector set
    public GameObject armourButton;     // inspector set
    public GameObject consumableButton; // inspector set
    
    public Camp playerCamp;             // inspector set

    public GameObject townName;         // inspector set

    void Start()
    {
        if (PlayerPrefs.HasKey("Town Name"))
            townName.GetComponent<Text>().text = PlayerPrefs.GetString("Town Name");
        else
            townName.GetComponent<Text>().text = "Your Town";
    }

    void Update()
    {
        
    }

    public void ToggleState(GameObject parent)
    {
        if (tutorial.currentStep == 2 && parent == consumableButton) {
            tutorial.AdvanceTutorial();
        }

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
