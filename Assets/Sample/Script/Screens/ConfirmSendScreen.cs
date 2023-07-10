using AllArt.SUI.RPC.Filter.Types;
using AllArt.SUI.RPC.Response.Types;
using AllArt.SUI.Wallets;
using Chaos.NaCl;
using Newtonsoft.Json;
using SimpleScreen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmSendScreen : BaseScreen {

    public TextMeshProUGUI to;
    public TextMeshProUGUI fee;
    public TextMeshProUGUI amount;

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
        GoTo("TransactionDone");
    }

    /// <summary>
    /// Sends a payment in SUI currency.
    /// </summary>
    /// <returns>A task that represents the asynchronous payment operation.</returns>
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
        query.filter = filter;

        ObjectDataOptions options = new();
        query.options = options;
        ulong amount = (ulong)(float.Parse(TransferData.amount) * Mathf.Pow(10, TransferData.coin.decimals));

        loaderScreen.gameObject.SetActive(true);
        try {
            var res = await WalletComponent.Instance.GetOwnedObjects(wallet.publicKey, query, null, 3);

            if(res == null || res.data.Count == 0)
            {
                InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Transaction failed");
                return;
            }

            List<string> objects = new();

            foreach(var data in res.data)
            {
                objects.Add(data.data.objectId);
            }

            Debug.Log(TransferData.amount);
            var res_pay = await WalletComponent.Instance.PaySui(wallet, objects, new List<string>() {TransferData.to},
                new List<string>() {amount.ToString()}, "1000000000");

            var signature = wallet.SignData(Wallet.GetMessageWithIntent(CryptoBytes.FromBase64String(res_pay.txBytes)));

            var transaction = await WalletComponent.Instance.client.ExecuteTransactionBlock(res_pay.txBytes,
                new string[] { signature }, new ObjectDataOptions(), ExecuteTransactionRequestType.WaitForEffectsCert);

            Debug.Log(JsonConvert.SerializeObject(transaction));

            if(transaction.error != null && transaction.error.code != 0)
            {
                Debug.Log(transaction.error.message);
                InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Transaction failed: " + transaction.error.message);
                return;
            }

        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Transaction failed");
        }

        
        loaderScreen.gameObject.SetActive(false);
        GoTo("TransactionDone");
    }

    /// <summary>
    /// Sends a payment in a currency other tokens.
    /// </summary>
    /// <returns>A task that represents the asynchronous payment operation.</returns>
    private async System.Threading.Tasks.Task PayOtherCurrency()
    {
        var wallet = WalletComponent.Instance.currentWallet;

        string type = WalletComponent.Instance.coinMetadatas.FirstOrDefault(x => x.Value == WalletComponent.Instance.currentCoinMetadata).Key;
        Page_for_SuiObjectResponse_and_ObjectID ownedCoins = await WalletComponent.Instance.GetOwnedObjectsOfType(wallet, type);

        if (ownedCoins == null || ownedCoins.data.Count == 0)
        {
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Transaction failed");
            return;
        }

        List<string> ownedCoinObjectIds = new();

        foreach (var data in ownedCoins.data)
        {
            ownedCoinObjectIds.Add(data.data.objectId);
        }

        ulong amount = (ulong)(float.Parse(TransferData.amount) * Mathf.Pow(10, TransferData.coin.decimals));
        var res_pay = await WalletComponent.Instance.Pay(wallet,
            ownedCoinObjectIds.ToArray(),
            new string[] { TransferData.to },
            new string[] { amount.ToString() },
            null,
            "100000000");

        if (res_pay == null || res_pay.txBytes == null)
        {
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Transaction failed");
            loaderScreen.gameObject.SetActive(false);
            return;
        }
        Debug.Log(JsonConvert.SerializeObject(res_pay));
        var signature = wallet.SignData(Wallet.GetMessageWithIntent(CryptoBytes.FromBase64String(res_pay.txBytes)));

        var transaction = await WalletComponent.Instance.client.ExecuteTransactionBlock(res_pay.txBytes,
            new string[] { signature }, new ObjectDataOptions(), ExecuteTransactionRequestType.WaitForEffectsCert);

        if(transaction.error != null && transaction.error.code != 0)
        {
            Debug.Log(transaction.error.message);
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Transaction failed: " + transaction.error.message);
            return;
        }
        loaderScreen.gameObject.SetActive(false);
        GoTo("TransactionDone");
    }

    public override async void ShowScreen(object data = null)
    {
        base.ShowScreen(data);

        var confirmSendData = data as TransferData;
        Debug.Log(confirmSendData.to);

        string feeAmount = await WalletComponent.Instance.GetReferenceGasPrice();
        float feeAmountFloat = float.Parse(feeAmount) / Mathf.Pow(10, 9);
        if (confirmSendData != null)
        {
            TransferData = confirmSendData;
            to.text = confirmSendData.to;
            amount.text = $"{confirmSendData.amount} {confirmSendData.coin.symbol}";
            fee.text = $"{feeAmountFloat.ToString("0.#########")} SUI";
            return;
        }
    }


}
