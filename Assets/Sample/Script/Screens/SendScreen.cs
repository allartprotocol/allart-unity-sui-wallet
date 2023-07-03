using SimpleScreen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SendScreen : BaseScreen
{
    public TextMeshProUGUI tokenName;

    public TMP_InputField to;
    public TMP_InputField amount;

    public Button continueBtn;
    public Button tokenSelectBtn;

    public Button max;

    Balance balance;

    private void Start()
    {
        continueBtn.onClick.AddListener(OnContinue);
        tokenSelectBtn.onClick.AddListener(OnTokenSelect);
        max.onClick.AddListener(OnMax);
    }

    private void OnMax()
    {
        amount.text = balance.totalBalance.ToString();
    }

    private void OnTokenSelect()
    {
        GoTo("TokenSelectScreen");
    }

    public override async void ShowScreen(object data = null)
    {
        base.ShowScreen(data);
        amount.text = "0";
        tokenName.text = WalletComponent.Instance.currentCoinMetadata.name;
        var coinType = WalletComponent.Instance.coinMetadatas.Where(x => x.Value.symbol == WalletComponent.Instance.currentCoinMetadata.symbol).FirstOrDefault().Key;
        balance = await WalletComponent.Instance.GetBalance(WalletComponent.Instance.currentWallet.publicKey, 
            coinType);
    }

    private void OnContinue()
    {
        GoTo("SendConfirmScreen", new TransferData()
        {
            to = to.text,
            amount = amount.text,
            coin = WalletComponent.Instance.currentCoinMetadata
        });
    }
}
