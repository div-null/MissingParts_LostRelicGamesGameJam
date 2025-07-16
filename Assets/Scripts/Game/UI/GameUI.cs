using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
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

            int tutorialNum =
                levelNumber switch
                {
                    2 => 1,
                    3 => 2,
                    6 => 3,
                    7 => 4,
                    9 => 5,
                    11 => 6,
                    12 => 7,
                    13 => 8,
                    _ => 0
                };

            if (tutorialNum != 0)
                UnlockTutorial(tutorialNum);
        }

        private void UnlockTutorial(int tutorialNumber)
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
            TutorialPillsPanel.SetActive(!TutorialPillsPanel.activeSelf);
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
}