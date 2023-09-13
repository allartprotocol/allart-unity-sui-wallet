using AllArt.SUI.RPC.Filter.Types;
using AllArt.SUI.RPC.Response;
using AllArt.SUI.Wallets;
using Chaos.NaCl;
using Newtonsoft.Json;
using SimpleScreen;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MnemonicScreen : BaseScreen
{
    public TMP_InputField mnemonicField;
    public TMP_InputField pubKeyField;
    public Button regenerateKeyBtn;
    public Button checkBtn;

    private WalletComponent walletComponent;
    private void Start()
    {
        PlayerPrefs.DeleteAll();
        walletComponent = FindObjectOfType<WalletComponent>();
        regenerateKeyBtn.onClick.AddListener(RegenerateKey);
        checkBtn.onClick.AddListener(CheckKey);
    }

    private async void CheckKey()
    {
        Wallet wallet = walletComponent.GetWalletByIndex(0);
        ObjectResponseQuery query = new ObjectResponseQuery();
        var filter = new MatchAllDataFilter();
        filter.MatchAll = new List<ObjectDataFilter>
        {
            new StructTypeDataFilter() { StructType = SUIConstantVars.suiCoinType },
            new OwnerDataFilter() { AddressOwner = wallet.publicKey }
        };
        query.filter = filter;
        ObjectDataOptions options = new ObjectDataOptions();
        query.options = options;
        var res = await walletComponent.GetOwnedObjects(wallet.publicKey, query, null, 3);
    }

    private void RegenerateKey()
    {
        string mnem = mnemonicField.text;
        Wallet wallet;
        if (string.IsNullOrEmpty(mnem))
        {
            wallet = walletComponent.CreateWalletWithNewMnemonic("password", "");
        }
        else
        {
            wallet = walletComponent.CreateWallet(mnem, "password");
        }

        mnemonicField.text = mnem;
        pubKeyField.text = wallet.publicKey;
    }

    public override void ShowScreen(object data = null)
    {
        base.ShowScreen(data);
    }

    public override void HideScreen()
    {
        base.HideScreen();
    }
}
