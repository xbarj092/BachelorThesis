using DG.Tweening;
using System.Collections;
using UnityEngine;

public class FloatingComponent : MonoBehaviour
{
    [SerializeField] protected float _floatDistance = 50f;
    [SerializeField] protected float _floatDuration = 1f;
    [SerializeField] protected float _floatDelay = 1.2f;
    [SerializeField] protected float _fadeDuration = 0.5f;

    private void Start()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);

        Destroy(gameObject, _floatDelay + _floatDuration + _fadeDuration);
        StartCoroutine(FloatAndFade());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    protected IEnumerator FloatAndFade()
    {
        yield return new WaitForSeconds(_floatDelay);

        yield return StartCoroutine(Float());
        yield return StartCoroutine(Fade());
    }

    protected IEnumerator Float()
    {
        yield return transform.DOMoveY(transform.position.y + _floatDistance, _floatDuration).SetEase(Ease.OutSine).WaitForCompletion();
    }

    protected virtual IEnumerator Fade()
    {
        yield return null;
        Destroy(gameObject);
    }
}