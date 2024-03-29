using Game.Character;
using UnityEngine;

namespace Game.Cell
{
    public class FinishView : MonoBehaviour
    {
        private static readonly Color colorRed = new Color(1, 0.079f, 0);
        private static readonly Color colorGreen = new Color(0, 0.92f, 0.18f);
        private static readonly Color colorBlue = new Color(0, 0.666f, 1);

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
}
