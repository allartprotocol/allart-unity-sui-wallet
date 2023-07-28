using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UITween : MonoBehaviour, IScreenAnimation
{
    [SerializeField] private RectTransform targetRectTransform;
    [SerializeField] private float duration = 1f;
    [SerializeField] private AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [SerializeField] public Vector2 startPosition;
    [SerializeField] private Vector2 endPosition;
    [SerializeField] private Vector3 startScale;
    [SerializeField] private Vector3 endScale;
    [SerializeField] private float startAlpha;
    [SerializeField] private float endAlpha;

    public RectTransform inTransform;
    public RectTransform outTransform;

    CanvasGroup canvasGroup;

    private void Awake()
    {
        if (targetRectTransform == null)
        {
            targetRectTransform = GetComponent<RectTransform>();
        }

        if(canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        startPosition = targetRectTransform.anchoredPosition;
        startScale = targetRectTransform.localScale;
        startAlpha = canvasGroup.alpha;
    }

    public void PlayInstant()
    {
        startPosition = targetRectTransform.anchoredPosition;
        endPosition = outTransform.anchoredPosition;
        endAlpha = 0f;
        targetRectTransform.anchoredPosition = endPosition;
        targetRectTransform.localScale = endScale;
        canvasGroup.alpha = endAlpha;
    }

    public void TweenIn()
    {
        startPosition = inTransform.anchoredPosition;
        endPosition = Vector2.zero;
        endAlpha = 1f;
        startAlpha = 0f;
        PlayTween();
    }

    public void TweenOut()
    {
        startPosition = targetRectTransform.anchoredPosition;
        endPosition = outTransform.anchoredPosition;
        endAlpha = 0f;
        startAlpha = 1f;

        PlayTween();
    }

    public void TweenPosition(Vector2 targetPosition)
    {
        startPosition = targetRectTransform.anchoredPosition;
        endPosition = targetPosition;

        PlayTween();
    }

    public void TweenScale(Vector3 targetScale)
    {
        startScale = targetRectTransform.localScale;
        endScale = targetScale;

        PlayTween();
    }

    public void TweenAlpha(float targetAlpha)
    {
        startAlpha = canvasGroup.alpha;
        endAlpha = targetAlpha;

        PlayTween();
    }

    private void PlayTween()
    {
        StopAllCoroutines();
        StartCoroutine(TweenCoroutine());
    }

    private IEnumerator TweenCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = curve.Evaluate(elapsed / duration);

            targetRectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t);
            targetRectTransform.localScale = Vector3.Lerp(startScale, endScale, t);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);

            elapsed += Time.deltaTime;

            yield return null;
        }

        // Ensure that the target values are set exactly when the tween finishes
        canvasGroup.alpha = endAlpha;
        targetRectTransform.anchoredPosition = endPosition;
        targetRectTransform.localScale = endScale;
    }
}
