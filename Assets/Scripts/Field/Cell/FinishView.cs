using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishView : MonoBehaviour
{
    private static readonly Color colorRed = new Color(0.717f, 0.159437f, 0);
    private static readonly Color colorGreen = new Color(0, 0.7176471f, 0.1244776f);
    private static readonly Color colorBlue = new Color(0, 0.3267215f, 0.7176471f);

    private Color _currentColor;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    public void SetColor(ColorType color)
    {
        switch (color)
        {
            case ColorType.Red:
            {
                _currentColor = colorRed;
                break;
            }
            case ColorType.Green:
            {
                _currentColor = colorGreen;
                break;
            }
            default:
            {
                _currentColor = colorBlue;
                break;
            }
        }

        _spriteRenderer.color = _currentColor;
    }
}
