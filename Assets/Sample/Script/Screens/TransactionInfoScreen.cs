using System;
using System.Collections;
using System.Collections.Generic;
using AllArt.SUI.RPC.Response;
using Newtonsoft.Json;
using SimpleScreen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TransactionInfoScreen : BaseScreen {

    SuiTransactionBlockResponse suiTransactionBlockResponse;

    public TextMeshProUGUI date;
    public TextMeshProUGUI status;
    public TextMeshProUGUI sender;
    public TextMeshProUGUI network;
    public TextMeshProUGUI fee;

    public TextMeshProUGUI type;
    public TextMeshProUGUI balanceChange;

    public Sprite failImage;
    public Sprite successImage;

    public UnityEngine.UI.Image statusImage;

    public Button suiExplorerBtn;

    private void Start() {
        suiExplorerBtn.onClick.AddListener(OnSuiExplorer);
    }

    string GetBalanceChange() {
        long balanceChange = 0;
        string type = "";
        foreach (var effect in suiTransactionBlockResponse.balanceChanges)
        {
            Debug.Log(JsonConvert.SerializeObject(effect));
            if(effect.owner.AddressOwner == WalletComponent.Instance.currentWallet.publicKey)
            {
                Debug.Log(effect.amount);
                balanceChange += long.Parse(effect.amount);
                type = effect.coinType;
            }
        }
        
        string change = "0 SUI";

        CoinMetadata coinMetadata = null;
        if(WalletComponent.Instance.coinMetadatas.ContainsKey(type))
            coinMetadata = WalletComponent.Instance.coinMetadatas[type];

        if(coinMetadata == null)
            return change;
        
        float decimalChange = WalletComponent.ApplyDecimals((long)balanceChange, coinMetadata);

        if(balanceChange == 0)
            return change;

        if(balanceChange > 0)
            change = "+" + decimalChange.ToString("0.############") + " SUI";
        else if(balanceChange < 0)
            change = decimalChange.ToString("0.############") + " SUI";

        return change;
    }

    private void OnSuiExplorer()
    {
        string network = WalletComponent.Instance.nodeType switch
        {
            ENodeType.MainNet => "mainnet",
            ENodeType.TestNet => "testnet",
            _ => "devnet",
        };

        //open browser with the transaction hash
        Application.OpenURL($"https://suiexplorer.com/txblock/{suiTransactionBlockResponse.digest}?network={network}");
    }

    public override void ShowScreen(object data) {
        base.ShowScreen(data);
        Debug.Log(data.GetType());

        suiTransactionBlockResponse = data as SuiTransactionBlockResponse;

        if(suiTransactionBlockResponse.effects.status.status == "success")
            statusImage.sprite = successImage;
        else
            statusImage.sprite = failImage;

        DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeMilliseconds((long)ulong.Parse(suiTransactionBlockResponse.timestampMs));
        date.text = dateTime.ToString("MMMM d, yyyy 'at' h:mm tt");

        if(suiTransactionBlockResponse.transaction.data.sender == WalletComponent.Instance.currentWallet.publicKey)
            type.text = "Transaction";
        else
            type.text = "Receive";

        var gasUsed = suiTransactionBlockResponse.effects.gasUsed;

        Debug.Log(JsonUtility.ToJson(gasUsed));
        float gasUsedFloat = 0;
        if(gasUsed != null && gasUsed != default ){
            if(gasUsed.computationCost != null)
                gasUsedFloat += float.Parse(gasUsed.computationCost);
            if(gasUsed.storageCost != null)
                gasUsedFloat += float.Parse(gasUsed.storageCost);
            if(gasUsed.storageRebate != null)
                gasUsedFloat -= float.Parse(gasUsed.storageRebate);
            if(gasUsed.nonRefundableStorageFees != null)
                gasUsedFloat += float.Parse(gasUsed.nonRefundableStorageFees);
        }
        gasUsedFloat += float.Parse(suiTransactionBlockResponse.transaction.data.gasData.price);

        status.text = suiTransactionBlockResponse.effects.status.status;
        sender.text = suiTransactionBlockResponse.transaction.data.sender;
        network.text = "SUI";
        var feeText = (gasUsedFloat / Mathf.Pow(10,9)).ToString("0.############");
        fee.text = $"~{feeText} SUI";
        try{
            balanceChange.text = GetBalanceChange();
        }
        catch(Exception e){
            Debug.LogError(e);
        }
    }

}
