using System.Collections;
using System.Collections.Generic;
using LevelEditor;
using UnityEngine;

public class CharacterPartView : MonoBehaviour
{
    private ColorType _colorType;
    private AbilityType _abilityType;
    private int _spriteNumber;
    private bool _isActive;

    private SpriteRenderer _spriteRenderer;
    
    // Start is called before the first frame update
    void Awake()
    {
        _spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(CharacterPartData partData)
    {
        _colorType = partData.Color;
        _abilityType = partData.Ability;
        _spriteNumber = partData.Sprite;
        _isActive = partData.IsActive;
        ChangeSprite();
    }

    public void ChangeSprite()
    {
        //Assets/Sprites/CharacterPart/Color/default_1
        
        string color = _colorType.ToString();
        string ability = _abilityType.ToString().ToLower();
        string pathToFile = $"Sprites/CharacterPart/{color}/{ability}_";

        Sprite sprite = Resources.Load<Sprite>($"{pathToFile}{_spriteNumber}");
        if (sprite == null)
        {
            sprite = Resources.Load<Sprite>($"{pathToFile}1") as Sprite;
            Debug.LogError($"Cant load sprite in path: {pathToFile}{_spriteNumber}");
        }
        
        if (sprite == null)
        {
            Debug.LogError($"Cant load sprite in path: {pathToFile}1");
            return;
        }

        _spriteRenderer.sprite = sprite;
    }

    public void SetActive(bool isActive)
    {
        if (isActive)
            _spriteRenderer.color = UnityEngine.Color.white;
        else
            _spriteRenderer.color = UnityEngine.Color.grey;
    }
}
