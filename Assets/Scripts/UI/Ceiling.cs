using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ceiling : MonoBehaviour
{
    public event Action OnFadeOut;
    public event Action OnFadeIn;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Image image; 

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
        OnFadeIn?.Invoke();
    }

    private void FadeOutStopped()
    {
        image.enabled = false;
        OnFadeOut?.Invoke();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
