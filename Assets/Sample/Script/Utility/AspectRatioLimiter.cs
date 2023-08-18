using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]

public class AspectRatioLimiter : MonoBehaviour
{
    [SerializeField] private float maxAspectRatio = 1.7778f; // Example: 16:9

    public  RectTransform holder;
    public  AspectRatioFitter aspectRatioFitter;
    public CanvasScaler canvasScaler;

    private void Awake()
    {
    }

    private void Update()
    {
        AdjustWidthByAspect();
    }

    private float GetAspectRatio()
    {
        return (float) Screen.width / Screen.height;
    }

    private void AdjustWidthByAspect()
    {
        float aspectRatio = GetAspectRatio();

        if (aspectRatio > maxAspectRatio)
        {
            aspectRatioFitter.enabled = true;
            canvasScaler.matchWidthOrHeight = 1;
        }
        else
        {
            aspectRatioFitter.enabled = false;
            holder.sizeDelta = new Vector2(0, 0);
            canvasScaler.matchWidthOrHeight = 0;
        }
    }
}