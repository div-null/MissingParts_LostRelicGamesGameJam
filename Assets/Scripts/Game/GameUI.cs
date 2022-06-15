using System;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public event Action RestartClicked;
    public event Action MuteSoundClicked;

    [SerializeField] private AudioSource _audio;

    [SerializeField] private Button _restartButton;

    private void Awake()
    {
        _restartButton.onClick.AddListener(OnRestartClicked);
    }

    public void DebugMe()
    {
        Debug.Log("ASD");
    }

    private void OnRestartClicked()
    {
        RestartClicked?.Invoke();
    }
}