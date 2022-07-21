using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class HookView : MonoBehaviour
{
    public event Action HookStopped;
    public event Action HookReturned;

    [SerializeField] private SpriteRenderer _hookSpriteRenderer;

    [SerializeField] private SpriteRenderer _kernelSpriteRenderer;

    private static readonly Vector2 BasePosition = new Vector3(0, 1, 0);
    private const float ReachBlockLength = 0.44f;
    private const float LengthPerBlock = 3.33f;

    // Start is called before the first frame update
    public void RunForward(int numberOfBlocks)
    {
        // Grab a free Sequence to use
        float newPosition = BasePosition.y + ReachBlockLength + LengthPerBlock * numberOfBlocks;

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(_hookSpriteRenderer.transform.DOLocalMoveY(newPosition, 0.1f, false).SetEase(Ease.Linear));
        mySequence.Append(_hookSpriteRenderer.transform.DOLocalMoveY(BasePosition.y, 0.1f, false).SetEase(Ease.Linear));

        Sequence mySequence2 = DOTween.Sequence();
        mySequence2.Append(_kernelSpriteRenderer.transform.DOScaleY(newPosition, 0.1f).SetEase(Ease.Linear));
        mySequence2.Append(_kernelSpriteRenderer.transform.DOScaleY(1, 0.1f).SetEase(Ease.Linear));
    }
}