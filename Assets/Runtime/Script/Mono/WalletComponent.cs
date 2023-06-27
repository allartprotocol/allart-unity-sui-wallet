using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;

public class WalletComponent : MonoBehaviour
{
    public static WalletComponent Instance { get; private set; }

    private Dictionary<string, Wallet> wallets = new Dictionary<string, Wallet>();
    public SUIRPCClient client;

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
        client = new SUIRPCClient("https://fullnode.devnet.sui.io:443/");
    }

    public bool CheckPasswordValidity(string password)
    {
        if(password.Length < 1 || password.Contains(" ") || string.IsNullOrEmpty(password))
        {
            return false;
        }
        Debug.Log("RESTORE!");

        RestoreAllWallets(password);

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
        this.wallets.Add(wallet.walletName, wallet);
        return wallet;
    }

    public Wallet CreateWallet(string mnemonic, string password = "password", string walletName = "")
    {
        Wallet wallet = new Wallet(mnemonic, password, walletName);
        wallet.SaveWallet();
        this.wallets.Add(wallet.walletName, wallet);
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
                this.wallets.Add(wallet.walletName, wallet);
            }
        }
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
            return Wallet.RestoreWallet(walletNames[index], "password");
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

    public void RemoveAllWallets()
    {
        foreach (var wallet in wallets.Values)
        {
            wallet.RemoveWallet();
        }
        wallets.Clear();
    }

    public async Task<List<Balance>> GetAllBalances(Wallet wallet)
    {
        var request = await client.GetAllBalances(wallet);
        return request.result;
    }

    public async Task<TransactionBlockBytes> Pay(Wallet wallet, ObjectId[] inputCoins, SUIAddress[] recipients, BigInteger[] amounts, ObjectId gas, BigInteger gasBudget)
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
}
