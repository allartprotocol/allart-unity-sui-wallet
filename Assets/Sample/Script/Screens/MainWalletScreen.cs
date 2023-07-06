using SimpleScreen;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
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
    public Button settingsBtn;

    public Button assets;
    public Button history;

    public Transform objectListContent;
    public GameObject objectListItemPrefab;

    private List<WalletObject> loadedObjects = new();

    public Transform assetHolder;
    public Transform historyHolder;

    public GameObject historyObjectPrefab;
    public Transform historyContent;

    public List<EventObject> loadedEvents = new();

    public Transform loadingScreen;

    private void Start()
    {
        receiveBtn.onClick.AddListener(OnReceive);
        sendBtn.onClick.AddListener(OnSend);
        walletsDropdown.onValueChanged.AddListener(OnWalletSelected);
        settingsBtn.onClick.AddListener(OnSettings);
        assets.onClick.AddListener(OnAssets);
        history.onClick.AddListener(OnHistory);
        WebsocketController.instance.onWSEvent += OnNewTransaction;
    }

    private async void OnNewTransaction()
    {
        await UpdateWalletData();
    }

    private void OnHistory()
    {
        assetHolder.gameObject.SetActive(false);
        historyHolder.gameObject.SetActive(true);
        PopulateHistory();
    }

    void PopulateWalletsDropdown()
    {

        walletsDropdown.ClearOptions();

        var wallets = WalletComponent.Instance.GetAllWallets();

        List<string> options = new();

        foreach (var wallet in wallets)
        {
            options.Add(wallet.Value.publicKey);
        }

        walletsDropdown.AddOptions(options);
    }

    // <summary>
    /// Populates the history of transactions for the current wallet.
    /// </summary>
    async void PopulateHistory()
    {
        var history = await WalletComponent.Instance.GetTransactionsForSelectedWallet();

        foreach (var obj in loadedEvents)
        {
            Destroy(obj.gameObject);
        }

        loadedEvents.Clear();

        foreach (var eventPage in history)
        {
            var obj = Instantiate(historyObjectPrefab, historyContent);
            var eventObject = obj.GetComponent<EventObject>();
            eventObject.InitializeObject(eventPage, manager);
            loadedEvents.Add(eventObject);
        }
    }

    private void OnAssets()
    {
        assetHolder.gameObject.SetActive(true);
        historyHolder.gameObject.SetActive(false);
    }

    private void OnSettings()
    {
        GoTo("WalletsListScreen");
    }

    private async void OnWalletSelected(int value)
    {
        WalletComponent.Instance.SetWalletByIndex(value);
        await UpdateWalletData();
    }

    private void OnSend()
    {
        GoTo("SendScreen");
    }

    private void OnReceive()
    {
        var wallet = WalletComponent.Instance.currentWallet;
        GoTo("QRScreen", wallet);
    }

    private async Task LoadWalletData()
    {
        if (WalletComponent.Instance.currentWallet == null)
            WalletComponent.Instance.SetWalletByIndex(0);

        var wallet = WalletComponent.Instance.currentWallet;
        await WalletComponent.Instance.GetDataForAllCoins(wallet.publicKey);

        walletPubText.text = wallet.publicKey;

        walletBalanceText.text = "0";
        percentageText.text = "0%";

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
            wo.Init(balance, manager);
        }

        sendBtn.interactable = balances.Count > 0;
    }

    public override async void ShowScreen(object data = null)
    {
        base.ShowScreen(data);

        string password = WalletComponent.Instance.password;
        WalletComponent.Instance.RestoreAllWallets(password);

        var wallet = WalletComponent.Instance.GetAllWallets();

        OnAssets();

        await UpdateWalletData();
        PopulateWalletsDropdown();

        manager.ClearHistory(this);
    }

    private async Task UpdateWalletData()
    {
        try
        {
            loadingScreen.gameObject.SetActive(true);
            await LoadWalletData();
            UpdateBalance();
        }
        catch (System.Exception e)
        {
            loadingScreen.gameObject.SetActive(false);
            Debug.LogError(e);
        }
        finally
        {
            loadingScreen.gameObject.SetActive(false);
        }
    }

    private async void UpdateBalance()
    {
        Balance balance = await WalletComponent.Instance.GetBalance(WalletComponent.Instance.currentWallet, "0x2::sui::SUI");
        percentageText.text = "0%";
        walletBalanceText.text = "0";
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

        if (WalletComponent.Instance.currentCoinMetadata == null)
        {
            WalletComponent.Instance.currentCoinMetadata = coinMetadata;
        }
    }

    public override void HideScreen()
    {
        base.HideScreen();
    }
}
