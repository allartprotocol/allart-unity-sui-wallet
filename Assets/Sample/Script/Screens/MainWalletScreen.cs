using SimpleScreen;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainWalletScreen : BaseScreen
{
    public TextMeshProUGUI walletPubText;
    public TextMeshProUGUI walletBalanceText;
    public TextMeshProUGUI percentageText;

    public Button walletsDropdown;

    public Button receiveBtn;
    public Button sendBtn;
    public Button settingsBtn;

    public Transform objectListContent;
    public GameObject objectListItemPrefab;

    private List<WalletObject> loadedObjects = new List<WalletObject>();

    private void Start()
    {
        receiveBtn.onClick.AddListener(OnReceive);
        sendBtn.onClick.AddListener(OnSend);
        walletsDropdown.onClick.AddListener(OnWalletSelected);
        settingsBtn.onClick.AddListener(OnSettings);
    }

    private void OnSettings()
    {
        manager.ShowScreen("SettingsScreen");
    }

    private void OnWalletSelected()
    {
        manager.ShowScreen("WalletsListScreen");
    }

    private void OnSend()
    {
        manager.ShowScreen("SendScreen");
    }

    private void OnReceive()
    {
        var wallet = WalletComponent.Instance.currentWallet;
        manager.ShowScreen("QRScreen", wallet);
    }

    private async Task LoadWalletData()
    {
        if(WalletComponent.Instance.currentWallet == null)
            WalletComponent.Instance.SetWalletByIndex(0);
        var wallet = WalletComponent.Instance.currentWallet;
        await WalletComponent.Instance.GetDataForAllCoins(wallet.publicKey);

        walletPubText.text = wallet.publicKey;

        walletBalanceText.text = "0";

        var balances = await WalletComponent.Instance.GetAllBalances(wallet);

        foreach (var obj in loadedObjects)
        {
            Destroy(obj.gameObject);
        }
        loadedObjects.Clear();
        foreach (var balance in balances)
        {            
            var obj = Instantiate(objectListItemPrefab, objectListContent);
            WalletObject wo = obj.GetComponent<WalletObject>();
            loadedObjects.Add(wo);
            wo.Init(balance);
        }
    }

    public override async void ShowScreen(object data = null)
    {
        base.ShowScreen(data);

        string password = WalletComponent.Instance.password;
        WalletComponent.Instance.RestoreAllWallets(password);

        var wallet = WalletComponent.Instance.GetAllWallets();

        

        await UpdateWalletData();
    }

    private async Task UpdateWalletData()
    {
        await LoadWalletData();
        UpdateBalance();
    }

    private async void UpdateBalance()
    {
        Balance balance = await WalletComponent.Instance.GetBalance(WalletComponent.Instance.currentWallet, "0x2::sui::SUI");
        if (!WalletComponent.Instance.coinMetadatas.ContainsKey(balance.coinType))
            return;
        CoinMetadata coinMetadata = WalletComponent.Instance.coinMetadatas[balance.coinType];
        if (balance != null)
        {
            var geckoData = WalletComponent.Instance.coinData[coinMetadata.symbol];
            if (geckoData != null)
            {
                var usdValue = geckoData.market_data.current_price["usd"] * WalletComponent.ApplyDecimals(balance, coinMetadata);
                walletBalanceText.text = $"${usdValue.ToString("0.00")}";
                percentageText.text = $"{geckoData.market_data.price_change_percentage_24h.ToString("0.00")}%";
            }
        }

        if(WalletComponent.Instance.currentCoinMetadata == null)
        {
            WalletComponent.Instance.currentCoinMetadata = coinMetadata;
        }
    }  

    public override void HideScreen()
    {
        base.HideScreen();
    }
}
