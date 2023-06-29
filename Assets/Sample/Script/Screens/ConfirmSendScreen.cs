using Chaos.NaCl;
using Newtonsoft.Json;
using SimpleScreen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmSendScreen : BaseScreen {

    public TextMeshProUGUI to;
    public TextMeshProUGUI fee;

    public Button confirmBtn;
    private TransferData TransferData;

    public Transform loaderScreen;

    private void Start()
    {
        confirmBtn.onClick.AddListener(OnConfirm);
    }

    private async void OnConfirm()
    {
        if(WalletComponent.Instance.currentCoinMetadata.symbol == "SUI")
        {
            await PaySui();
        }
        else
        {
            await PayOtherCurrency();
        }
        manager.ShowScreen("TransactionDone");
    }

    private async System.Threading.Tasks.Task PaySui()
    {
        var wallet = WalletComponent.Instance.currentWallet;
        ObjectResponseQuery query = new ObjectResponseQuery();
        var filter = new MatchAllDataFilter();
        filter.MatchAll = new List<ObjectDataFilter>
        {
            new StructTypeDataFilter() { StructType = "0x2::coin::Coin<0x2::sui::SUI>" },
            new OwnerDataFilter() { AddressOwner = wallet.publicKey }
        };
        loaderScreen.gameObject.SetActive(true);
        query.filter = filter;
        ObjectDataOptions options = new ObjectDataOptions();
        query.options = options;
        var res = await WalletComponent.Instance.GetOwnedObjects(wallet.publicKey, query, null, 3);
        Debug.Log(JsonConvert.SerializeObject(res));
        var res_pay = await WalletComponent.Instance.PaySui(wallet, res.data[0].data.objectId, TransferData.to,
            ulong.Parse(TransferData.amount), "1000000000");

        Debug.Log(JsonConvert.SerializeObject(res_pay));
        var signature = wallet.SignData(Wallet.GetMessageWithIntent(CryptoBytes.FromBase64String(res_pay.txBytes)));

        var transaction = await WalletComponent.Instance.client.ExecuteTransactionBlock(res_pay.txBytes,
            new string[] { signature }, new ObjectDataOptions(), ExecuteTransactionRequestType.WaitForEffectsCert);
        loaderScreen.gameObject.SetActive(false);
    }

    private async System.Threading.Tasks.Task PayOtherCurrency()
    {
        var wallet = WalletComponent.Instance.currentWallet;
        ObjectResponseQuery query = new ObjectResponseQuery();
        var filter = new MatchAllDataFilter();
        filter.MatchAll = new List<ObjectDataFilter>
        {
            new StructTypeDataFilter() { StructType = "0x2::coin::Coin<0x2::sui::SUI>" },
            new OwnerDataFilter() { AddressOwner = wallet.publicKey }
        };
        loaderScreen.gameObject.SetActive(true);
        query.filter = filter;
        ObjectDataOptions options = new ObjectDataOptions();
        query.options = options;
        var res = await WalletComponent.Instance.GetOwnedObjects(wallet.publicKey, query, null, 3);
        Debug.Log(JsonConvert.SerializeObject(res));
        var res_pay = await WalletComponent.Instance.Pay(wallet,
            new string[] { res.data[0].data.objectId },
            new string[] { TransferData.to },
            new string[] { TransferData.amount },
            res.data[0].data.objectId,
            "1000000000");

        Debug.Log(JsonConvert.SerializeObject(res_pay));
        var signature = wallet.SignData(Wallet.GetMessageWithIntent(CryptoBytes.FromBase64String(res_pay.txBytes)));

        var transaction = await WalletComponent.Instance.client.ExecuteTransactionBlock(res_pay.txBytes,
            new string[] { signature }, new ObjectDataOptions(), ExecuteTransactionRequestType.WaitForEffectsCert);
        loaderScreen.gameObject.SetActive(false);
        manager.ShowScreen("TransactionDone");
    }


    public override void ShowScreen(object data = null)
    {
        base.ShowScreen(data);

        var confirmSendData = data as TransferData;
        Debug.Log(confirmSendData.to);
        if (confirmSendData != null)
        {
            TransferData = confirmSendData;
            to.text = confirmSendData.to;
            Debug.LogError("ConfirmSendScreen: data is null");
            return;
        }
    }


}
