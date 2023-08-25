using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

public class QRReader : MonoBehaviour
{

    private WebCamTexture camTexture;
    
    public RawImage feed;
    private Coroutine tryScanRoutine;

    public Action<string> OnQRRead;
    public AspectRatioFitter imageFitter;

    // Image rotation
    Vector3 rotationVector = new Vector3(0f, 0f, 0f);

    // Image uvRect
    Rect defaultRect = new Rect(0f, 0f, 1f, 1f);
    Rect fixedRect = new Rect(0f, 1f, 1f, -1f);

    // Image Parent's scale
    Vector3 defaultScale = new Vector3(1f, 1f, 1f);
    Vector3 fixedScale = new Vector3(-1f, 1f, 1f);

    public void StartFeed()
    {
        camTexture = new WebCamTexture
        {
            requestedHeight = (int)feed.rectTransform.rect.height,
            requestedWidth = (int)feed.rectTransform.rect.width
        };
        camTexture?.Play();

        rotationVector.z = -camTexture.videoRotationAngle;
        feed.rectTransform.localEulerAngles = rotationVector;

        // Set AspectRatioFitter's ratio
        float videoRatio = 
            (float)camTexture.width / (float)camTexture.height;
        imageFitter.aspectRatio = videoRatio;

        // Unflip if vertically flipped
        feed.uvRect = 
            camTexture.videoVerticallyMirrored ? fixedRect : defaultRect;


        feed.texture = camTexture;
        tryScanRoutine = StartCoroutine(ScanRoutine());
    }

    public void StopFeed()
    {
        camTexture?.Stop();
        if(tryScanRoutine != null)
            StopCoroutine(tryScanRoutine);
    }

    IEnumerator ScanRoutine(){
        while(true){
            yield return new WaitForSeconds(1f);
            TryReadQR();
        }
    }

    void TryReadQR(){
        try
        {
            IBarcodeReader barcodeReader = new BarcodeReader();
            // decode the current frame
            var result = barcodeReader.Decode(camTexture.GetPixels32(),
              camTexture.width, camTexture.height);
            if (result != null)
            {
                Debug.Log("DECODED TEXT FROM QR: " + result.Text);
                OnQRRead?.Invoke(result.Text);
            }
        }
        catch (Exception ex) { Debug.LogWarning(ex.Message); }
    }
}
