using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllArt.SUI.RPC;
using AllArt.SUI.RPC.Filter.Types;
using AllArt.SUI.RPC.Response;
using AllArt.SUI.Wallets;
using Newtonsoft.Json;
using SimpleScreen;
using UnityEngine;

public enum ENodeType
{
    MainNet,
    TestNet,
    DevNet
}

public class WalletComponent : MonoBehaviour
{
    public static WalletComponent Instance { get; private set; }

    private Dictionary<string, Wallet> wallets = new();
    public SUIRPCClient client { get; private set; }
    private WebsocketController websocketController {get; set;}

    // cached coin data
    private List<SUIMarketData> suiMarketData = new();
    public Dictionary<string, CoinMetadata> coinMetadatas {get; private set;} = new();
    // public Dictionary<string, SUIMarketData> coinGeckoData {get; private set;} = new();
    public Dictionary<string, CoinPage> coinPages {get; private set;} = new();
    private Dictionary<string, Sprite> coinImages {get; set;} = new();

    private List<Balance> _currentWalletBalance = new();

    public List<Balance> currentWalletBalance
    {
        get
        {
            return _currentWalletBalance;
        }
        private set
        {
            _currentWalletBalance = value;
        }
    }

    public DateTime lastUpdated {get; private set;} = DateTime.Now;

    // currently selected wallet
    public Wallet currentWallet {get; private set;}
    public CoinMetadata currentCoinMetadata;

    private SimpleScreenManager screenManager;

    public Sprite suiIcon;
    public Sprite bonkIcon;

    public string nodeAddress
    {
        get
        {
            return nodeType switch
            {
                ENodeType.MainNet => SUIConstantVars.mainNetNode,
                ENodeType.TestNet => SUIConstantVars.testNetNode,
                ENodeType.DevNet => SUIConstantVars.devNetNode,
                _ => SUIConstantVars.mainNetNode,
            };
        }
    }

    private string _password;
    public string password
    {
        get { return _password; }
        private set
        {
            if(value != null && value != string.Empty)
            {
                _password = value;
                StartTimer();
            }
        }
    }

    public ENodeType nodeType { get; private set; }

    bool isLocked = true;

    DateTime timeoutTimer;

    #region Mono

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            return;
        }
        Destroy(this.gameObject);

        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("nodeType"))
        {
            nodeType = (ENodeType)PlayerPrefs.GetInt("nodeType");
        }
        else
        {
            nodeType = ENodeType.MainNet;
        }

        client = new SUIRPCClient(nodeAddress);
        websocketController = WebsocketController.instance;
        websocketController.SetupConnection(nodeAddress);
        screenManager = FindObjectOfType<SimpleScreenManager>();
        GetSUIMarketDataForCoins();

    }

    public async void GetSUIMarketDataForCoins()
    {
        if(suiMarketData == null){
            suiMarketData = new();
        }
        if (suiMarketData.Count == 0 || lastUpdated.AddMinutes(1) <= DateTime.Now)
        {
            lastUpdated = DateTime.Now;
            suiMarketData = await GetCoingeckoCoinValues();
            Debug.Log("Got SUI market data");
        }
    }

    private void OnDisable()
    {
        password = null;
        timeoutTimer = DateTime.Now;
    }

    #endregion

    #region UI
        public void ShowWalletInterface()
        {
            screenManager?.Show();
        }

        public void HideWalletInterface()
        {
            screenManager?.Hide();
        }
    #endregion

    #region Timer 

    /// <summary>
    /// Starts a timer coroutine that will disconnect the websocket connection.
    /// </summary>
    public void StartTimer()
    {
        float time = 10000000;
        if (PlayerPrefs.HasKey("timeout"))
        {
            time = PlayerPrefs.GetFloat("timeout");
        }

        timeoutTimer = time == -1 ? DateTime.MaxValue : DateTime.Now.AddSeconds(time);
        
        isLocked = false;   
    }
    

    private void Update() {
        if(timeoutTimer < DateTime.Now && isLocked == false)
        {
            websocketController?.Stop();
            password = null;
            screenManager?.ShowScreen("SplashScreen");
            currentWallet = null;
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Warning, "Connection timed out. Please try again.");
            isLocked = true;
        }
    }

    #endregion

    /// <summary>
    /// Sets the node address for the wallet component and updates the RPC client and websocket controller accordingly.
    /// </summary>
    /// <param name="nodeAddress">The type of node address to set.</param>
    public void SetNodeAddress(ENodeType nodeAddress)
    {
        if (nodeAddress == nodeType)
        {
            return;
        }
        PlayerPrefs.SetInt("nodeType", (int)nodeAddress);

        this.nodeType = nodeAddress;
        client = new SUIRPCClient(this.nodeAddress);
        websocketController.Stop();
        websocketController.SetupConnection(this.nodeAddress);
    }

    /// <summary>
    /// Sets the password for the wallet component and starts the timer.
    /// </summary>
    /// <param name="password">The password to set.</param>
    public void SetPassword(string password)
    {
        this.password = password;
    }

    /// <summary>
    /// Checks if the given password is valid by attempting to restore all wallets with the given password.
    /// </summary>
    /// <param name="passwordString">The password to check for validity.</param>
    /// <returns>True if the password is valid and can be used to restore wallets, false otherwise.</returns>
    public bool CheckPasswordValidity(string passwordString)
    {
        if (passwordString.Length < 1 || passwordString.Contains(" ") || string.IsNullOrEmpty(passwordString))
        {
            return false;
        }

        List<Wallet> wallets = RestoreWallets(passwordString);
        
        if (wallets.Count > 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Sets the current wallet to the wallet at the specified index in the list of wallets and subscribes to its transactions.
    /// </summary>
    /// <param name="value">The index of the wallet to set as the current wallet.</param>
    internal void SetWalletByIndex(int value)
    {
        SetCurrentWallet(GetWalletByIndex(value));
    }

    public int GetWalletIndex(Wallet wallet)
    {
        var walletNames = Wallet.GetWalletSavedKeys();
        for (int i = 0; i < walletNames.Count; i++)
        {
            if (walletNames[i] == wallet.walletName)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Sets the current wallet to the specified wallet and subscribes to its transactions using the websocket controller.
    /// </summary>
    /// <param name="wallet">The wallet to set as the current wallet.</param>
    public async void SetCurrentWallet(Wallet wallet)
    {
        if(wallet == null)
            return;
        currentWallet = wallet;
        var fromOrToFilter = new FromOrToAddressFilter(new FromOrObject(wallet.publicKey));
        var fromAndToFilter = new FromAndToAddressFilter(new FromToObject(wallet.publicKey, wallet.publicKey));
        var filterOr = new FilterOr(new List<object>() { new FromAddresObject(new FromAddressFilter(wallet.publicKey)), new ToAddresObject( new ToAddressFilter(wallet.publicKey)) });
        var toFilter = new ToAddressFilter(wallet.publicKey);
        var fromFilter = new FromAddressFilter(wallet.publicKey);
        websocketController.UnsubscribeCurrent();
        await websocketController.Subscribe(toFilter);
    }

    public void SetWalletBalance(List<Balance> balances)
    {
        if(balances == null)
        {
            currentWalletBalance = new List<Balance>();
            return;
        }
        currentWalletBalance = balances;
    }

    public Balance GetCurrentWalletBalanceForType(string coinType){
        foreach (var balance in currentWalletBalance)
        {
            if(balance.coinType == coinType)
            {
                return balance;
            }
        }
        return null;
    }

    #region Wallet Management

    /// <summary>
    /// Represents a cryptocurrency wallet that can store and manage multiple accounts.
    /// </summary>
    public Wallet CreateWalletWithNewMnemonic(string password = "password", string walletName = "")
    {
        Wallet wallet = new(password, walletName);
        wallet.SaveWallet();
        return wallet;
    }

    /// <summary>
    /// Creates a new wallet with the specified mnemonic, password and wallet name, and saves it to the device.
    /// </summary>
    /// <param name="mnemonic">The mnemonic to use for creating the wallet.</param>
    /// <param name="password">The password to use for encrypting the wallet.</param>
    /// <param name="walletName">The name to give to the wallet.</param>
    /// <returns>The newly created wallet.</returns>
    public Wallet CreateWallet(string mnemonic, string password = "password", string walletName = "")
    {
        Wallet wallet = new(mnemonic, password, walletName);
        return wallet;
    }

    public Wallet CreateWalletFromPrivateKey(string privateKey, string password = "password")
    {
        KeyPair walletKeyPair;
        try{
            walletKeyPair = KeyPair.GenerateKeyPairFromPrivateKey(privateKey);
        }catch(Exception e){
            Debug.LogError(e.Message);
            return null;
        }
        Wallet wallet = new(walletKeyPair, password);        

        return wallet;
    }

    /// <summary>
    /// Restores all wallets saved on the device with the given password and adds them to the list of wallets.
    /// </summary>
    /// <param name="password">The password to use for restoring the wallets.</param>
    public void RestoreAllWallets(string password)
    {
        var walletNames = Wallet.GetWalletSavedKeys();
        this.wallets.Clear();
        foreach (var walletName in walletNames)
        {
            try
            {
                Wallet wallet = Wallet.RestoreWallet(walletName, password);
                if (wallet != null)
                {
                    if (!this.wallets.ContainsKey(wallet.walletName))
                    {
                        this.wallets.Add(wallet.walletName, wallet);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }

    public List<Wallet> RestoreWallets(string password)
    {
        var walletNames = Wallet.GetWalletSavedKeys();
        List<Wallet> restoredWallets = new();
        foreach (var walletName in walletNames)
        {
            try
            {
                Wallet wallet = Wallet.RestoreWallet(walletName, password);
                if (wallet != null)
                {
                    if (!restoredWallets.Contains(wallet))
                    {
                        restoredWallets.Add(wallet);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
        return restoredWallets;
    }

    /// <summary>
    /// Checks if a wallet with the specified mnemonic exists by iterating through all saved wallets and comparing their mnemonics.
    /// </summary>
    /// <param name="mnemonic">The mnemonic to check for.</param>
    /// <returns>True if a wallet with the specified mnemonic exists, false otherwise.</returns>
    public bool DoesWalletWithMnemonicExists(string mnemonic)
    {
        var walletNames = Wallet.GetWalletSavedKeys();
        
        foreach (var walletName in walletNames)
        {
            Wallet wallet = Wallet.RestoreWallet(walletName, password);
            if (wallet != null)
            {
                if (wallet.mnemonic == mnemonic)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool DoesWalletWithPublicKeyAlreadyExists(string publicKey){
        var walletNames = Wallet.GetWalletSavedKeys();
        
        foreach (var walletName in walletNames)
        {
            Wallet wallet = Wallet.RestoreWallet(walletName, password);
            if (wallet != null)
            {
                if (wallet.publicKey == publicKey)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public Dictionary<string, Wallet> GetAllWallets()
    {
        RestoreAllWallets(password);
        return wallets;
    }

    public Wallet GetWalletByIndex(int index)
    {
        var walletNames = Wallet.GetWalletSavedKeys();
        if (walletNames.Count > index)
        {
            return Wallet.RestoreWallet(walletNames[index], password);
        }
        return null;
    }

    public Wallet GetWalletByName(string walletName)
    {
        if (wallets.ContainsKey(walletName))
        {
            return wallets[walletName];
        }
        return null;
    }

    /// <summary>
    /// Returns the wallet with the specified public key.
    /// </summary>
    /// <param name="publicKey">The public key of the wallet to retrieve.</param>
    /// <returns>The wallet with the specified public key, or null if no wallet was found.</returns>
    public Wallet GetWalletByPublicKey(string publicKey)
    {
        foreach (var wallet in wallets.Values)
        {
            if (wallet.publicKey == publicKey)
            {
                return wallet;
            }
        }
        return null;
    }

    /// <summary>
    /// Removes the wallet with the specified name from the list of wallets and clears it from memory.
    /// </summary>
    /// <param name="walletName">The name of the wallet to remove.</param>
    public void RemoveWalletByName(string walletName)
    {
        if (wallets.ContainsKey(walletName))
        {
            Wallet removedwallet = wallets[walletName];
            removedwallet.RemoveWallet();
            wallets.Remove(walletName);
        }
    }

    /// <summary>
    /// Removes the wallet with the specified public key from the list of wallets and clears it from memory.
    /// </summary>
    /// <param name="publicKey">The public key of the wallet to remove.</param>
    public void RemoveWalletByPublicKey(string publicKey)
    {
        foreach (var wallet in wallets.Values)
        {
            if (wallet.publicKey == publicKey)
            {
                wallet.RemoveWallet();
                wallets.Remove(wallet.walletName);
                break;
            }
        }
    }

    /// <summary>
    /// Removes the specified wallet from the list of wallets and clears it from memory.
    /// </summary>
    /// <param name="wallet">The wallet to remove.</param>
    public void RemoveWallet(Wallet wallet)
    {
        wallets.Remove(wallet.walletName);
        wallet.RemoveWallet();
    }

    /// <summary>
    /// Removes all wallets from the list of wallets and clears the current wallet and password.
    /// </summary>
    public void RemoveAllWallets()
    {
        wallets.Clear();

        currentWallet = null;
        password = "";

        PlayerPrefs.DeleteAll();
    }
    #endregion

    public void RequestSignMessage(byte[] message){
        if(currentWallet == null)
        {
            screenManager.ShowScreen("LoginScreen", message);
            return;
        }

        screenManager.ShowScreen("SignScreen", message);
    }

    public byte[] SignMessageWithCurrentWallet(byte[] message)
    {
        if(currentWallet == null)
        {
            return new byte[0];
        }
        var signature = currentWallet.Sign(message);
        return signature;
    }

    public byte[] SignMessageWithWallet(Wallet wallet, byte[] message)
    {
        var signature = wallet.Sign(message);
        return signature;
    }

    /// <summary>
    /// Retrieves all coin data for an owner.  
    /// </summary>
    /// <param name="owner">The address of the owner of the coins.</param>
    public async Task GetDataForAllCoins(string owner)
    {
        var coins = new PageForCoinAndObjectID();
        try
        {
            coins = await GetAllCoins(owner);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return;
        }

        await GetCoins(coins);
        await GetCoinImageData();
    }

    private async Task GetCoins(PageForCoinAndObjectID coins)
    {
        if(coins == null)
            return;

        if (coins.data == null)
            return;
            
        foreach (var coin in coins.data)
        {
            if (!coinPages.ContainsKey(coin.coinType))
            {
                coinPages.TryAdd(coin.coinType, coin);
            }
        }

        foreach (var coinType in coinPages.Keys)
        {
            if (coinMetadatas.ContainsKey(coinType))
                continue;
            var coinMetadata = await GetCoinMetadata(coinType);

            if (coinMetadata != null)
            {
                //remove this picece of code later
                if(coinMetadata.symbol == "SCP")
                {
                    coinMetadata.symbol = "AART";
                    coinMetadata.name = "AllArt";
                }

                coinMetadatas.TryAdd(coinType, coinMetadata);
            }
        }
    }

    public Sprite GetCoinImage(string symbol)
    {
        if(coinImages.ContainsKey(symbol.ToLower()))
        {
            return coinImages[symbol.ToLower()];
        }
        return null;
    }

    private async Task GetCoinImageData()
    {
        foreach (var coin in suiMarketData)
        {
            //check for sui and bonk
            //hardcoded values for now
            if(coinImages.Keys.Contains(coin.symbol))
            {
                continue;
            }

            if(coin.symbol == "sui")
            {
                coinImages.TryAdd(coin.symbol, suiIcon);
                continue;
            }
            if(coin.symbol == "scp")
            {
                coinImages.TryAdd(coin.symbol, bonkIcon);
                continue;
            }

            if (!string.IsNullOrEmpty(coin.image))
            {
                if (coinImages.ContainsKey(coin.symbol))
                {
                    continue;
                }
                coin.image = coin.image.Replace("/large/", "/small/");
                var image = await GetImage(coin.image);
                coinImages.TryAdd(coin.symbol, image);        
            }
        }
    }

    public SUIMarketData GetCoinMarketData(string symbol)
    {
        foreach (var coin in suiMarketData)
        {
            if (coin.symbol.ToLower() == symbol.ToLower())
            {
                return coin;
            }
        }
        return null;
    }

    /// <summary>
    /// Retrieves all coin data for a specific owner.
    /// </summary>
    /// <param name="owner">The address of the owner of the coins.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. The result of the task contains a <see cref="PageForCoinAndObjectID"/> object representing the page of coin data for the specified owner.</returns>
    public async Task<PageForCoinAndObjectID> GetAllCoins(string owner)
    {
        var request = await client.GetAllCoins(owner);
        return request.result;
    }

    /// <summary>
    /// Retrieves the current USD price for a given coin symbol from the CoinGecko API.
    /// </summary>
    /// <param name="coinMetadata">The metadata for the coin to retrieve the price for.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. The result of the task contains a <see cref="GeckoCoinData"/> object representing the current USD price for the specified coin symbol.</returns>
    public async Task<GeckoCoinData> GetUSDPrice(CoinMetadata coinMetadata)
    {
        var rpc = new RPCClient("");
        string reqUrl = $"{SUIConstantVars.coingeckoApi}{coinMetadata.symbol.ToLower()}?localization=false";
        var data = await rpc.Get<GeckoCoinData>(reqUrl);
        return data;
    }

    public async Task<List<SUIMarketData>> GetCoingeckoCoinValues()
    {
        var rpc = new RPCClient("");
        string reqUrl = "https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&ids=bonk%2Call-art%2Cbluemove%2Cmeadow%2Cturbos-finance%2Csuipad%2Cusdcet%2Csuia%2Csuifloki-inu%2Cusd-coin-wormhole-bnb%2Cseapad%2Csuipepe%2Csuitizen%2Cusdtet%2Cusd-coin-wormhole-arb%2Creleap%2Cflame-protocol%2Csui%2Ccetus-protocol%2Cusd-coin%2Cwrapped-solana%2C&order=market_cap_desc&per_page=100&page=1&sparkline=false&locale=en";
        var data = await rpc.Get<List<SUIMarketData>>(reqUrl);
        return data;
    }

    public async Task<List<CoinID>> GetAllCoingeckoIds()
    {
        var rpc = new RPCClient("");
        string reqUrl = "https://api.coingecko.com/api/v3/coins/list";
        var data = await rpc.Get<List<CoinID>>(reqUrl);
        return data;
    }

    /// <summary>
    /// Retrieves the current USD price for a given coin symbol from the CoinGecko API.
    /// </summary>
    /// <param name="coinId">The symbol of the coin to retrieve the price for.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. The result of the task contains a <see cref="GeckoCoinData"/> object representing the current USD price for the specified coin symbol.</returns>
    public async Task<GeckoCoinData> GetUSDPrice(string coinId)
    {
        var rpc = new RPCClient("");
        string reqUrl = $"{SUIConstantVars.coingeckoApi}{coinId.ToLower()}?localization=false";
        var data = await rpc.Get<GeckoCoinData>(reqUrl);
        return data;
    }

    /// <summary>
    /// Retrieves an image from the specified URL using the CoinGecko API.
    /// </summary>
    /// <param name="url">The URL of the image to retrieve.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. The result of the task contains a <see cref="Sprite"/> object representing the retrieved image.</returns>
    public async Task<Sprite> GetImage(string url)
    {
        var rpc = new RPCClient(SUIConstantVars.coingeckoApi);
        var data = await rpc.DownloadImage(url);
        return data;
    }

    /// <summary>
    /// Applies the decimal places for a given balance based on the coin metadata.
    /// </summary>
    /// <param name="balance">The balance to apply the decimal places to.</param>
    /// <param name="coinMetadata">The metadata for the coin to apply the decimal places for.</param>
    /// <returns>The balance with the decimal places applied.</returns>
    public static decimal ApplyDecimals(Balance balance, CoinMetadata coinMetadata)
    {
        return balance.totalBalance / (decimal)Mathf.Pow(10f, coinMetadata.decimals);
    }

    /// <summary>
    /// Applies the decimal places for a given balance based on the coin metadata.
    /// </summary>
    /// <param name="balance">The balance to apply the decimal places to.</param>
    /// <param name="coinMetadata">The metadata for the coin to apply the decimal places for.</param>
    /// <returns>The balance with the decimal places applied.</returns>
    public static decimal ApplyDecimals(ulong balance, CoinMetadata coinMetadata)
    {
        return balance / (decimal)Mathf.Pow(10f, coinMetadata.decimals);
    }

    public static decimal ApplyDecimals(long balance, CoinMetadata coinMetadata)
    {
        return balance / (decimal)Mathf.Pow(10f, coinMetadata.decimals);
    }

    public async Task<List<Balance>> GetAllBalances(Wallet wallet)
    {
        var request = await client.GetAllBalances(wallet);
        return request.result;
    }

    #region RPC methods

    /// <summary>
    /// Retrieves a page of SUI objects of a specific type that are owned by the specified wallet.
    /// </summary>
    /// <param name="wallet">The wallet that owns the SUI objects.</param>
    /// <param name="type">The type of SUI objects to retrieve.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. The result of the task contains a <see cref="Page_for_SuiObjectResponse_and_ObjectID"/> object representing the page of SUI objects of the specified type that are owned by the specified wallet.</returns>
    public async Task<Page_for_SuiObjectResponse_and_ObjectID> GetOwnedObjectsOfType(Wallet wallet, string type)
    {
        ObjectResponseQuery coinQuery = new();
        var coinFilter = new MatchAllDataFilter
        {
            MatchAll = new List<ObjectDataFilter>
            {
                new StructTypeDataFilter() { StructType =  $"0x2::coin::Coin<{type}>"},
                new OwnerDataFilter() { AddressOwner = wallet.publicKey }
            }
        };
        coinQuery.filter = coinFilter;
        ObjectDataOptions options = new();
        coinQuery.options = options;

        var ownedCoins = await WalletComponent.Instance.GetOwnedObjects(wallet.publicKey, coinQuery, null, 3);
        return ownedCoins;
    }

    /// <summary>
    /// Sends a payment transaction from the specified wallet to the specified recipients.
    /// </summary>
    /// <param name="wallet">The wallet to send the payment from.</param>
    /// <param name="inputCoins">The input coins to use for the payment.</param>
    /// <param name="recipients">The recipients of the payment.</param>
    /// <param name="amounts">The amounts to send to each recipient.</param>
    /// <param name="gas">The gas price to use for the transaction.</param>
    /// <param name="gasBudget">The maximum gas budget for the transaction.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. The result of the task contains a <see cref="TransactionBlockBytes"/> object representing the transaction bytes for the payment transaction.</returns>
    public async Task<JsonRpcResponse<TransactionBlockBytes>> Pay(Wallet wallet, string[] inputCoins, string[] recipients, string[] amounts, string gas, string gasBudget)
    {
        var request = await client.Pay(wallet, inputCoins, recipients, amounts, gas, gasBudget);
        return request;
    }

    /// <summary>
    /// Sends a payment transaction in SUI from the specified wallet to the specified recipients.
    /// </summary>
    /// <param name="wallet">The wallet to send the payment from.</param>
    /// <param name="inputCoins">The input coins to use for the payment.</param>
    /// <param name="recipients">The recipients of the payment.</param>
    /// <param name="amounts">The amounts to send to each recipient.</param>
    /// <param name="gasBudget">The maximum gas budget for the transaction.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. The result of the task contains a <see cref="TransactionBlockBytes"/> object representing the transaction bytes for the payment transaction.</returns>
    public async Task<JsonRpcResponse<TransactionBlockBytes>> PaySui(Wallet wallet, List<string> inputCoins, List<string> recipients, List<string> amounts, string gasBudget)
    {
        var request = await client.PaySui(wallet, inputCoins, recipients, amounts, gasBudget);
        return request;
    }

    public async Task<JsonRpcResponse<TransactionBlockBytes>> PayAllSui(Wallet wallet, List<string> inputCoins, string recipients, string gasBudget)
    {
        var request = await client.PayAllSui(wallet, inputCoins, recipients, gasBudget);
        return request;
    }

    /// <summary>
    /// Retrieves a page of SUI objects owned by the specified account that match the specified query.
    /// </summary>
    /// <param name="account">The account that owns the SUI objects.</param>
    /// <param name="query">The query to match the SUI objects against.</param>
    /// <param name="objectId">The ID of the object to start the page from.</param>
    /// <param name="limit">The maximum number of objects to return in the page.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. The result of the task contains a <see cref="Page_for_SuiObjectResponse_and_ObjectID"/> object representing the page of SUI objects that match the specified query and are owned by the specified account.</returns>
    public async Task<Page_for_SuiObjectResponse_and_ObjectID> GetOwnedObjects(string account, ObjectResponseQuery query, string objectId, uint limit)
    {
        var request = await client.GetOwnedObjects(account, query, objectId, limit);
        return request;
    }

    /// <summary>
    /// Retrieves the SUI object with the specified ID.
    /// </summary>
    /// <param name="objectId">The ID of the SUI object to retrieve.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. The result of the task contains a <see cref="SUIObjectResponse"/> object representing the retrieved SUI object.</returns>
    public async Task<SUIObjectResponse> GetObject(string objectId)
    {
        var request = await client.GetObject(objectId);
        return request;
    }

    /// <summary>
    /// Retrieves metadata for a specific coin type.
    /// </summary>
    /// <param name="coinType">The type of coin to retrieve metadata for.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. The result of the task contains a <see cref="CoinMetadata"/> object representing the metadata for the specified coin type.</returns>
    public async Task<CoinMetadata> GetCoinMetadata(string coinType)
    {
        var request = await client.GetCoinMetadata(coinType);
        return request.result;
    }

    /// <summary>
    /// Retrieves the balance of a specified coin type for the specified account.
    /// </summary>
    /// <param name="account">The account to retrieve the balance for.</param>
    /// <param name="coinType">The type of coin to retrieve the balance for.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. The result of the task contains a <see cref="Balance"/> object representing the balance of the specified coin type for the specified account.</returns>
    public async Task<Balance> GetBalance(string account, string coinType)
    {
        var request = await client.GetBalance(account, coinType);
        return request.result;
    }

    /// <summary>
    /// Retrieves the balance of a specified coin type for the specified account.
    /// </summary>
    /// <param name="account">The wallet to retrieve the balance for.</param>
    /// <param name="coinType">The type of coin to retrieve the balance for.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. The result of the task contains a <see cref="Balance"/> object representing the balance of the specified coin type for the specified account.</returns>
    public async Task<Balance> GetBalance(Wallet account, string coinType)
    {
        var request = await client.GetBalance(account, coinType);
        return request.result;
    }

    /// <summary>
    /// Changes the password for all wallets by restoring them with the old password and saving them with the new password.
    /// </summary>
    /// <param name="oldPassword">The old password to restore the wallets with.</param>
    /// <param name="newPassword">The new password to save the wallets with.</param>
    internal void ChangePassword(string oldPassword, string newPassword)
    {
        RestoreAllWallets(oldPassword);

        foreach (var wallet in wallets.Values)
        {
            wallet.UpdateWalletAndSave(newPassword);
        }
    }

    public async Task<JsonRpcResponse<SuiTransactionBlockResponse>> DryRunTransaction(string transactionBlockBytes)
    {
        var request = await client.DryRunTransactionBlock(transactionBlockBytes);
        
        return request;
    }

    /// <summary>
    /// Retrieves a list of SUI transaction blocks for the currently selected wallet.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. The result of the task contains a <see cref="List{T}"/> of <see cref="SuiTransactionBlockResponse"/> objects representing the retrieved SUI transaction blocks.</returns>
    public async Task<List<SuiTransactionBlockResponse>> GetTransactionsForSelectedWallet()
    {
        var transactions = new List<SuiTransactionBlockResponse>();
        
        var requestFrom = await client.QueryTransactionBlocks(
            new SuiTransactionBlockResponseQuery(){
                filter = new FromAddressFilter(currentWallet.publicKey),
                options = new TransactionBlockResponseOptions(true, false, true, true, true, true)
            }
        );

        var requestTo = await client.QueryTransactionBlocks(
            new SuiTransactionBlockResponseQuery(){
                filter = new ToAddressFilter(currentWallet.publicKey),
                options = new TransactionBlockResponseOptions(true, false, true, true, true, true)
            }
        );

        if(requestFrom != null){

            foreach (var transactionBlock in requestFrom.result.data)
            {
                if (!transactions.Contains(transactionBlock))
                    transactions.Add(transactionBlock);
            }
        }

        foreach (var transactionBlock in requestTo.result.data)
        {
            if (!transactions.Contains(transactionBlock))
                transactions.Add(transactionBlock);
        }

        var cleanList = transactions.GroupBy(x => x.digest).Select(x => x.First()).ToList();

        return cleanList;
    }

    public async Task<string> GetReferenceGasPrice()
    {
        var request = await client.GetReferenceGasPrice();
        return request.result;
    }

    public async Task<PageForTransactionBlockResponseAndTransactionDigest> GetTransactionsForWallet(Wallet wallet)
    {
        FromAndToAddressFilter transactionInputObject = new(new FromToObject(wallet.publicKey, wallet.publicKey));
        var request = await client.QueryTransactionBlocks(transactionInputObject);
        return request.result;
    }

    #endregion

    internal void LockWallet()
    {
        password = "";
        wallets.Clear();
    }

    internal void ToggleWallet()
    {
        screenManager?.ToggleAllScreens();
    }
}
