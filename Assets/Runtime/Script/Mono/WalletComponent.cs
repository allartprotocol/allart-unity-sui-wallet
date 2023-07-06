using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public WebsocketController websocketController;

    public Dictionary<string, CoinMetadata> coinMetadatas = new();
    public Dictionary<string, GeckoCoinData> coinData = new();
    public Dictionary<string, Sprite> coinImages = new();

    public Wallet currentWallet;
    public CoinMetadata currentCoinMetadata;

    public string nodeAddress
    {
        get
        {
            switch (nodeType)
            {
                case ENodeType.MainNet:
                    return SUIConstantVars.mainNetNode;
                case ENodeType.TestNet:
                    return SUIConstantVars.testNetNode;
                case ENodeType.DevNet:
                    return SUIConstantVars.devNetNode;
                default:
                    return SUIConstantVars.mainNetNode;
            }
        }
    }

    private string _password;
    public string password
    {
        get { return _password; }
        private set
        {
            _password = value;
            StartTimer();
        }
    }

    public ENodeType nodeType { get; private set; }

    Coroutine timeoutTimer;

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
    }

    private void OnDisable()
    {
        password = null;
        if (timeoutTimer != null)
            StopCoroutine(timeoutTimer);
    }

    #endregion

    #region Timer 

    public void StartTimer()
    {
        if (timeoutTimer != null)
            StopCoroutine(timeoutTimer);
        timeoutTimer = StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        float time = -1;
        if(PlayerPrefs.HasKey("timeout")){
            time = PlayerPrefs.GetFloat("timeout");
        }
        if(time == -1){
            time = 10000000;
        }
        yield return new WaitForSeconds(time);
        websocketController?.Stop();
        password = null;
        var manager = FindObjectOfType<SimpleScreen.SimpleScreenManager>();
        manager?.ShowScreen("SplashScreen");
        InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Warning, "Connection timed out. Please try again.");

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
    /// Sets the current wallet to the wallet at the specified index in the list of wallets and subscribes to its transactions.
    /// </summary>
    /// <param name="value">The index of the wallet to set as the current wallet.</param>
    internal void SetWalletByIndex(int value)
    {
        SetCurrentWallet(GetWalletByIndex(value));
    }

    /// <summary>
    /// Sets the current wallet to the specified wallet and subscribes to its transactions using the websocket controller.
    /// </summary>
    /// <param name="wallet">The wallet to set as the current wallet.</param>
    public async void SetCurrentWallet(Wallet wallet)
    {
        currentWallet = wallet;

        var fromToFilter = new FromAndToAddressFilter(new FromToObject(wallet.publicKey, wallet.publicKey));
        await GetTransactionsForSelectedWallet();
        await websocketController.Subscribe(fromToFilter);
    }

    public void SetPassword(string password)
    {
        this.password = password;
    }

    /// <summary>
    /// Checks if the given password is valid by attempting to restore all wallets with the given password.
    /// </summary>
    /// <param name="password">The password to check for validity.</param>
    /// <returns>True if the password is valid and can be used to restore wallets, false otherwise.</returns>
    public bool CheckPasswordValidity(string password)
    {
        if (password.Length < 1 || password.Contains(" ") || string.IsNullOrEmpty(password))
        {
            return false;
        }

        try
        {
            RestoreAllWallets(password);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }

        if (wallets.Count > 0)
        {
            return true;
        }
        return false;
    }

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
        wallet.SaveWallet();
        return wallet;
    }

    /// <summary>
    /// Restores all wallets saved on the device with the given password and adds them to the list of wallets.
    /// </summary>
    /// <param name="password">The password to use for restoring the wallets.</param>
    public void RestoreAllWallets(string password)
    {
        var walletNames = Wallet.GetWalletSavedKeys();
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
                Debug.Log(e.Message);
            }
        }
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

    public Dictionary<string, Wallet> GetAllWallets()
    {
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

    public void RemoveWalletByName(string walletName)
    {
        if (wallets.ContainsKey(walletName))
        {
            Wallet removedwallet = wallets[walletName];
            removedwallet.RemoveWallet();
            wallets.Remove(walletName);
        }
    }

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

    public void RemoveWallet(Wallet wallet)
    {
        wallet.RemoveWallet();
        wallets.Remove(wallet.walletName);
    }

    public void RemoveAllWallets()
    {
        wallets.Clear();

        currentWallet = null;
        password = "";

        PlayerPrefs.DeleteAll();
    }

    /// <summary>
    /// Retrieves all coin data for an owner.  
    /// </summary>
    /// <param name="owner">The address of the owner of the coins.</param>
    public async Task GetDataForAllCoins(string owner)
    {
        var coins = await GetAllCoins(owner);

        if (coins != null)
        {
            List<string> coinTypes = new();
            foreach (var coin in coins.data)
            {
                if (!coinTypes.Contains(coin.coinType))
                {
                    coinTypes.Add(coin.coinType);
                }
            }

            foreach (var coinType in coinTypes)
            {
                if (coinMetadatas.ContainsKey(coinType))
                    continue;
                var coinMetadata = await GetCoinMetadata(coinType);
                if (coinMetadata != null)
                {
                    coinMetadatas.Add(coinType, coinMetadata);
                }
            }

            foreach (var coin in coinMetadatas.Keys)
            {
                var coinMetadata = coinMetadatas[coin];
                if (this.coinData.ContainsKey(coinMetadata.symbol))
                    continue;
                var coinData = await GetUSDPrice(coinMetadata);
                if (coinData != null)
                {
                    this.coinData.Add(coinMetadata.symbol, coinData);
                }
            }

            foreach (var data in coinData.Keys)
            {
                var coin = coinData[data];
                if (coin.image != null && coin.image.thumb != null)
                {
                    if (coinImages.ContainsKey(data))
                    {
                        continue;
                    }
                    var image = await GetImage(coin.image.thumb);
                    if (image != null)
                    {
                        coinImages.Add(data, image);
                    }
                }
            }
        }
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

    /// <summary>
    /// Retrieves the current USD price for a given coin symbol from the CoinGecko API.
    /// </summary>
    /// <param name="coinSymbol">The symbol of the coin to retrieve the price for.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. The result of the task contains a <see cref="GeckoCoinData"/> object representing the current USD price for the specified coin symbol.</returns>
    public async Task<GeckoCoinData> GetUSDPrice(string coinSymbol)
    {
        var rpc = new RPCClient("");
        string reqUrl = $"{SUIConstantVars.coingeckoApi}{coinSymbol.ToLower()}?localization=false";
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
    public static float ApplyDecimals(Balance balance, CoinMetadata coinMetadata)
    {
        return balance.totalBalance / Mathf.Pow(10f, coinMetadata.decimals);
    }

    public async Task<List<Balance>> GetAllBalances(Wallet wallet)
    {
        var request = await client.GetAllBalances(wallet);
        return request.result;
    }

    public async Task<TransactionBlockBytes> Pay(Wallet wallet, string[] inputCoins, string[] recipients, string[] amounts, string gas, string gasBudget)
    {

        var request = await client.Pay(wallet, inputCoins, recipients, amounts, gas, gasBudget);
        return request;
    }

    public async Task<TransactionBlockBytes> PaySui(Wallet wallet, List<string> inputCoins, List<string> recipients, List<string> amounts, string gasBudget)
    {
        var request = await client.PaySui(wallet, inputCoins, recipients, amounts, gasBudget);
        return request;
    }

    public async Task<Page_for_SuiObjectResponse_and_ObjectID> GetOwnedObjects(string account, ObjectResponseQuery query, string objectId, uint limit)
    {
        var request = await client.GetOwnedObjects(account, query, objectId, limit);
        return request;
    }

    public async Task<SUIObjectResponse> GetObject(string objectId)
    {
        var request = await client.GetObject(objectId);
        return request;
    }

    public async Task<CoinMetadata> GetCoinMetadata(string coinType)
    {
        var request = await client.GetCoinMetadata(coinType);
        return request.result;
    }

    public async Task<Balance> GetBalance(string account, string coinType)
    {
        var request = await client.GetBalance(account, coinType);
        return request.result;
    }

    public async Task<Balance> GetBalance(Wallet account, string coinType)
    {
        var request = await client.GetBalance(account, coinType);
        return request.result;
    }

    internal void ChangePassword(string oldPassword, string newPassword)
    {
        RestoreAllWallets(oldPassword);

        foreach (var wallet in wallets.Values)
        {
            wallet.SaveWallet(newPassword);
        }
    }

    public async Task<List<SuiTransactionBlockResponse>> GetTransactionsForSelectedWallet()
    {
        ObjectResponseQuery query = new()
        {
            filter = null,
            options = null
        };

        var ownedObj = await GetOwnedObjects(currentWallet.publicKey, query, null, 50);

        if (ownedObj == null) return null;

        if (ownedObj.data == null) return null;

        List<string> digests = new();

        foreach (var obj in ownedObj.data)
        {
            var request = await client.QueryTransactionBlocks(
                new Filter(
                    new InputObjectFilter(obj.data.objectId)
                ));

            if (request == null) continue;
            if (request.result == null) continue;

            foreach (var transactionBlock in request.result.data)
            {
                if (!digests.Contains(transactionBlock.digest))
                    digests.Add(transactionBlock.digest);
            }
        }

        var transactions = await client.MultiGetTransactionBlocks(digests, new TransactionBlockResponseOptions());

        return transactions;
    }

    public async Task<PageForTransactionBlockResponseAndTransactionDigest> GetTransactionsForWallet(Wallet wallet)
    {
        FromAndToAddressFilter transactionInputObject = new(new FromToObject(wallet.publicKey, wallet.publicKey));
        var request = await client.QueryTransactionBlocks(transactionInputObject);
        return request.result;
    }

    internal void LockWallet()
    {
        password = null;
    }
}
