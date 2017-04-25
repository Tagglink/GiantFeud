using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HUDTutorial : MonoBehaviour {

    public RectTransform textbox; // inspector set
    public RectTransform arrow;   // inspector set
    public Text textboxText;      // inspector set

    public int currentStep;

    List<TutorialStep> stepList;

    struct TutorialStep
    {
        public int arrowPositionX { get; set; }
        public int arrowPositionY { get; set; }
        public int textboxPositionX { get; set; }
        public int textboxPositionY { get; set; }
        public string textboxText { get; set; }
        public int textboxWidth { get; set; }
        public int textboxHeight { get; set; }

        public TutorialStep(int _arrowPositionX, int _arrowPositionY, int _textboxPositionX, int _textboxPositionY, string _textboxText, int _textboxWidth, int _textboxHeight)
        {
            arrowPositionX = _arrowPositionX;
            arrowPositionY = _arrowPositionY;
            textboxPositionX = _textboxPositionX;
            textboxPositionY = _textboxPositionY;
            textboxText = _textboxText;
            textboxWidth = _textboxWidth;
            textboxHeight = _textboxHeight;
        }
    }

    // Use this for initialization
    void Start() {
        stepList = new List<TutorialStep>()
        {
            new TutorialStep(233, 198, -105, 228, "Klicka på träden för att samla trä!", 560, 75),
            new TutorialStep(-641, -72, -382, -59, "Klicka här för att crafta!", 400, 75),
            new TutorialStep(-640, 75, -382, 31, "Crafta ett äpple!", 285, 75),
            new TutorialStep(-640, 75, -382, 31, "Klicka igen för att ge äpplet till jätten!", 340, 120),
            new TutorialStep(-3000, -3000, -3000, -3000, "Kanna is cute", 500, 500)
        };

        currentStep = 1;
        ApplyTutorialStep(stepList[0]);
	}

    public void AdvanceTutorial()
    {
        if (stepList.Count > currentStep)
        {
            ApplyTutorialStep(stepList[currentStep]);
            currentStep++;
        }
    }

    void ApplyTutorialStep(TutorialStep step)
    {
        textbox.anchoredPosition = new Vector2(step.textboxPositionX, step.textboxPositionY);
        arrow.anchoredPosition = new Vector2(step.arrowPositionX, step.arrowPositionY);
        textbox.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, step.textboxWidth);
        textbox.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, step.textboxHeight);
        textboxText.text = step.textboxText;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
