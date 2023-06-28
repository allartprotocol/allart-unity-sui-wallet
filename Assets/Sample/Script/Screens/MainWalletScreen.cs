using SimpleScreen;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainWalletScreen : BaseScreen
{
    public TextMeshProUGUI walletPubText;
    public TextMeshProUGUI walletBalanceText;
    public TextMeshProUGUI percentageText;

    public TMP_Dropdown walletsDropdown;

    public Button receiveBtn;
    public Button sendBtn;

    public Transform objectListContent;
    public GameObject objectListItemPrefab;

    private void Start()
    {
        receiveBtn.onClick.AddListener(OnReceive);
        sendBtn.onClick.AddListener(OnSend);
        walletsDropdown.onValueChanged.AddListener(OnWalletSelected);
    }

    private void OnWalletSelected(int value)
    {

    }

    private void OnSend()
    {
    }

    private void OnReceive()
    {
        var wallet = WalletComponent.Instance.GetWalletByIndex(0);
        manager.ShowScreen("QRScreen", wallet);
    }

    private async void LoadWalletData()
    {
        var wallet = WalletComponent.Instance.GetWalletByIndex(0);

        walletPubText.text = wallet.publicKey;

        walletBalanceText.text = "0";

        var balances = await WalletComponent.Instance.GetAllBalances(wallet);
        
        foreach (var balance in balances)
        {
            var obj = Instantiate(objectListItemPrefab, objectListContent);
            obj.GetComponent<WalletObject>().Init(balance);
        }
    }

    public override void ShowScreen(object data = null)
    {
        base.ShowScreen(data);

        string password = WalletComponent.Instance.password;
        WalletComponent.Instance.RestoreAllWallets(password);

        var wallet = WalletComponent.Instance.GetAllWallets();

        if (wallet.Count > 0)
        {
            PopulateDropdownWithWallets(wallet, walletsDropdown);
            walletsDropdown.value = 0;
        }
        else
        {
            Debug.Log("No wallet found");
        }

        LoadWalletData();
        UpdateBalance();
    }

    private async void UpdateBalance()
    {
        Balance balance = await WalletComponent.Instance.GetBalance(WalletComponent.Instance.GetWalletByIndex(0), "0x2::sui::SUI");
        CoinMetadata coinMetadata = await WalletComponent.Instance.GetCoinMetadata(balance.coinType);
        if (balance != null)
        {
            var geckoData = await WalletComponent.Instance.GetUSDPrice("sui");
            if(geckoData != null)
            {
                var usdValue = geckoData.market_data.current_price["usd"] * WalletComponent.ApplyDecimals(balance, coinMetadata);
                walletBalanceText.text = $"${usdValue.ToString("0.00")}";
                percentageText.text = $"{geckoData.market_data.price_change_percentage_24h.ToString("0.00")}%";
            }
        }
    }

    void PopulateDropdownWithWallets(Dictionary<string, Wallet> wallets, TMP_Dropdown walletsDropdown)
    {
        foreach (var wallet in wallets)
        {
            walletsDropdown.options.Add(new TMP_Dropdown.OptionData(wallet.Key));
        }
    }

    public override void HideScreen()
    {
        base.HideScreen();
    }
}
