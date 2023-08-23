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
        coin_usd.text = "$0";
        coin_change.text = "+0%";
        coin_balance.text = "";
        if(balance == null)
        {
            return;
        }

        coinMetadata = null;
        if(WalletComponent.Instance.coinMetadatas.ContainsKey(balance.coinType))
        {
            coinMetadata = WalletComponent.Instance.coinMetadatas[balance.coinType];           
        }

        coin_balance.text = balance.totalBalance.ToString();

        if (coinMetadata == null)
        {
            return;
        }

        if(!overrideImage)
            coin_name.text = coinMetadata.name;
        coin_balance.text = $"{WalletComponent.ApplyDecimals(balance, coinMetadata)} {coinMetadata.symbol}";

        var tokenImage = GetComponentInChildren<TokenImage>();
        if(!overrideImage)
        {
            if(WalletComponent.Instance.coinImages != null)
            {
                if(WalletComponent.Instance.coinImages.TryGetValue(coinMetadata.symbol, out Sprite image)){
                    tokenImage.Init(image, coinMetadata.name);
                }
            }            
        }

        if(WalletComponent.Instance.coinGeckoData == null){
            return;
        }
        
        if(!WalletComponent.Instance.coinGeckoData.ContainsKey(coinMetadata.symbol))
        {
            return;
        }
        var geckoData = WalletComponent.Instance.coinGeckoData[coinMetadata.symbol];
        geckoCoinData = geckoData;

        coin_usd.text = "$0";
        coin_change.text = $"{0.00}%";
        if (geckoData != null) { 
            if(geckoData.current_price != null){
                try{
                    float.TryParse(geckoData.current_price.ToString(), out float price);
                    var usdValue = price * WalletComponent.ApplyDecimals(balance, coinMetadata);
                    coin_usd.text = $"${usdValue:0.00}";
                }
                catch(Exception e){
                    Debug.Log(e);
                }
            }

            try{
                if(geckoData.price_change_percentage_24h != null)
                {
                    float.TryParse(geckoData.price_change_percentage_24h.ToString(), out float priceChange);
                    coin_change.text = $"{priceChange.ToString("0.00")}%";
                }
            }catch(Exception e){
                Debug.Log(e);
            }
        }
    }

}
