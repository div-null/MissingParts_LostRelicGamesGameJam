using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public event Action RestartClicked;
    public event Action MuteSoundClicked;

    [SerializeField] private AudioSource _audio;

    [SerializeField] private Button _restartButton;

    [SerializeField] private Animator MainMenuAnimator;

    [SerializeField] private GameObject Credits;

    [SerializeField] private TextMeshProUGUI LevelNumber;

    [SerializeField] private GameObject Tutorial1;
    [SerializeField] private GameObject Tutorial2;
    [SerializeField] private GameObject Tutorial3;
    [SerializeField] private GameObject Tutorial4;
    [SerializeField] private GameObject Tutorial5;
    [SerializeField] private GameObject Tutorial6;
    [SerializeField] private GameObject Tutorial7;
    [SerializeField] private GameObject Tutorial8;
    [SerializeField] private GameObject Tutorial9;

    private void Awake()
    {
        _restartButton.onClick.AddListener(OnRestartClicked);
    }

    private void OnRestartClicked()
    {
        RestartClicked?.Invoke();
    }
    
    public void HideMenu()
    {
        MainMenuAnimator.SetTrigger("StartFade");
    }

    public void ShowCredits()
    {
        Credits.SetActive(true);
    }

    public void ToNextLevel(int levelNumber)
    {
        LevelNumber.text = $"Level_{levelNumber}";

        switch (levelNumber)
        {
            case 2:
                Tutorial1.SetActive(true);
                break;
            case 3:
                Tutorial2.SetActive(true);
                break;
            case 6:
                Tutorial3.SetActive(true);
                break;
            case 7:
                Tutorial4.SetActive(true);
                break;
            case 9:
                Tutorial5.SetActive(true);
                break;
            case 10:
                Tutorial6.SetActive(true);
                break;
            case 11:
                Tutorial7.SetActive(true);
                break;
            case 12:
                Tutorial8.SetActive(true);
                break;
            case 13:
                Tutorial9.SetActive(true);
                break;
        }
        if (levelNumber == 2)
        {
            Tutorial1.SetActive(true);
        }
    }
}