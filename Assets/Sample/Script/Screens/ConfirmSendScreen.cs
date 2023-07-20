using AllArt.SUI.RPC.Filter.Types;
using AllArt.SUI.RPC.Response;
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
        bool done = false;
        if(WalletComponent.Instance.currentCoinMetadata.symbol == "SUI")
        {
            done = await PaySui();
        }
        else
        {
            done = await PayOtherCurrency();
        }
        GoTo("TransactionDone", done);
    }

    /// <summary>
    /// Sends a payment in SUI currency.
    /// </summary>
    /// <returns>A task that represents the asynchronous payment operation.</returns>
    private async System.Threading.Tasks.Task<bool> PaySui()
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
                return false;
            }

            List<string> objects = new();

            foreach(var data in res.data)
            {
                objects.Add(data.data.objectId);
            }

            var res_pay = await WalletComponent.Instance.PaySui(wallet, objects, new List<string>() {TransferData.to},
                new List<string>() {amount.ToString()}, "100000000");

            if(res_pay == null || res_pay.result == null)
            {
                string msg = res_pay != null ? res_pay.error.message : "Transaction failed";
                InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, msg);
                loaderScreen.gameObject.SetActive(false);
                return false;
            }

            if(res_pay.error != null)
            {
                InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Transaction failed: " + res_pay.error.message);
                loaderScreen.gameObject.SetActive(false);
                return false;
            }

            var signature = wallet.SignData(Wallet.GetMessageWithIntent(CryptoBytes.FromBase64String(res_pay.result.txBytes)));

            var transaction = await WalletComponent.Instance.client.ExecuteTransactionBlock(res_pay.result.txBytes,
                new string[] { signature }, new ObjectDataOptions(), ExecuteTransactionRequestType.WaitForEffectsCert);

            if(transaction.error != null && transaction.error.code != 0)
            {
                InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Transaction failed: " + transaction.error.message);
                loaderScreen.gameObject.SetActive(false);
                return false;
            }
        }
        catch(Exception e)
        {
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Transaction failed");
        }

        
        loaderScreen.gameObject.SetActive(false);
        return true;
    }

    public async Task<string> CreatePaySuiTransaction(){

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

        try {
            var res = await WalletComponent.Instance.GetOwnedObjects(wallet.publicKey, query, null, 3);

            if(res == null || res.data.Count == 0)
            {
                InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Transaction failed");
                return "";
            }

            List<string> objects = new();

            foreach(var data in res.data)
            {
                objects.Add(data.data.objectId);
            }

            var res_pay = await WalletComponent.Instance.PaySui(wallet, objects, new List<string>() {TransferData.to},
                new List<string>() {amount.ToString()}, "100000000");
            return res_pay.result.txBytes;

        }
        catch(Exception e)
        {
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Transaction failed");
            return "";
        }
    }

    /// <summary>
    /// Sends a payment in a currency other tokens.
    /// </summary>
    /// <returns>A task that represents the asynchronous payment operation.</returns>
    private async System.Threading.Tasks.Task<bool> PayOtherCurrency()
    {
        var wallet = WalletComponent.Instance.currentWallet;

        string type = WalletComponent.Instance.coinMetadatas.FirstOrDefault(x => x.Value == WalletComponent.Instance.currentCoinMetadata).Key;
        Page_for_SuiObjectResponse_and_ObjectID ownedCoins = await WalletComponent.Instance.GetOwnedObjectsOfType(wallet, type);

        if (ownedCoins == null || ownedCoins.data.Count == 0)
        {
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Transaction failed");
            return false;
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

        if (res_pay == null || res_pay.result == null)
        {
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Transaction failed");
            loaderScreen.gameObject.SetActive(false);
            return false;
        }

        if(res_pay.error != null)
        {
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Transaction failed: " + res_pay.error.message);
            return false;
        }

        var signature = wallet.SignData(Wallet.GetMessageWithIntent(CryptoBytes.FromBase64String(res_pay.result.txBytes)));

        var transaction = await WalletComponent.Instance.client.ExecuteTransactionBlock(res_pay.result.txBytes,
            new string[] { signature }, new ObjectDataOptions(), ExecuteTransactionRequestType.WaitForEffectsCert);

        if(transaction.error != null && transaction.error.code != 0)
        {
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Transaction failed: " + transaction.error.message);
            return false;
        }
        loaderScreen.gameObject.SetActive(false);
        return true;
    }

    public async Task<string> CreatePayOtherCurrenciesTransaction(){
            
            var wallet = WalletComponent.Instance.currentWallet;
    
            string type = WalletComponent.Instance.coinMetadatas.FirstOrDefault(x => x.Value == WalletComponent.Instance.currentCoinMetadata).Key;
            Page_for_SuiObjectResponse_and_ObjectID ownedCoins = await WalletComponent.Instance.GetOwnedObjectsOfType(wallet, type);
    
            if (ownedCoins == null || ownedCoins.data.Count == 0)
            {
                InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Transaction failed");
                return "";
            }
    
            List<string> ownedCoinObjectIds = new();
    
            foreach (var data in ownedCoins.data)
            {
                ownedCoinObjectIds.Add(data.data.objectId);
            }
    
            ulong amount = (ulong)(float.Parse(TransferData.amount) * Mathf.Pow(10, TransferData.coin.decimals));
            JsonRpcResponse<TransactionBlockBytes> res_pay = null;
            try{
                res_pay = await WalletComponent.Instance.Pay(wallet,
                    ownedCoinObjectIds.ToArray(),
                    new string[] { TransferData.to },
                    new string[] { amount.ToString() },
                    null,
                    "100000000");
            }
            catch (Exception e){
                InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Transaction failed");
                return "";
            }

            return res_pay != null ? res_pay.result.txBytes : "";
    }

    public override async void ShowScreen(object data = null)
    {
        base.ShowScreen(data);
        confirmBtn.interactable = false;
        string feeAmount = await WalletComponent.Instance.GetReferenceGasPrice();
        float feeAmountFloat = float.Parse(feeAmount) / Mathf.Pow(10, 9);
        if (data is TransferData confirmSendData)
        {
            TransferData = confirmSendData;
            to.text = confirmSendData.to;
            amount.text = $"{confirmSendData.amount} {confirmSendData.coin.symbol}";
            fee.text = $"{feeAmountFloat.ToString("0.#########")} SUI";
        }
        var res = await TryRunTransaction();

        if(res != null){
            float gasUsed = CalculateGasUsed(res);
            float gasUsedInSUI = gasUsed / Mathf.Pow(10, 9);
            fee.text = $"{gasUsedInSUI.ToString("0.#########")} SUI";
        }
        confirmBtn.interactable = res != null;
        
    }

    private float CalculateGasUsed(SuiTransactionBlockResponse suiTransactionBlockResponse)
    {
        var gasUsed = suiTransactionBlockResponse.effects.gasUsed;
        float gasUsedFloat = 0;
        if (gasUsed != null && gasUsed != default)
        {
            if (gasUsed.computationCost != null)
                gasUsedFloat += float.Parse(gasUsed.computationCost);
            if (gasUsed.storageCost != null)
                gasUsedFloat += float.Parse(gasUsed.storageCost);
            if (gasUsed.storageRebate != null)
                gasUsedFloat -= float.Parse(gasUsed.storageRebate);
            if (gasUsed.nonRefundableStorageFees != null)
                gasUsedFloat += float.Parse(gasUsed.nonRefundableStorageFees);
        }
        return gasUsedFloat;
    }

    async  Task<SuiTransactionBlockResponse> TryRunTransaction(){
        string transactionBytes = "";

        if(WalletComponent.Instance.currentCoinMetadata.symbol == "SUI"){
            transactionBytes = await CreatePaySuiTransaction();
        }
        else{
            transactionBytes = await CreatePayOtherCurrenciesTransaction();
        }

        var res = await WalletComponent.Instance.DryRunTransaction(transactionBytes);
        if(res == null || res.error != null || res.result.effects.status.status == "failure")
        {
            string msg = "";
            if(res != null && res.error != null)
                msg = res.error.message;
            else if(res != null && res.result != null && res.result.effects != null && res.result.effects.status != null)
                msg = "Transaction failed";
            else
                msg = "Transaction failed";
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, msg);
            return null;
        }

        return res.result;
    }
}
