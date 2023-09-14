using System;
using AllArt.SUI.Wallets;
using SimpleScreen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QRScreen : BaseScreen
{
    public Image qrImage;
    public TMP_InputField publicKey;
    public Button copyButton;
    
    private Wallet wallet;

    private void Start()
    {
        copyButton.onClick.AddListener(OnCopy);        
    }

    public override void ShowScreen(object data)
    {
        base.ShowScreen(data);
        this.wallet = (Wallet)data;

        publicKey.text = wallet.publicKey;
        qrImage.sprite = QRGenerator.GenerateQRSprite(wallet.publicKey, 512, 512);
    }

    public void OnCopy()
    {
        GUIUtility.systemCopyBuffer = wallet.publicKey;
        InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Info, "Copied to clipboard");
    }
}
