using UnityEngine;

[RequireComponent(typeof(RectTransform))]

public class AspectRatioLimiter : MonoBehaviour
{
    [SerializeField] private float maxAspectRatio = 1.7778f; // Example: 16:9

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        AdjustWidthByAspect();
    }

    private void AdjustWidthByAspect()
    {
        // Get the current aspect ratio of the UI element
        float currentAspectRatio = rectTransform.rect.width / rectTransform.rect.height;

        if (currentAspectRatio <= maxAspectRatio)
        {
            // If the current aspect ratio is less than or equal to the threshold, match the screen width
            float desiredWidth = Screen.width * (rectTransform.anchorMax.x - rectTransform.anchorMin.x);
            rectTransform.sizeDelta = new Vector2(desiredWidth, rectTransform.sizeDelta.y);
        }
        // If the aspect ratio exceeds the threshold, the width remains the same.
    }
}