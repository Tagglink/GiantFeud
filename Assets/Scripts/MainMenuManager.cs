using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {

    public GameObject townNameObject;     // inspector set
    private Text townName;

	void Start () {
        townName = townNameObject.GetComponent<Text>();
	}

    public void StartGame()
    {
        if (townName.text.Length > 0)
            PlayerPrefs.SetString("Town Name", townName.text);
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
