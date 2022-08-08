using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Ceiling : MonoBehaviour
    {
        public ReactiveCommand OnFadeOut = new ReactiveCommand();
        public ReactiveCommand OnFadeIn = new ReactiveCommand();

        [SerializeField] private Animator animator;

        [SerializeField] private Image image;

        private static readonly int In = Animator.StringToHash("fadeIn");
        private static readonly int Out = Animator.StringToHash("fadeOut");


        public void FadeIn()
        {
            image.enabled = true;
            animator.SetTrigger(In);
        }

        public void FadeOut()
        {
            animator.SetTrigger(Out);
        }

        private void FadeInStopped()
        {
            OnFadeIn.Execute();
        }

        private void FadeOutStopped()
        {
            image.enabled = false;
            OnFadeOut.Execute();
        }
    }
}