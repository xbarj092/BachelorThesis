using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FloatingImage : FloatingComponent
{
    [SerializeField] private Image _image;

    public void Init(Sprite sprite)
    {
        _image.sprite = sprite;
        StartCoroutine(FloatAndFade());
    }

    protected override IEnumerator Fade()
    {
        float elapsedTime = 0f;
        Color startColor = _image.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsedTime < _fadeDuration)
        {
            _image.color = Color.Lerp(startColor, endColor, elapsedTime / _fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _image.color = endColor;
    }
}
