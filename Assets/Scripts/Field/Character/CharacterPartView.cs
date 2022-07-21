using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LevelEditor;
using UnityEngine;
using static DG.Tweening.DOTween;

public class CharacterPartView : MonoBehaviour
{
    private static readonly Color activeColor = Color.white;
    private static readonly Color inactiveColor = new Color(0.5f, 0.5f, 0.5f);
    private static readonly Color greenColor = new Color(0, 1, 0.3339117f);
    private static readonly Color blueColor = new Color(0, 0.6480749f, 1);
    private static readonly Color redColor = new Color(1, 0.1322696f, 0);

    private Color currentColor;

    private ColorType _colorType;
    private AbilityType _abilityType;
    private int _spriteNumber;
    private bool _isActive;

    [SerializeField] private SpriteRenderer _mainSpriteRenderer;

    [SerializeField] private SpriteRenderer _lightSpriteRenderer;


    public void Initialize(CharacterPartData partData)
    {
        SetColor(partData.Color);
        _abilityType = partData.Ability;
        _spriteNumber = partData.Sprite;
        SetActive(partData.IsActive);
        ChangeSprite();
    }

    public void ChangeSprite()
    {
        //Assets/Sprites/CharacterPart/Color/default_1

        string color = _colorType.ToString();
        string ability = _abilityType.ToString().ToLower();

        Sprite mainSprite;
        Sprite lightSprite;
        if (TryToLoadSprite(ability, _spriteNumber, out mainSprite, out lightSprite))
        {
            _mainSpriteRenderer.sprite = mainSprite;
            _lightSpriteRenderer.sprite = lightSprite;
        }
        else
        {
            Debug.LogError($"Cant load sprite");
            if (TryToLoadSprite(ability, 1, out mainSprite, out lightSprite))
            {
                _mainSpriteRenderer.sprite = mainSprite;
                _lightSpriteRenderer.sprite = lightSprite;
            }
            else
            {
                Debug.LogError($"Cant load sprite");
            }
        }
    }

    private bool TryToLoadSprite(string ability, int spriteNumber, out Sprite mainSprite, out Sprite lightSprite)
    {
        string pathToMain = $"Sprites/CharacterPart/{ability}/{spriteNumber}";
        string pathToLight = $"Sprites/CharacterPart/{ability}/{spriteNumber}_light";

        mainSprite = Resources.Load<Sprite>(pathToMain);
        lightSprite = Resources.Load<Sprite>(pathToLight);

        return mainSprite != null && lightSprite != null;
    }

    public void SetActive(bool isActive)
    {
        _isActive = isActive;
        if (isActive)
            _lightSpriteRenderer.DOColor(activeColor * currentColor, 0.2f);
        else
            _lightSpriteRenderer.DOColor(inactiveColor * currentColor, 0.2f);
    }

    public void SetRotation(int degrees)
    {
        Vector2Int lookDirection = degrees.ToDirection().ToVector();
        Quaternion quaternion = Quaternion.Euler(0, 0, -degrees);
        //quaternion.
        _mainSpriteRenderer.gameObject.transform.DORotate(quaternion.eulerAngles, 0.1f, RotateMode.Fast);
    }

    public void SetColor(ColorType color)
    {
        switch (color)
        {
            case ColorType.Blue:
            {
                currentColor = blueColor;
                break;
            }
            case ColorType.Green:
            {
                currentColor = greenColor;
                break;
            }
            default:
            {
                currentColor = redColor;
                break;
            }
        }

        _lightSpriteRenderer.DOColor(currentColor, 0.2f);
    }
}