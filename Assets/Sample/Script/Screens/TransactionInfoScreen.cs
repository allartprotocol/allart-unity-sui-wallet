using System;
using System.Collections;
using System.Collections.Generic;
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
        float balanceChange = 0;

        foreach (var effect in suiTransactionBlockResponse.balanceChanges)
        {
            if(effect.owner.AddressOwner == WalletComponent.Instance.currentWallet.publicKey)
                balanceChange += effect.ammount;
        }
        
        string change = "0 SUI";

        if(balanceChange > 0)
            change = "+" + balanceChange + " SUI";
        else if(balanceChange < 0)
            change = balanceChange + " SUI";

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
        date.text = dateTime.ToString("dd/MM/yyyy HH:mm:ss");

        if(suiTransactionBlockResponse.transaction.data.sender == WalletComponent.Instance.currentWallet.publicKey)
            type.text = "Transaction";
        else
            type.text = "Receive";

        status.text = suiTransactionBlockResponse.effects.status.status;
        sender.text = suiTransactionBlockResponse.transaction.data.sender;
        network.text = "SUI";
        fee.text = (float.Parse(suiTransactionBlockResponse.transaction.data.gasData.price) / Mathf.Pow(10,9)).ToString() + " SUI";
        balanceChange.text = GetBalanceChange();
    }

}
