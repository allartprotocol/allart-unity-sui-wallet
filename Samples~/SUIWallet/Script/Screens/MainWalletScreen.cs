using AllArt.SUI.RPC.Response;
using AllArt.SUI.Wallets;
using SimpleScreen;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainWalletScreen : BaseScreen
{
    public CopyButton walletPubText;
    public TextMeshProUGUI walletBalanceText;
    public TextMeshProUGUI percentageText;

    public TMP_Dropdown walletsDropdown;
    public TextMeshProUGUI walletNameDrop;

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
    public Transform noBalanceText;
    public Transform noActivityText;

    public WalletObject suiPrimaryWalletObject;
    public WalletObject suiSecondaryWalletObject;

    public enum EDisplay
    {
        WALLET,
        HISTORY
    }

    private EDisplay display = EDisplay.WALLET;

    private void Start()
    {
        receiveBtn.onClick.AddListener(OnReceive);
        sendBtn.onClick.AddListener(OnSend);
        walletsDropdown.onValueChanged.AddListener(OnWalletSelected);
        settingsBtn.onClick.AddListener(OnSettings);
        assets.onClick.AddListener(OnAssets);
        history.onClick.AddListener(OnHistory);
        WebsocketController.instance.onWSEvent += OnNewTransaction;
        loadingScreen = GameObject.FindObjectOfType<LoaderScreen>(true).transform;
        loadingScreen.gameObject.SetActive(true);
    }

    private async void OnNewTransaction()
    {
        await UpdateWalletData();
    }

    private void OnHistory()
    {
        display = EDisplay.HISTORY;
        assetHolder.gameObject.SetActive(false);
        historyHolder.gameObject.SetActive(true);
        PopulateHistory();
    }

    void PopulateWalletsDropdown()
    {
        int selectedIndex = 0;
        walletsDropdown.ClearOptions();
        var wallets = WalletComponent.Instance.GetAllWallets();
        List<string> options = new();

        int index = 1;

        foreach (var wallet in wallets)
        {
            if (wallet.Value.publicKey == WalletComponent.Instance.currentWallet.publicKey)
            {
                selectedIndex = options.Count;
            }
            options.Add("<color=#BABABA>" + $"{options.Count + 1}.</color> {wallet.Value.displayAddress}");
            index++;
        }

        walletsDropdown.AddOptions(options);
        walletsDropdown.value = selectedIndex;
    }

    // <summary>
    /// Populates the history of transactions for the current wallet.
    /// </summary>
    async void PopulateHistory()
    {
        ClearActivity();
        Debug.Log("Populating history");

        LoaderScreen.instance.ShowLoading("Loading activity...");

        var history = await WalletComponent.Instance.GetTransactionsForSelectedWallet();
        LoaderScreen.instance.HideLoading();

        if (history.Count == 0)
        {
            noActivityText.gameObject.SetActive(true);
            return;
        }
        else
        {
            noActivityText.gameObject.SetActive(false);
        }


        if (history.Count > 0)
        {
            history = history.OrderByDescending(x => x.timestampMs).ToList();
        }

        foreach (var eventPage in history)
        {
            if (eventPage == null)
            {
                continue;
            }
            var obj = Instantiate(historyObjectPrefab, historyContent);
            var eventObject = obj.GetComponent<EventObject>();
            try
            {
                eventObject.InitializeObject(eventPage, manager);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Destroy(obj);
                continue;
            }
            loadedEvents.Add(eventObject);
        }
    }

    private void ClearActivity()
    {
        foreach (var obj in loadedEvents)
        {
            Destroy(obj.gameObject);
        }

        loadedEvents.Clear();
    }

    private void OnAssets()
    {
        display = EDisplay.WALLET;
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
        SetWalletName(WalletComponent.Instance.currentWallet);

        await UpdateWalletData();
        if(display == EDisplay.HISTORY){
            PopulateHistory();
        }
    }

    private void SetWalletName(Wallet wallet){
        int index = WalletComponent.Instance.GetWalletIndex(wallet);
        walletPubText.SetText(wallet.publicKey, wallet.displayAddress);
        walletNameDrop.text = $"Wallet {index + 1}";
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

    private bool IsTotalBalanceAboveZero(List<Balance> balances)
    {
        foreach (var balance in balances)
        {
            if (balance.totalBalance > 0)
            {
                return true;
            }
        }

        return false;
    }

    private async Task LoadWalletData()
    {
        walletBalanceText.text = "$0";
        percentageText.text = "";
        walletNameDrop.text = "Wallet";
        if (WalletComponent.Instance.currentWallet == null)
            WalletComponent.Instance.SetWalletByIndex(0);

        if (WalletComponent.Instance.currentWallet == null)
            return;

        SetWalletName(WalletComponent.Instance.currentWallet);

        suiSecondaryWalletObject.Init(null, manager);
        suiPrimaryWalletObject.Init(null, manager);
        WalletComponent.Instance.GetSUIMarketDataForCoins();
        
        var wallet = WalletComponent.Instance.currentWallet;
        await WalletComponent.Instance.GetDataForAllCoins(wallet.publicKey);

        walletPubText.SetText(wallet.publicKey, wallet.displayAddress);

        walletBalanceText.text = "$0";
        percentageText.text = "";

        var balances = await WalletComponent.Instance.GetAllBalances(wallet);
        WalletComponent.Instance.SetWalletBalance(balances);
        if(balances == null)
        {
            return;
        }
        sendBtn.interactable = balances.Count > 0;
        foreach (var obj in loadedObjects)
        {
            Destroy(obj.gameObject);
        }
        loadedObjects.Clear();

        if(balances.Count == 0)
        {
            noBalanceText.gameObject.SetActive(true);
            return;
        }
        
        noBalanceText.gameObject.SetActive(false);
        
        bool suiPrimary = false;
        bool suiSecondary = false;

        foreach (Balance balance in balances)
        {
            if(!WalletComponent.Instance.coinMetadatas.ContainsKey(balance.coinType))
            {
                continue;
            }
            var meta = WalletComponent.Instance.coinMetadatas[balance.coinType];
            if(meta.symbol == "SUI")
            {
                suiPrimaryWalletObject.Init(balance, manager);
                suiPrimary = true;
                continue;
            }
            else if(meta.symbol == "AART")
            {
                suiSecondaryWalletObject.Init(balance, manager);
                suiSecondary = true;
                continue;
            }

            if(balance.totalBalance == 0)
            {
                continue;
            }
            var obj = Instantiate(objectListItemPrefab, objectListContent);
            WalletObject wo = obj.GetComponent<WalletObject>();
            wo.Init(balance, manager);
            loadedObjects.Add(wo);
        }

        if(!suiPrimary)
        {
            Debug.Log("SUI Primary not found");
            suiPrimaryWalletObject.Init(null, manager);
        }

        if(!suiSecondary)
        {
            suiSecondaryWalletObject.Init(null, manager);
        }

        sendBtn.interactable = IsTotalBalanceAboveZero(balances);
    }

    public override async void ShowScreen(object data = null)
    {
        base.ShowScreen(data);

        string password = WalletComponent.Instance.password;
        WalletComponent.Instance.RestoreAllWallets(password);

        Wallet wallet = WalletComponent.Instance.currentWallet;

        if (wallet == null)
        {
            wallet = WalletComponent.Instance.GetWalletByIndex(0);
            WalletComponent.Instance.SetCurrentWallet(wallet);
        }

        SetWalletName(wallet);
        PopulateWalletsDropdown();

        await UpdateWalletData();
        if(display == EDisplay.HISTORY){
            PopulateHistory();
        }

        manager.ClearHistory(this);
    }

    private async Task UpdateWalletData()
    {
        LoaderScreen.instance.ShowLoading("Please wait...");
        try
        {
            await LoadWalletData();
            UpdateBalance();
        }
        catch (System.Exception e)
        {
            LoaderScreen.instance.HideLoading();
            Debug.LogError(e);
        }
        finally
        {
            LoaderScreen.instance.HideLoading();
        }
    }

    private async void UpdateBalance()
    {
        percentageText.text = "";
        walletBalanceText.text = "$0";
        Balance balance = await WalletComponent.Instance.GetBalance(WalletComponent.Instance.currentWallet, "0x2::sui::SUI");
        if (!WalletComponent.Instance.coinMetadatas.ContainsKey(balance.coinType))
            return;

        CoinMetadata coinMetadata = WalletComponent.Instance.coinMetadatas[balance.coinType];

        if (WalletComponent.Instance.currentCoinMetadata == null)
        {
            WalletComponent.Instance.currentCoinMetadata = coinMetadata;
        }

        if (balance != null && balance.totalBalance > 0)
        {
            var geckoData = WalletComponent.Instance.GetCoinMarketData(coinMetadata.symbol);
            if(geckoData == null)
            {
                return;
            }

            percentageText.GetComponent<PriceChangeText>().SetText($"{0.00}%");
            if(geckoData.current_price != null){
                decimal.TryParse(geckoData.current_price.ToString(), out decimal price);
                var usdValue = price * WalletComponent.ApplyDecimals(balance, coinMetadata);
                walletBalanceText.text = $"${usdValue.ToString("0.00")}";
            }
            else{
                walletBalanceText.text = "$0";
            }
            try{
                if(geckoData.price_change_percentage_24h != null)
                {
                    float.TryParse(geckoData.price_change_percentage_24h.ToString(), out float priceChange);
                    percentageText.GetComponent<PriceChangeText>().SetText($"{priceChange.ToString("0.00")}%");
                }
            }catch(Exception e){
                Debug.Log(e);
            }
        }
    }

    public override void HideScreen()
    {
        base.HideScreen();
    }
}
