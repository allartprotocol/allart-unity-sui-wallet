using AllArt.SUI.RPC.Response;
using SimpleScreen;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TokenSelect : MonoBehaviour
{
    private CoinMetadata _coinMetadata;
    private GeckoCoinData _coinData;
    private Balance _balance;

    private SimpleScreenManager manager;
    public TokenImage tokenImage;

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
    
    public GeckoCoinData coinData
    {
        get { return _coinData; }
        set
        {
            _coinData = value;
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
                coinBalance.text = (_balance.totalBalance / Mathf.Pow(10, coinMetadata.decimals)).ToString();
            }
        }
    }

    public TMPro.TextMeshProUGUI coinName;
    public TMPro.TextMeshProUGUI coinSymbol;
    public TMPro.TextMeshProUGUI coinPrice;
    public TMPro.TextMeshProUGUI coinPriceChange;
    public TMPro.TextMeshProUGUI coinBalance;
    public UnityEngine.UI.Image coinImage;

    public Button go;

    public void InitComponent(CoinMetadata coinMetadata, GeckoCoinData coinData, Balance balance, SimpleScreenManager manager)
    {
        go = GetComponent<Button>();
        this.coinMetadata = coinMetadata;
        this.coinData = coinData;
        this.balance = balance;
        this.manager = manager;

        tokenImage = GetComponentInChildren<TokenImage>();
        WalletComponent.Instance.coinImages.TryGetValue(coinMetadata.symbol, out Sprite image);
        tokenImage.Init(image, coinMetadata.name);
        go.onClick.AddListener(OnGo);
    }

    private void OnGo()
    {
        WalletComponent.Instance.currentCoinMetadata = coinMetadata;     
        manager.GoBack();
    }
}
