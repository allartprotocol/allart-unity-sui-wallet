using System;
using AllArt.SUI.RPC.Response;
using Newtonsoft.Json;
using SimpleScreen;
using UnityEngine;
using UnityEngine.UI;

public class EventObject : MonoBehaviour
{
    SimpleScreenManager screenManager;

    public Sprite successImage;
    public Sprite failImage;

    public UnityEngine.UI.Image statusImage;

    public SuiTransactionBlockResponse eventPage;
    
    public TMPro.TextMeshProUGUI nameTxt;
    public TMPro.TextMeshProUGUI amount;
    public TMPro.TextMeshProUGUI date;

    private Button transactionButton;

    public void InitializeObject(SuiTransactionBlockResponse eventPage, SimpleScreenManager screenManager)
    {
        nameTxt.text = "Transaction";
        amount.text = "";
        date.text = "";

        this.screenManager = screenManager;
        transactionButton = GetComponent<Button>();
        this.eventPage = eventPage;
        transactionButton.onClick.AddListener(() =>
        {
            this.screenManager.ShowScreen("TransactionInfo", this.eventPage);
        });

        if(eventPage.transaction.data.sender == WalletComponent.Instance.currentWallet.publicKey)
        {
            nameTxt.text = "Sent";
            if(GetReceiver() == "Unknown Receiver")
                amount.text = "To " + GetReceiver();
            else
                amount.text = "To " + WalletUtility.ShortenAddress(GetReceiver());
        }
        else{
            nameTxt.text = "Received";
            amount.text = "From " + WalletUtility.ShortenAddress(eventPage.transaction.data.sender);
        }

        if(eventPage.transaction.data.transaction.kind == null)
            date.text = "Unknown";
        else
            date.text = eventPage.transaction.data.transaction.kind;

        DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeMilliseconds((long)ulong.Parse(eventPage.timestampMs));
        date.text = dateTime.ToString("dd/MM/yyyy hh:mm:ss");

        if(eventPage.effects.status.status == "success")
            statusImage.sprite = successImage;
        else
            statusImage.sprite = failImage;
    }

    private string GetReceiver()
    {
        foreach (var input in eventPage.transaction.data.transaction.inputs)
        {
            if(input.valueType == "address")
            {
                return (string)input.value;
            }
        }
        return "Unknown Receiver";
    }

    private void OnDestroy()
    {
        transactionButton?.onClick.RemoveAllListeners();
    }

}
