using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingText : FloatingComponent
{
    [SerializeField] private TMP_Text _text;

    public void Init(float amount)
    {
        if (amount == 0)
        {
            Destroy(gameObject);
            return;
        }

        _text.color = amount < 0 ? Color.red : Color.green;
        Init(amount < 0 ? amount.ToString() : "+" + amount.ToString());
    }

    public void Init(string text)
    {
        _text.text = text;
        StartCoroutine(FloatAndFade());
    }

    protected override IEnumerator Fade()
    {
        yield return _text.DOFade(0f, _fadeDuration).SetEase(Ease.InQuad).WaitForCompletion();
        yield return StartCoroutine(base.Fade());
    }
}
