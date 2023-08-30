using AllArt.SUI.RPC.Response;
using AllArt.SUI.Wallets;
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
    public TextMeshProUGUI maxAmmount;

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
        amount.onValueChanged.AddListener(OnAmountChanged);

        
        qrReader = FindObjectOfType<QRReader>();
        qrReader.OnQRRead += OnQRCodeFound;
    }

    private void OnAmountChanged(string arg0)
    {
        if(string.IsNullOrEmpty(arg0))
        {
            return;
        }
        decimal.TryParse(arg0, out decimal parseAmmount); 
        

        if(parseAmmount > GetMaxBalance())
        {
            amount.text = GetMaxBalance().ToString();
        }
        bool maxBalance = arg0.Equals(GetMaxBalance().ToString());
        Debug.Log($"Max Balance: {maxBalance}");
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
        decimal balance = GetMaxBalance();
        amount.text = balance.ToString();
    }

    private decimal GetMaxBalance(){
        var meta = WalletComponent.Instance.coinMetadatas.Where(x => x.Value.symbol == WalletComponent.Instance.currentCoinMetadata.symbol).FirstOrDefault();
        if(meta.Value == null)
        {
            return 0;
        }
        var coinType = meta.Value;
        return balance.totalBalance / (decimal)Mathf.Pow(10, coinType.decimals);

    }

    private void OnTokenSelect()
    {
        GoTo("TokenSelectScreen");
    }

    public override async void ShowScreen(object data = null)
    {
        base.ShowScreen(data);
        to.text = "";
        amount.text = "";
        maxAmmount.text = "";
        tokenName.text = $"Send {WalletComponent.Instance.currentCoinMetadata.symbol}";
        var coinType = WalletComponent.Instance.coinMetadatas.Where(x => x.Value.symbol == WalletComponent.Instance.currentCoinMetadata.symbol).FirstOrDefault().Key;
        balance = await WalletComponent.Instance.GetBalance(WalletComponent.Instance.currentWallet.publicKey, 
            coinType);

        maxAmmount.text = $"Available {GetMaxBalance()} Tokens";

        var tokenImgComponent = GetComponentInChildren<TokenImage>();
        WalletComponent.Instance.coinImages.TryGetValue(WalletComponent.Instance.currentCoinMetadata.symbol, out Sprite image);
        tokenImgComponent.Init(image, WalletComponent.Instance.currentCoinMetadata.symbol);
    }

    override public void HideScreen()
    {
        base.HideScreen();
        OnClose();
    }

    private void OnContinue()
    {
        if(string.IsNullOrEmpty(to.text) || string.IsNullOrEmpty(amount.text))
        {
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Please fill in all fields");
            return;
        }

        if(float.Parse(amount.text) <= 0)
        {
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Amount must be greater than 0");
            return;
        }

        if(KeyPair.IsSuiAddressInCorrectFormat(to.text) == false)
        {
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Invalid address");
            return;
        }
        decimal balance = GetMaxBalance();
        Debug.Log($"Balance: {balance}");
        Debug.Log($"Amount: {amount.text}");
        bool maxBalance = amount.text.Equals(balance.ToString());
        Debug.Log($"Max Balance: {maxBalance}");
        Debug.Log($"Transfer All: {maxBalance}");
        GoTo("SendConfirmScreen", new TransferData()
        {
            to = to.text,
            amount = amount.text,
            coin = WalletComponent.Instance.currentCoinMetadata,
            transferAll = maxBalance
        });
    }
}