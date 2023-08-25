using AllArt.SUI.RPC.Response;
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
        foreach (var wallet in wallets)
        {
            if (wallet.Value.publicKey == WalletComponent.Instance.currentWallet.publicKey)
            {
                Debug.Log("Found current wallet: " + wallet.Value.publicKey + " at index: " + options.Count);
                selectedIndex = options.Count;
            }
            options.Add($"{options.Count + 1}. {wallet.Value.displayAddress}");
        }

        walletsDropdown.AddOptions(options);
        walletsDropdown.value = selectedIndex;
    }

    // <summary>
    /// Populates the history of transactions for the current wallet.
    /// </summary>
    async void PopulateHistory()
    {
        foreach (var obj in loadedEvents)
        {
            Destroy(obj.gameObject);
        }

        loadedEvents.Clear();

        LoaderScreen.instance.ShowLoading("Loading history...");
        
        var history = await WalletComponent.Instance.GetTransactionsForSelectedWallet();
        LoaderScreen.instance.HideLoading();

        if(history.Count == 0)
        {
            noActivityText.gameObject.SetActive(true);
            return;
        }
        else
        {
            noActivityText.gameObject.SetActive(false);
        }


        if(history.Count != 0)
        {
            history = history.OrderByDescending(x => x.timestampMs).ToList();
        }

        foreach (var eventPage in history)
        {
            if(eventPage == null)
            {
                continue;
            }
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
        Debug.Log("Selected wallet: " + WalletComponent.Instance.currentWallet.publicKey);
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

        if (WalletComponent.Instance.currentWallet == null)
            return;

        suiSecondaryWalletObject.Init(null, manager);
        suiPrimaryWalletObject.Init(null, manager);
        
        var wallet = WalletComponent.Instance.currentWallet;
        await WalletComponent.Instance.GetDataForAllCoins(wallet.publicKey);

        walletPubText.SetText(wallet.publicKey, wallet.displayAddress);

        walletBalanceText.text = "$0";
        percentageText.text = "";

        var balances = await WalletComponent.Instance.GetAllBalances(wallet);
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
            else if(meta.symbol == "Bonk")
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
    }

    public override async void ShowScreen(object data = null)
    {
        base.ShowScreen(data);

        if(manager.previousScreen != null)
        {
            Debug.Log("Previous screen: " + manager.previousScreen.name);
            if(manager.previousScreen.name == "TransactionInfo")
            {
                OnHistory();
                return;
            }
        }

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
        // loadingScreen.gameObject.SetActive(true);
        LoaderScreen.instance.ShowLoading("Please wait...");
        await LoadWalletData();
        UpdateBalance();
        try
        {
        }
        catch (System.Exception e)
        {
            // loadingScreen.gameObject.SetActive(false);
            LoaderScreen.instance.HideLoading();
            Debug.LogError(e);
        }
        finally
        {
            // loadingScreen.gameObject.SetActive(false);
            LoaderScreen.instance.HideLoading();
        }
    }

    private async void UpdateBalance()
    {
        Balance balance = await WalletComponent.Instance.GetBalance(WalletComponent.Instance.currentWallet, "0x2::sui::SUI");
        percentageText.text = "";
        walletBalanceText.text = "$0";
        if (!WalletComponent.Instance.coinMetadatas.ContainsKey(balance.coinType))
            return;
        CoinMetadata coinMetadata = WalletComponent.Instance.coinMetadatas[balance.coinType];
        if (balance != null && balance.totalBalance > 0)
        {
            if(!WalletComponent.Instance.coinGeckoData.ContainsKey(coinMetadata.symbol))
            {
                return;
            }
            var geckoData = WalletComponent.Instance.coinGeckoData[coinMetadata.symbol];
            if (geckoData != null)
            {
                percentageText.GetComponent<PriceChangeText>().SetText($"{0.00}%");
                if(geckoData.current_price != null){
                    double.TryParse(geckoData.current_price.ToString(), out double price);
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
