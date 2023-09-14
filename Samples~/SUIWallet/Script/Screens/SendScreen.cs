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

    private QRReader qrReader;
    public Button max;

    private Balance balance;
    private CoinMetadata coinMetadata;

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

        // limit to 8 decimal places
        if(arg0.Contains(".") && arg0.Split('.')[1].Length > coinMetadata.decimals)
        {
            amount.text = arg0.Split('.')[0] + "." + arg0.Split('.')[1].Substring(0, coinMetadata.decimals);
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, $"The value exceeds the maximum decimals ({coinMetadata.decimals})");
        }
        
        decimal.TryParse(arg0, out decimal parseAmmount);

        if(parseAmmount > GetMaxBalance())
        {
            amount.text = GetMaxBalance().ToString();
        }

        bool maxBalance = arg0.Equals(GetMaxBalance().ToString());
    }

    private void OnQRCodeFound(string obj)
    {
        to.text = obj;
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
        coinMetadata = WalletComponent.Instance.currentCoinMetadata;
        tokenName.text = $"Send {WalletComponent.Instance.currentCoinMetadata.symbol}";
        var coinType = WalletComponent.Instance.coinMetadatas.Where(x => x.Value.symbol == WalletComponent.Instance.currentCoinMetadata.symbol).FirstOrDefault().Key;
        balance = await WalletComponent.Instance.GetBalance(WalletComponent.Instance.currentWallet.publicKey, 
            coinType);

        maxAmmount.text = $"Available {GetMaxBalance()} Tokens";

        var tokenImgComponent = GetComponentInChildren<TokenImage>();
        Sprite icon = WalletComponent.Instance.GetCoinImage( WalletComponent.Instance.currentCoinMetadata.symbol);
        tokenImgComponent.Init(icon, WalletComponent.Instance.currentCoinMetadata.symbol);
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
        bool maxBalance = amount.text.Equals(balance.ToString());

        GoTo("SendConfirmScreen", new TransferData()
        {
            to = to.text,
            amount = amount.text,
            coin = WalletComponent.Instance.currentCoinMetadata,
            transferAll = maxBalance
        });
    }
}
