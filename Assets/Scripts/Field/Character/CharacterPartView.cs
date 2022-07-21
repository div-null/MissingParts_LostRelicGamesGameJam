using DG.Tweening;
using LevelEditor;
using UniRx;
using UnityEngine;

public class CharacterPartView : MonoBehaviour
{
    private static readonly Color activeColor = Color.white;
    private static readonly Color inactiveColor = new Color(0.5f, 0.5f, 0.5f);
    private static readonly Color greenColor = new Color(0, 1, 0.3339117f);
    private static readonly Color blueColor = new Color(0, 0.6480749f, 1);
    private static readonly Color redColor = new Color(1, 0.1322696f, 0);

    private Color currentColor;

    private AbilityType _abilityType;
    private int _spriteNumber;

    [SerializeField] private SpriteRenderer _mainSpriteRenderer;

    [SerializeField] private SpriteRenderer _lightSpriteRenderer;
    private CharacterPart _characterPart;


    public void Initialize(CharacterPart characterPart, CharacterPartData partData)
    {
        _characterPart = characterPart;
        characterPart.ColorChanged.Subscribe(OnColorChanged).AddTo(this);
        characterPart.IsActiveChanged.Subscribe(OnActivate).AddTo(this);
        characterPart.PositionChanged.Subscribe(OnMove).AddTo(this);
        characterPart.LookChanged.Subscribe(OnRotate).AddTo(this);
        characterPart.Deleted.Subscribe(_ => OnDelete()).AddTo(this);

        _abilityType = partData.Ability;
        _spriteNumber = partData.Sprite;
        LoadSprite();
    }

    private void OnMove(Vector2Int newPosition)
    {
        // ???
        // Vector3 newPosition = _field.Get(destination).gameObject.transform.position - Vector3.forward;
        Vector3 endValue = newPosition.ToVector3() + Vector3.back;
        this.transform.DOMove(endValue, 0.1f).SetEase(Ease.Flash);
    }

    private void OnActivate(bool isActive)
    {
        Color color = isActive ? activeColor * currentColor : inactiveColor * currentColor;
        _lightSpriteRenderer.DOColor(color, 0.2f);
    }

    private void OnRotate(DirectionType lookDirection)
    {
        Quaternion quaternion = Quaternion.Euler(0, 0, -lookDirection.Degrees());
        _mainSpriteRenderer.gameObject.transform.DORotate(quaternion.eulerAngles, 0.1f, RotateMode.Fast);
    }

    private void OnColorChanged(ColorType color)
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
            case ColorType.Red:
            {
                currentColor = redColor;
                break;
            }
        }

        _lightSpriteRenderer.DOColor(currentColor, 0.2f);
    }

    private void OnDelete()
    {
        Quaternion quaternion = Quaternion.Euler(0, 0, -180);
        transform.DORotate(quaternion.eulerAngles, 0.25f, RotateMode.Fast).SetLoops(3).SetEase(Ease.Linear);
        transform.DOScale(0.01f, 0.5f).SetEase(Ease.Linear);
        Destroy(this.gameObject, 0.6f);
    }

    private void LoadSprite()
    {
        //Assets/Sprites/CharacterPart/Color/default_1

        string color = _characterPart.Color.ToString();
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
}