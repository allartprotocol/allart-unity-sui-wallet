using System;
using System.Collections;
using System.Collections.Generic;
using AllArt.SUI.RPC.Response;
using Newtonsoft.Json;
using SimpleScreen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TokenInfoScreen : BaseScreen {

    public TextMeshProUGUI titleText;
    public Button receiveBtn;
    public Button sendBtn;

    private CoinMetadata _coinMetadata;
    private Balance _balance;
    private SUIMarketData geckoData;

    public CoinMetadata coinMetadata
    {
        get { return _coinMetadata; }
        set
        {
            _coinMetadata = value;
            if (_coinMetadata != null)
            {
                coinName.text = _coinMetadata.name;           
            }
        }
    }
    
    public Balance balance
    {
        get { return _balance; }
        set
        {
            _balance = value;
            if (_balance != null)
            {
                coinBalance.text = $"{WalletUtility.ParseDecimalValueToString(WalletComponent.ApplyDecimals(_balance, coinMetadata))} {coinMetadata.symbol}";
            }
        }
    }

    public TMPro.TextMeshProUGUI coinName;
    public TMPro.TextMeshProUGUI coinSymbol;
    public TMPro.TextMeshProUGUI coinPrice;
    public TMPro.TextMeshProUGUI coinPriceChange;
    public TMPro.TextMeshProUGUI coinBalance;
    
    private void Start() {
        receiveBtn.onClick.AddListener(OnReceive);
        sendBtn.onClick.AddListener(OnSend);
    }

    public override void ShowScreen(object data = null)
    {
        base.ShowScreen(data);

        if (data is not Tuple<CoinMetadata, SUIMarketData, Balance> coinData)
        {
            return;
        }


        coinMetadata = coinData.Item1;
        geckoData = coinData.Item2;
        balance = coinData.Item3;

        this.coinMetadata = coinMetadata;
        this.balance = balance;
        this.manager = manager;

        sendBtn.interactable = balance.totalBalance > 0;
        
        Debug.Log(JsonConvert.SerializeObject(coinMetadata));

        coinPrice.text = "0 $";
        coinSymbol.text = coinMetadata.symbol;
        titleText.text = coinMetadata.name;

        if (geckoData != null) { 
            if(geckoData.current_price == null)
            {
                return;
            }
            var usdValue = (decimal)geckoData.current_price_double * WalletComponent.ApplyDecimals(balance, coinMetadata);
            coinPrice.text = $"${usdValue:0.00}";
        }

        var tokenImage = GetComponentInChildren<TokenImage>();
        Sprite icon = WalletComponent.Instance.GetCoinImage(coinMetadata.symbol);
        tokenImage.Init(icon, coinMetadata.symbol);
    }

    private void OnSend()
    {
        WalletComponent.Instance.currentCoinMetadata = coinMetadata;
        GoTo("SendScreen");
    }

    private void OnReceive()
    {
        var wallet = WalletComponent.Instance.currentWallet;
        GoTo("QRScreen", wallet);
    }
}
