using SimpleScreen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SendScreen : BaseScreen
{
    public TextMeshProUGUI tokenName;

    public Image tokenImage;

    public TMP_InputField to;
    public TMP_InputField amount;

    public Button continueBtn;
    public Button tokenSelectBtn;
    public Button closeButton;
    public Button scanButton;
    public Transform feed;


    public Button max;

    Balance balance;
    private QRReader qrReader;

    private void Start()
    {
        continueBtn.onClick.AddListener(OnContinue);
        tokenSelectBtn.onClick.AddListener(OnTokenSelect);
        closeButton.onClick.AddListener(OnClose);
        scanButton.onClick.AddListener(OnScan);
        max.onClick.AddListener(OnMax);

        
        qrReader = FindObjectOfType<QRReader>();
        qrReader.OnQRRead += OnQRCodeFound;
    }

    private void OnQRCodeFound(string obj)
    {
        to.text = obj;
        Debug.Log("QR Code Found: " + obj);
        OnClose();
    }

    private void OnScan()
    {
        feed.gameObject.SetActive(true);
        qrReader.StartFeed();
    }

    private void OnClose()
    {
        feed.gameObject.SetActive(false);
        qrReader.StopFeed();
    }

    private void OnMax()
    {
        var meta = WalletComponent.Instance.coinMetadatas.Where(x => x.Value.symbol == WalletComponent.Instance.currentCoinMetadata.symbol).FirstOrDefault();
        if(meta.Value == null)
        {
            return;
        }
        var coinType = meta.Value;
        amount.text = (balance.totalBalance / (Mathf.Pow(10, coinType.decimals))).ToString();
    }

    private void OnTokenSelect()
    {
        GoTo("TokenSelectScreen");
    }

    public override async void ShowScreen(object data = null)
    {
        base.ShowScreen(data);
        amount.text = "0";
        tokenName.text = WalletComponent.Instance.currentCoinMetadata.name;
        var coinType = WalletComponent.Instance.coinMetadatas.Where(x => x.Value.symbol == WalletComponent.Instance.currentCoinMetadata.symbol).FirstOrDefault().Key;
        balance = await WalletComponent.Instance.GetBalance(WalletComponent.Instance.currentWallet.publicKey, 
            coinType);

        var tokenImgComponent = GetComponentInChildren<TokenImage>();
        WalletComponent.Instance.coinImages.TryGetValue(WalletComponent.Instance.currentCoinMetadata.symbol, out Sprite image);
        tokenImgComponent.Init(image, WalletComponent.Instance.currentCoinMetadata.name);
    }

    override public void HideScreen()
    {
        base.HideScreen();
        OnClose();
    }

    private void OnContinue()
    {
        GoTo("SendConfirmScreen", new TransferData()
        {
            to = to.text,
            amount = amount.text,
            coin = WalletComponent.Instance.currentCoinMetadata
        });
    }
}
