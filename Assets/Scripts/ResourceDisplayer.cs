using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ResourceDisplayer : MonoBehaviour {

    public GameObject giant;
    public int resource;
    private GameObject child;

	void Start () {
        child = transform.GetChild(0).gameObject;
	}

    void Update() {
        float resourceAmount = 0;
        switch (resource) {
            case 0:
                resourceAmount = giant.GetComponent<Giant>().camp.GetComponent<Camp>().resources.wheat;
                break;
            case 1:
                resourceAmount = giant.GetComponent<Giant>().camp.GetComponent<Camp>().resources.meat;
                break;
            case 2:
                resourceAmount = giant.GetComponent<Giant>().camp.GetComponent<Camp>().resources.water;
                break;
            case 3:
                resourceAmount = giant.GetComponent<Giant>().camp.GetComponent<Camp>().resources.wood;
                break;
            case 4:
                resourceAmount = giant.GetComponent<Giant>().camp.GetComponent<Camp>().resources.stone;
                break;
        }
        child.GetComponent<Image>().fillAmount = resourceAmount / 20;

    }
}
