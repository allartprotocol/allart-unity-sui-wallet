using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WalletObject : MonoBehaviour
{
    private Balance balance;
    public TextMeshProUGUI coin_name;
    public TextMeshProUGUI coin_balance;
    public TextMeshProUGUI coin_usd;
    public TextMeshProUGUI coin_change;

    public Image coinImage;

    private CoinMetadata coinMetadata;

    public async void Init(Balance balance)
    {
        this.balance = balance;
        await FetchDisplayData(balance);
    }

    private async System.Threading.Tasks.Task FetchDisplayData(Balance balance)
    {
        coinMetadata = WalletComponent.Instance.coinMetadatas[balance.coinType];

        coin_name.text = balance.coinType;
        coin_balance.text = balance.totalBalance.ToString();
        coin_usd.text = "$0";
        coin_change.text = "+0%";

        if (coinMetadata != null)
        {
            coin_name.text = coinMetadata.name;
            coin_balance.text = $"{coinMetadata.symbol} {WalletComponent.ApplyDecimals(balance, coinMetadata)}";
        }

        var geckoData = WalletComponent.Instance.coinData[coinMetadata.symbol];

        if (geckoData != null) { 
            if(geckoData.market_data.current_price == null)
            {
                return;
            }
            var usdValue = geckoData.market_data.current_price["usd"] * WalletComponent.ApplyDecimals(balance, coinMetadata);
            coin_usd.text = $"${usdValue.ToString("0.00")}";
            coin_change.text = $"{geckoData.market_data.price_change_percentage_24h.ToString("0.00")}%";

            var image = await WalletComponent.Instance.GetImage(geckoData.image.thumb);
            if(image != null)
                coinImage.sprite = image;
        }
    }

}
