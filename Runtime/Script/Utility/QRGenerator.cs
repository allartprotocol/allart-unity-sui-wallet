using UnityEngine;
using ZXing;
using ZXing.QrCode;

public static class QRGenerator
{
    private static Color32[] Encode(string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }

    public static Texture2D GenerateQR(string text, int width, int height)
    {
        var color32 = Encode(text, width, height);
        var tex = new Texture2D(width, height);
        tex.SetPixels32(color32);
        tex.Apply();
        return tex;
    }

    public static Sprite GenerateQRSprite(string text, int width, int height)
    {
        var tex = GenerateQR(text, width, height);
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
    }

}
