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
    public TextMeshProUGUI toHeader;
    public TextMeshProUGUI fee;
    public TextMeshProUGUI amount;

    public TokenImage tokenImage;

    public Button confirmBtn;
    public Button cancelBtn;
    private TransferData TransferData;

    public Transform loaderScreen;

    private void Start()
    {
        confirmBtn.onClick.AddListener(OnConfirm);
        cancelBtn.onClick.AddListener(OnCancel);
    }

    private void OnCancel()
    {
        GoTo("MainScreen");
    }

    private async void OnConfirm()
    {
        LoaderScreen.instance.ShowLoading("Sending");
        JsonRpcResponse<SuiTransactionBlockResponse> completedTransaction = null;
        try{
            if(WalletComponent.Instance.currentCoinMetadata.symbol == "SUI")
            {
                if(TransferData.transferAll)
                    completedTransaction = await PayAllSui();
                else
                    completedTransaction = await PaySui();
            }
            else
            {
                completedTransaction = await PayOtherCurrency();
            }
        }
        catch(Exception e)
        {
            Debug.Log(e);
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Transaction was not successful ");
        }
        
        LoaderScreen.instance.HideLoading();

        TransferData finalData = TransferData;
        finalData.response = completedTransaction;
        GoTo("TransactionDone", TransferData);
    }

    /// <summary>
    /// Sends a payment in SUI currency.
    /// </summary>
    /// <returns>A task that represents the asynchronous payment operation.</returns>
    private async System.Threading.Tasks.Task<JsonRpcResponse<SuiTransactionBlockResponse>> PaySui()
    {
        JsonRpcResponse<SuiTransactionBlockResponse> transaction = null;
        try {
            var res_pay = await CreatePaySuiTransaction();

            if(res_pay == null || res_pay.result == null)
            {
                string msg = res_pay != null ? res_pay.error.message : "Transaction failed";
                InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, msg);
            }

            if(res_pay.error != null)
            {
                InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Transaction failed: " + res_pay.error.message);
            }

            var signature = WalletComponent.Instance.currentWallet.SignData(Wallet.GetMessageWithIntent(CryptoBytes.FromBase64String(res_pay.result.txBytes)));

            transaction = await WalletComponent.Instance.client.ExecuteTransactionBlock(res_pay.result.txBytes,
                new string[] { signature }, new TransactionBlockResponseOptions(), ExecuteTransactionRequestType.WaitForLocalExecution);

            if(transaction.error != null && transaction.error.code != 0)
            {
                InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Transaction failed: " + transaction.error.message);
            }
        }
        catch(Exception e)
        {
            Debug.Log(e);
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Transaction failed");
        }

        return transaction;
    }

    private async System.Threading.Tasks.Task<JsonRpcResponse<SuiTransactionBlockResponse>> PayAllSui()
    {        
        JsonRpcResponse<SuiTransactionBlockResponse> transaction = null;
        try {

            JsonRpcResponse<TransactionBlockBytes> res_pay = await CreatePayAllSuiTransaction();

            if(res_pay == null || res_pay.result == null)
            {
                string msg = res_pay != null ? res_pay.error.message : "Transaction failed";
                InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, msg);
            }

            if(res_pay.error != null)
            {
                InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Transaction failed: " + res_pay.error.message);
            }

            var signature = WalletComponent.Instance.currentWallet.SignData(Wallet.GetMessageWithIntent(CryptoBytes.FromBase64String(res_pay.result.txBytes)));

            transaction = await WalletComponent.Instance.client.ExecuteTransactionBlock(res_pay.result.txBytes,
                new string[] { signature }, new TransactionBlockResponseOptions(), ExecuteTransactionRequestType.WaitForLocalExecution);

            if(transaction.error != null && transaction.error.code != 0)
            {
                InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Transaction failed: " + transaction.error.message);
            }
        }
        catch(Exception e)
        {
            Debug.Log(e);
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Transaction failed");
        }

        
        // loaderScreen.gameObject.SetActive(false);
        LoaderScreen.instance.HideLoading();
        return transaction;
    }

    public async Task<JsonRpcResponse<TransactionBlockBytes>> CreatePaySuiTransaction(){

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
        ulong amount = (ulong)(decimal.Parse(TransferData.amount) * (decimal)Mathf.Pow(10, TransferData.coin.decimals));
        try {
            var res = await WalletComponent.Instance.GetOwnedObjects(wallet.publicKey, query, null, 3);

            if(res == null || res.data.Count == 0)
            {
                return null;
            }

            List<string> objects = new();

            foreach(var data in res.data)
            {
                objects.Add(data.data.objectId);
            }

            var res_pay = await WalletComponent.Instance.PaySui(wallet, objects, new List<string>() {TransferData.to},
                new List<string>() {amount.ToString()}, SUIConstantVars.GAS_BUDGET);
            return res_pay;

        }
        catch(Exception e)
        {
            return null;
        }
    }

    public async Task<JsonRpcResponse<TransactionBlockBytes>> CreatePayAllSuiTransaction(){

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

        try {
            var res = await WalletComponent.Instance.GetOwnedObjects(wallet.publicKey, query, null, 3);

            if(res == null || res.data.Count == 0)
            {
                return null;
            }

            List<string> objects = new();

            foreach(var data in res.data)
            {
                objects.Add(data.data.objectId);
            }

            var res_pay = await WalletComponent.Instance.PayAllSui(wallet, objects, TransferData.to, SUIConstantVars.GAS_BUDGET);
            return res_pay;
        }
        catch(Exception e)
        {
            return null;
        }
    }

    /// <summary>
    /// Sends a payment in a currency other tokens.
    /// </summary>
    /// <returns>A task that represents the asynchronous payment operation.</returns>
    private async System.Threading.Tasks.Task<JsonRpcResponse<SuiTransactionBlockResponse>> PayOtherCurrency()
    {
        var res_pay = await CreatePayOtherCurrenciesTransaction();

        var signature = WalletComponent.Instance.currentWallet.SignData(Wallet.GetMessageWithIntent(CryptoBytes.FromBase64String(res_pay.result.txBytes)));

        JsonRpcResponse<SuiTransactionBlockResponse> transaction = await WalletComponent.Instance.client.ExecuteTransactionBlock(res_pay.result.txBytes,
            new string[] { signature }, new TransactionBlockResponseOptions(), ExecuteTransactionRequestType.WaitForLocalExecution);

        return transaction;
    }

    public async Task<JsonRpcResponse<TransactionBlockBytes>> CreatePayOtherCurrenciesTransaction(){
            
            var wallet = WalletComponent.Instance.currentWallet;
    
            string type = WalletComponent.Instance.coinMetadatas.FirstOrDefault(x => x.Value == WalletComponent.Instance.currentCoinMetadata).Key;
            Debug.Log(type);
            Page_for_SuiObjectResponse_and_ObjectID ownedCoins = await WalletComponent.Instance.GetOwnedObjectsOfType(wallet, type);
    
            if (ownedCoins == null || ownedCoins.data.Count == 0)
            {
                return null;
            }
    
            List<string> ownedCoinObjectIds = new();
    
            foreach (var data in ownedCoins.data)
            {
                ownedCoinObjectIds.Add(data.data.objectId);
            }
    
            Debug.Log(JsonConvert.SerializeObject(ownedCoinObjectIds));
            ulong amount = (ulong)(decimal.Parse(TransferData.amount) * (decimal)Mathf.Pow(10, TransferData.coin.decimals));
            JsonRpcResponse<TransactionBlockBytes> res_pay = null;
            Debug.Log(amount);
            try{
                res_pay = await WalletComponent.Instance.Pay(wallet,
                    ownedCoinObjectIds.ToArray(),
                    new string[] { TransferData.to },
                    new string[] { amount.ToString() },
                    null,
                    SUIConstantVars.GAS_BUDGET);
            }
            catch (Exception e){
                Debug.Log(e);
                return null;
            }

            return res_pay;
    }

    public override async void ShowScreen(object data = null)
    {
        base.ShowScreen(data);

        LoaderScreen.instance.ShowLoading("Running simulation");
        confirmBtn.interactable = false;

        string feeAmount = await WalletComponent.Instance.GetReferenceGasPrice();
        decimal feeAmountFloat = decimal.Parse(feeAmount) / (decimal)Mathf.Pow(10, 9);

        if (data is TransferData confirmSendData)
        {
            TransferData = confirmSendData;
            to.text = Wallet.DisplaySuiAddress(confirmSendData.to);
            toHeader.text = $"To {Wallet.DisplaySuiAddress(confirmSendData.to)}";
            amount.text = $"-{WalletUtility.ParseDecimalValueToString(WalletUtility.ParseDecimalValueFromString(confirmSendData.amount), true)} {confirmSendData.coin.symbol}";
            fee.text = $"{feeAmountFloat:0.#########} SUI";
            Sprite icon = WalletComponent.Instance.GetCoinImage(confirmSendData.coin.symbol);
            tokenImage.Init(icon, confirmSendData.coin.symbol);
        }

        SuiTransactionBlockResponse res = null;
        try{
            res = await TryRunTransaction(TransferData.transferAll);
            if(res != null){
                decimal gasUsed = CalculateGasUsed(res);
                decimal gasUsedInSUI = gasUsed / (decimal)Mathf.Pow(10, 9);
                fee.text = $"{gasUsedInSUI:0.#########} SUI";
            }
        }
        catch(Exception e){
            Debug.Log(e);
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Transaction simulation failed");
        }

        confirmBtn.interactable = res != null;  

        LoaderScreen.instance.HideLoading();    
    }

    private decimal CalculateGasUsed(SuiTransactionBlockResponse suiTransactionBlockResponse)
    {
        var gasUsed = suiTransactionBlockResponse.effects.gasUsed;
        decimal gasUsedFloat = 0;
        if (gasUsed != null && gasUsed != default)
        {
            if (gasUsed.computationCost != null)
                gasUsedFloat += decimal.Parse(gasUsed.computationCost);
            if (gasUsed.storageCost != null)
                gasUsedFloat += decimal.Parse(gasUsed.storageCost);
            if (gasUsed.storageRebate != null)
                gasUsedFloat -= decimal.Parse(gasUsed.storageRebate);
            if (gasUsed.nonRefundableStorageFee != null)
                gasUsedFloat += decimal.Parse(gasUsed.nonRefundableStorageFee);
        }
        return gasUsedFloat;
    }

    async  Task<SuiTransactionBlockResponse> TryRunTransaction(bool sendMax = false){
        JsonRpcResponse<TransactionBlockBytes> transaction;
        if (WalletComponent.Instance.currentCoinMetadata.symbol == "SUI"){
            if(sendMax)
                transaction = await CreatePayAllSuiTransaction();
            else
                transaction = await CreatePaySuiTransaction();
        }
        else{
            Debug.Log("pay other currency");
            transaction = await CreatePayOtherCurrenciesTransaction();
        }

        Debug.Log(JsonConvert.SerializeObject(transaction));

        if(transaction.error != null)
        {
            //{"jsonrpc":"2.0","id":1,"error":{"code":-32000,"message":"Cannot find gas coin for signer address [0x3ca4fe7e572b3703d8d3d2109102ae7f162f868a9584418265f9e89cb918eb35] with amount sufficient for the required gas amount [20000000]."},"result":null}
           
            if(transaction.error.code == -32000 && transaction.error.message.Contains("Cannot find gas coin for signer address"))
            {
                InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Insufficient SUI to cover transaction fee");
            }
            else
            {
                InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Transaction simulation failed");
            }
            return null;
        }

        if(transaction == null)
        {
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Transaction failed");
            return null;
        }
        string transactionBytes = transaction.result.txBytes;

        var res = await WalletComponent.Instance.DryRunTransaction(transactionBytes);
        Debug.Log(JsonConvert.SerializeObject(res));
        if(res == null || res.error != null || res.result.effects.status.status == "failure")
        {
            string msg;
            if (res != null && res.error != null)
            {
                msg = ParseErrorMessage(res.error.message);
            }
            else if(res != null && res.result != null && res.result.effects != null && res.result.effects.status.error != null){
                string error = res.result.effects.status.error;
                msg = ParseErrorMessage(error);
            }
            else
                msg = "Transaction simulation failed";
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, msg);
            return null;
        }

        return res.result;
    }
    private string ParseErrorMessage(string msg, string defaultMessage = "Transaction failed"){
        if(msg.Contains("InsufficientCoinBalance") || msg.Contains("GasBalanceTooLow")){
            return "Insufficient SUI to cover transaction fee";
        }
        else{
            return defaultMessage;
        }
    }
}

