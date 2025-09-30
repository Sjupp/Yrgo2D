using UnityEngine;
using UnityEngine.UI;

public class ColorChanger : MonoBehaviour
{
    public Color ColorA = Color.white;
    public Color ColorB = Color.black;

    //private Image _sprite = null;
    private SpriteRenderer _spriteRenderer = null;

    private void Start()
    {
        //_sprite = GetComponent<Image>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ChangeColor(float normalizedValue)
    {
        //_sprite.color = Color.Lerp(ColorA, ColorB, normalizedValue);
        _spriteRenderer.color = Color.Lerp(ColorA, ColorB, normalizedValue);
    }
}
