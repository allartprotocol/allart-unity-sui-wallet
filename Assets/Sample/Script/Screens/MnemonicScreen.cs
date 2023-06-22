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

    string suiRpc = "https://fullnode.devnet.sui.io:443/";

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
            new StructTypeDataFilter() { StructType = "0x2::coin::Coin<0x2::sui::SUI>" },
            new OwnerDataFilter() { AddressOwner = wallet.publicKey }
        };
        query.filter = filter;
        ObjectDataOptions options = new ObjectDataOptions();
        query.options = options;
        var res = await walletComponent.GetOwnedObjects(wallet.publicKey, query, null, 3);
        Debug.Log(JsonConvert.SerializeObject(res));
        var res_pay = await walletComponent.PaySui(wallet, res.data[0].data.objectId, "0xc4b63f85eea06ed2874d61dc3a6816818299e4286f6918546d666383a74529c2",
            1000000000ul, "1000000000");

        Debug.Log(JsonConvert.SerializeObject(res_pay));
        var signature = wallet.SignData(Wallet.GetMessageWithIntent(CryptoBytes.FromBase64String(res_pay.txBytes)));

        var transaction = await walletComponent.client.ExecuteTransactionBlock(res_pay.txBytes,
            new string[] { signature }, new ObjectDataOptions(), ExecuteTransactionRequestType.WaitForEffectsCert);

        Debug.Log(JsonConvert.SerializeObject(transaction));
        Debug.Log(wallet.publicKey);
        var result = await walletComponent.GetAllBalances(wallet);
        Debug.Log(JsonConvert.SerializeObject(result));
    }

    private void RegenerateKey()
    {
        string mnem = mnemonicField.text;
        Wallet wallet;
        if (string.IsNullOrEmpty(mnem))
        {
            wallet = walletComponent.CreateNewWallet("password", "");
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
