using System;
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

    [SerializeField] private GameObject Tutorial1;

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
}