using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Ceiling : MonoBehaviour
    {
        private static readonly int In = Animator.StringToHash("fadeIn");
        private static readonly int Out = Animator.StringToHash("fadeOut");

        public ReactiveCommand OnFadeOut = new ReactiveCommand();
        public ReactiveCommand OnFadeIn = new ReactiveCommand();

        [SerializeField] private Animator animator;

        [SerializeField] private Image image;


        public bool Faded { get; private set; }

        private void Awake()
        {
            Faded = image.enabled;
        }

        public void MakeTransition(Action onFadeIn, Action onFadeOut)
        {
            CompositeDisposable disposable = new CompositeDisposable();

            OnFadeIn.Subscribe(_ =>
            {
                onFadeIn.Invoke();
                FadeOut();
            }).AddTo(disposable);

            OnFadeOut.Subscribe(_ =>
            {
                onFadeOut.Invoke();
                disposable.Dispose();
            }).AddTo(disposable);
            FadeIn();
        }

        public void FadeIn()
        {
            if (Faded)
            {
                OnFadeIn.Execute();
                return;
            }

            image.enabled = true;
            animator.SetTrigger(In);
        }

        public void FadeOut()
        {
            if (!Faded)
            {
                OnFadeOut.Execute();
                return;
            }

            animator.SetTrigger(Out);
        }

        private void FadeInStopped()
        {
            Faded = true;
            OnFadeIn.Execute();
        }

        private void FadeOutStopped()
        {
            Faded = false;
            image.enabled = false;
            OnFadeOut.Execute();
        }
    }
}