using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{

    [SerializeField]
    private Transform[] _tutorialScreens;

    [SerializeField]
    private GameObject _previousButton;

    [SerializeField]
    private GameObject _nextButton;



    private int _tutorialScreensIndex = 0;

    public int TutorialScreensIndex
    {
        get
        {
            return _tutorialScreensIndex;
        }

        set
        {
            if (value >= _tutorialScreens.Length)
                return;

            if (value < 0)
                return;

            if (value == 0)
                _previousButton.SetActive(false);
            else
                _previousButton.SetActive(true);

            if (value == _tutorialScreens.Length - 1)
                _nextButton.SetActive(false);
            else
                _nextButton.SetActive(true);

            _tutorialScreens[_tutorialScreensIndex].gameObject.SetActive(false);
            _tutorialScreensIndex = value;
            _tutorialScreens[_tutorialScreensIndex].gameObject.SetActive(true);
        }
    }

    public void NextTutorialScreen()
    {
        TutorialScreensIndex++;
    }
    public void PreviousTutorialScreen()
    {
        TutorialScreensIndex--;
    }
    void Awake()
    {

    }

    void Start()
    {
        TutorialScreensIndex = 0;
    }

    void Update()
    {

    }
}
