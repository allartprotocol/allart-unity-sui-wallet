using System;
using System.Collections;
using System.Collections.Generic;
using AllArt.SUI.RPC.Response.Types;
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

    public override void ShowScreen(object data){
        base.ShowScreen(data);

        suiTransactionBlockResponse = (SuiTransactionBlockResponse)data;
        Debug.Log(JsonUtility.ToJson(data));

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

        status.text = suiTransactionBlockResponse.effects.status.status;
        sender.text = suiTransactionBlockResponse.transaction.data.sender;
        network.text = "SUI";
        var feeText = (float.Parse(suiTransactionBlockResponse.transaction.data.gasData.price) / Mathf.Pow(10,9)).ToString("0.############");
        Debug.Log(float.Parse(suiTransactionBlockResponse.transaction.data.gasData.price));
        fee.text = $"~{feeText} SUI";
        try{
            balanceChange.text = GetBalanceChange();
        }
        catch(Exception e){
            Debug.LogError(e);
        }
    }

}
