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
        this.screenManager = screenManager;
        transactionButton = GetComponent<Button>();
        this.eventPage = eventPage;
        transactionButton.onClick.AddListener(() =>
        {
            this.screenManager.ShowScreen("TransactionInfo", this.eventPage);
        });

        if(eventPage.transaction.data.sender == WalletComponent.Instance.currentWallet.publicKey)
            nameTxt.text = "Sent";
        else
            nameTxt.text = "Transaction";

        if(eventPage.transaction.data.transaction.kind == null)
            date.text = "Unknown";
        else
            date.text = eventPage.transaction.data.transaction.kind;

        DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeMilliseconds((long)ulong.Parse(eventPage.timestampMs));
        date.text = dateTime.ToString("MMMM d, yyyy 'at' h:mm tt");

        amount.text = eventPage.digest;

        if(eventPage.effects.status.status == "success")
            statusImage.sprite = successImage;
        else
            statusImage.sprite = failImage;
    }

    private void OnDestroy()
    {
        transactionButton.onClick.RemoveAllListeners();
    }

}
