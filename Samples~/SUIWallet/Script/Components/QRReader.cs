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

    public void StartFeed()
    {
        camTexture = new WebCamTexture
        {
            requestedHeight = (int)feed.rectTransform.rect.height,
            requestedWidth = (int)feed.rectTransform.rect.width
        };
        camTexture?.Play();

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
