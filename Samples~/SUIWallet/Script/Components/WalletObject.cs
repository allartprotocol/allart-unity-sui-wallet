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
    public bool overrideImage = false;

    private Balance balance;
    private CoinMetadata coinMetadata;
    private SUIMarketData geckoCoinData;
    private SimpleScreenManager manager;

    private void Start() {
        GetComponent<Button>().onClick.AddListener(OnSelect);
    }

    private void OnSelect()
    {
        if(balance == null)
        {
            return;
        }

        if(balance.totalBalance == 0)
        {
            return;
        }
        manager.ShowScreen("TokenInfoScreen", new Tuple<CoinMetadata, SUIMarketData, Balance>(coinMetadata, geckoCoinData, balance));
    }

    public void Init(Balance balance, SimpleScreenManager manager)
    {
        this.balance = balance;
        this.manager = manager;
        DisplayData(balance);
    }

    private void DisplayData(Balance balance)
    {
        coin_usd.text = "-";
        coin_change.text = "-";
        coin_balance.text = "";
        string symbol = "";
        if(balance == null)
        {
            return;
        }


        coinMetadata = null;
        if(WalletComponent.Instance.coinMetadatas.ContainsKey(balance.coinType))
        {
            coinMetadata = WalletComponent.Instance.coinMetadatas[balance.coinType];           
        }

        coin_balance.text = WalletUtility.ParseDecimalValueToString<long>(balance.totalBalance);

        if (coinMetadata == null)
        {
            return;
        }
        symbol = coinMetadata.symbol;

        if(!overrideImage)
            coin_name.text = coinMetadata.name;

        coin_balance.text = $"{WalletUtility.ParseDecimalValueToString(WalletComponent.ApplyDecimals(balance, coinMetadata))} {coinMetadata.symbol}";

        var tokenImage = GetComponentInChildren<TokenImage>();
        if(!overrideImage)
        {
            Sprite icon = WalletComponent.Instance.GetCoinImage(symbol);
            if(icon != null)
                tokenImage.Init(icon, coinMetadata.name);
        }

        var geckoData = WalletComponent.Instance.GetCoinMarketData(symbol);
        if(geckoData == null)
        {
            return;
        }

        coin_usd.text = "-";
        coin_change.text = $"-";
        decimal minValue = 0.01m;
        if (geckoData != null) { 
            if(geckoData.current_price != null){
                try{
                    decimal.TryParse(geckoData.current_price.ToString(), out decimal price);
                    var usdValue = price * WalletComponent.ApplyDecimals(balance, coinMetadata);
                    if(usdValue < minValue)
                    {
                        coin_usd.text = "<$0.01";
                    }
                    else
                        coin_usd.text = $"${WalletUtility.ParseDecimalValueToString(WalletUtility.ParseDecimalValueFromString(usdValue.ToString("0.00")))}";
                }
                catch(Exception e){
                    Debug.Log(e);
                }
            }

            try{
                if(geckoData.price_change_percentage_24h != null)
                {
                    decimal.TryParse(geckoData.price_change_percentage_24h.ToString(), out decimal priceChange);
                    coin_change.GetComponent<PriceChangeText>().SetText($"{priceChange.ToString("0.00")}%");
                }
            }catch(Exception e){
                Debug.Log(e);
            }
        }
    }

}
