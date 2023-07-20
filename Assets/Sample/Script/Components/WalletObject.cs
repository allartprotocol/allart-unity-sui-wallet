using System;
using System.Threading.Tasks;
using AllArt.SUI.RPC.Response;
using SimpleScreen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WalletObject : MonoBehaviour
{
    public TextMeshProUGUI coin_name;
    public TextMeshProUGUI coin_balance;
    public TextMeshProUGUI coin_usd;
    public TextMeshProUGUI coin_change;

    public Image coinImage;

    private Balance balance;
    private CoinMetadata coinMetadata;
    private GeckoCoinData geckoCoinData;
    private SimpleScreenManager manager;

    private void Start() {
        GetComponent<Button>().onClick.AddListener(OnSelect);
    }

    private void OnSelect()
    {
        manager.ShowScreen("TokenInfoScreen", new Tuple<CoinMetadata, GeckoCoinData, Balance>(coinMetadata, geckoCoinData, balance));
    }

    public void Init(Balance balance, SimpleScreenManager manager)
    {
        this.balance = balance;
        this.manager = manager;
        DisplayData(balance);
    }

    private void DisplayData(Balance balance)
    {
        coinMetadata = null;
        if(WalletComponent.Instance.coinMetadatas.ContainsKey(balance.coinType))
        {
            coinMetadata = WalletComponent.Instance.coinMetadatas[balance.coinType];           
        }

        coin_name.text = "";
        coin_balance.text = balance.totalBalance.ToString();
        coin_usd.text = "$0";
        coin_change.text = "+0%";

        if (coinMetadata != null)
        {
            coin_name.text = coinMetadata.name;
            coin_balance.text = $"{coinMetadata.symbol} {WalletComponent.ApplyDecimals(balance, coinMetadata)}";
        }

        var tokenImage = GetComponentInChildren<TokenImage>();
        WalletComponent.Instance.coinImages.TryGetValue(coinMetadata.symbol, out Sprite image);
        tokenImage.Init(image, coinMetadata.name);

        var geckoData = WalletComponent.Instance.coinGeckoData[coinMetadata.symbol];
        geckoCoinData = geckoData;
        if (geckoData != null && geckoData.market_data != null) { 
            if(geckoData.market_data.current_price == null)
            {
                return;
            }
            var usdValue = geckoData.market_data.current_price["usd"] * WalletComponent.ApplyDecimals(balance, coinMetadata);
            coin_usd.text = $"${usdValue:0.00}";
            coin_change.text = $"{geckoData.market_data.price_change_percentage_24h.ToString("0.00")}%";


        }
    }

}
