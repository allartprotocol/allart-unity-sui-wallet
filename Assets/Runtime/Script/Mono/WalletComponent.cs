using System;
using System.Collections.Generic;
using System.Numerics;
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

    private Dictionary<string, Wallet> wallets = new Dictionary<string, Wallet>();
    public SUIRPCClient client;
    public WebsocketController websocketController;

    public Dictionary<string, CoinMetadata> coinMetadatas = new Dictionary<string, CoinMetadata>();
    public Dictionary<string, GeckoCoinData> coinData = new Dictionary<string, GeckoCoinData>();

    public Wallet currentWallet;
    public CoinMetadata currentCoinMetadata;

    public string nodeAddress { get { 
            switch (_nodeType)
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
        }}

    private string _password;
    public string password
    {
        get { return _password; }
        private set { 
            _password = value;
        }
    }

    public ENodeType _nodeType { get; private set; }


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
        if(PlayerPrefs.HasKey("nodeType"))
        {
            _nodeType = (ENodeType)PlayerPrefs.GetInt("nodeType");
        }
        else
        {
            _nodeType = ENodeType.MainNet;
        }

        client = new SUIRPCClient(nodeAddress);
        websocketController = new WebsocketController(nodeAddress);
    }

    public void SetNodeAddress(ENodeType nodeAddress)
    {
        if(nodeAddress == _nodeType)
        {
            return;
        }
        PlayerPrefs.SetInt("nodeType", (int)nodeAddress);
        
        this._nodeType = nodeAddress;
        client = new SUIRPCClient(this.nodeAddress);
        websocketController.Stop();
        websocketController = new WebsocketController(this.nodeAddress);
    }

    internal void SetWalletByIndex(int value)
    {
        SetCurrentWallet(GetWalletByIndex(value));
    }

    public async void SetCurrentWallet(Wallet wallet)
    {
        currentWallet = wallet;

        SenderFilter filter = new SenderFilter(currentWallet.publicKey);
        RecipientFilter recipientFilter = new RecipientFilter(currentWallet.publicKey);
        FilterOr filterOr = new FilterOr(new List<object> { filter, recipientFilter });
        await GetTransactionsForWallet(currentWallet);
        await websocketController.Subscribe(new List<object> { filter });
    }

    public void SetPassword(string password)
    {
        this.password = password;
    }

    public bool CheckPasswordValidity(string password)
    {
        if(password.Length < 1 || password.Contains(" ") || string.IsNullOrEmpty(password))
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

        if(wallets.Count > 0)
        {
            return true;
        }
        return false;
    }

    public Wallet CreateNewWallet(string password = "password", string walletName = "")
    {
        Wallet wallet = new Wallet(password, walletName);
        wallet.SaveWallet();
        return wallet;
    }

    public Wallet CreateWallet(string mnemonic, string password = "password", string walletName = "")
    {
        Wallet wallet = new Wallet(mnemonic, password, walletName);
        wallet.SaveWallet();
        return wallet;
    }

    public void RestoreAllWallets(string password)
    {
        var walletNames = Wallet.GetWalletSavedKeys();
        foreach (var walletName in walletNames)
        {
            Wallet wallet = Wallet.RestoreWallet(walletName, password);
            if (wallet != null)
            {
                if(!this.wallets.ContainsKey(wallet.walletName))
                {
                    this.wallets.Add(wallet.walletName, wallet);
                }
            }
        }
    }

    public bool DoesWalletWithMnemonicExists(string mnemonic) { 
        
        var walletNames = Wallet.GetWalletSavedKeys();
        foreach (var walletName in walletNames)
        {
            Wallet wallet = Wallet.RestoreWallet(walletName, password);
            if (wallet != null)
            {
                return true;
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

    public void RemoveWallet(Wallet wallet) {
        wallet.RemoveWallet();
        wallets.Remove(wallet.walletName);
    }

    public void RemoveAllWallets()
    {
        foreach (var wallet in wallets.Values)
        {
            wallet.RemoveWallet();
        }
        wallets.Clear();
    }

    public async Task GetDataForAllCoins(string owner)
    {
        var coins = await GetAllCoins(owner);

        if(coins != null)
        {
            List<string> coinTypes = new List<string>();
            foreach (var coin in coins.data)
            {
                if (!coinTypes.Contains(coin.coinType))
                {
                    coinTypes.Add(coin.coinType);
                }
            }

            foreach (var coinType in coinTypes)
            {
                if(coinMetadatas.ContainsKey(coinType))
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
        }
    }

    public async Task<PageForCoinAndObjectID> GetAllCoins(string owner)
    {
        var request = await client.GetAllCoins(owner);
        return request.result;
    }

    public async Task<GeckoCoinData> GetUSDPrice(CoinMetadata coinMetadata)
    {
        var rpc = new RPCClient("");
        string reqUrl = $"https://api.coingecko.com/api/v3/coins/{coinMetadata.symbol.ToLower()}?localization=false";
        var data = await rpc.Get<GeckoCoinData>(reqUrl);
        return data;
    }

    public async Task<GeckoCoinData> GetUSDPrice(string coinSymbol)
    {
        var rpc = new RPCClient("");
        string reqUrl = $"https://api.coingecko.com/api/v3/coins/{coinSymbol.ToLower()}?localization=false";
        var data = await rpc.Get<GeckoCoinData>(reqUrl);
        return data;
    }

    public async Task<Sprite> GetImage(string url)
    {
        var rpc = new RPCClient($"https://api.coingecko.com/api/v3/coins/");
        var data = await rpc.DownloadImage(url);
        return data;
    }

    public static float ApplyDecimals(Balance balance, CoinMetadata coinMetadata)
    {
        return (balance.totalBalance / Mathf.Pow(10f, coinMetadata.decimals));
    }

    public async Task<List<Balance>> GetAllBalances(Wallet wallet)
    {
        var request = await client.GetAllBalances(wallet);
        return request.result;
    }

    //public async Task<TransactionBlockBytes> Pay(Wallet wallet, ObjectId[] inputCoins, SUIAddress[] recipients, BigInteger[] amounts, ObjectId gas, BigInteger gasBudget)
    //{

    //    var request = await client.Pay(wallet, inputCoins, recipients, amounts, gas, gasBudget);
    //    return request;
    //}

    public async Task<TransactionBlockBytes> Pay(Wallet wallet, string[] inputCoins, string[] recipients, string[] amounts, string gas, string gasBudget)
    {

        var request = await client.Pay(wallet, inputCoins, recipients, amounts, gas, gasBudget);
        return request;
    }

    public async Task<TransactionBlockBytes> PaySui(Wallet wallet, string inputCoins, string recipients, ulong amounts, string gasBudget)
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

    public async Task<PageForEventAndEventID> GetTransactionsForWallet(Wallet wallet) {
        SenderFilter filter = new SenderFilter(currentWallet.publicKey);
        RecipientFilter recipientFilter = new RecipientFilter(currentWallet.publicKey);
        FilterOr filterOr = new FilterOr(new List<object> { filter, recipientFilter });
        var filters = new List<object> { filter };
        var eventQuery = new EventQuery(filters);
        var request = await client.QueryEvents(filter);
        return request.result;
    }

    internal void LockWallet()
    {
        password = null;
    }
}
