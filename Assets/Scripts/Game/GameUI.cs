using System;
using LevelEditor;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public event Action RestartClicked;
    public event Action MuteSoundClicked;

    public event Action<int> ChooseExtraLevel;

    [SerializeField] private AudioSource _audio;

    [SerializeField] private Button _restartButton;

    [SerializeField] private GameObject _menu;

    [SerializeField] private Animator _mainMenuAnimator;

    [SerializeField] private GameObject Credits;

    [SerializeField] private TextMeshProUGUI LevelNumber;

    [SerializeField] private GameObject TutorialPillsPanel;

    [Header("Tutorials")] [SerializeField] private GameObject[] Tutorials = new GameObject[8];

    [Header("TutorialsPills")] [SerializeField]
    private GameObject[] TutorialPills = new GameObject[8];

    [SerializeField] private GameObject emptyTutorialsText;
    [SerializeField] private Button closeButton;


    private void Awake()
    {
        _restartButton.onClick.AddListener(OnRestartClicked);

        DeactivateCloseButton();
    }

    private void OnRestartClicked()
    {
        RestartClicked?.Invoke();
    }

    public void HideMenu()
    {
        _mainMenuAnimator.SetTrigger("StartFade");
    }

    public void ShowCredits()
    {
        _menu.SetActive(false);
        Credits.SetActive(true);
    }

    public void ToNextLevel(int levelNumber)
    {
        LevelNumber.text = $"Level {levelNumber}/{17}";

        switch (levelNumber)
        {
            case 2:
                UnlockTutorial(1);
                break;
            case 3:
                UnlockTutorial(2);
                break;
            case 6:
                UnlockTutorial(3);
                break;
            case 7:
                UnlockTutorial(4);
                break;
            case 9:
                UnlockTutorial(5);
                break;
            case 11:
                UnlockTutorial(6);
                break;
            case 12:
                UnlockTutorial(7);
                break;
            case 13:
                UnlockTutorial(8);
                break;
            default:
                break;
        }
    }

    public void UnlockTutorial(int tutorialNumber)
    {
        if (tutorialNumber >= Tutorials.Length) tutorialNumber = Tutorials.Length;
        OpenTutorial(tutorialNumber);
        TutorialPills[tutorialNumber - 1].SetActive(true);
        if (emptyTutorialsText.activeSelf)
            emptyTutorialsText.SetActive(false);
    }

    public void OpenTutorial(int tutorialNumber)
    {
        Tutorials[tutorialNumber - 1].SetActive(true);
    }

    public void ClickQuestion()
    {
        if (TutorialPillsPanel.activeSelf)
        {
            TutorialPillsPanel.SetActive(false);
        }
        else
        {
            TutorialPillsPanel.SetActive(true);
        }
    }

    public void ClickExtraLevel(int level)
    {
        _menu.SetActive(true);
        Credits.SetActive(false);
        ChooseExtraLevel?.Invoke(level);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void DeactivateCloseButton()
    {
#if PLATFORM_STANDALONE || PLATFORM_STANDALONE_WIN || UNITY_EDITOR || UNITY_EDITOR_64
        closeButton.gameObject.SetActive(true);
#endif
    }
}