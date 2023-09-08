using AllArt.SUI.Wallets;
using SimpleScreen;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WalletListObject : MonoBehaviour
{
    public Wallet wallet;
    public string walletName;

    public TextMeshProUGUI walletNameTxt;
    public TextMeshProUGUI walletAddressTxt;

    public Button button;
    private SimpleScreenManager manager;

    public void SetWallet(int index, Wallet wallet, string walletName, SimpleScreenManager manager)
    {
        this.manager = manager;
        this.wallet = wallet;
        this.walletName = walletName;
        walletNameTxt.text = $"Wallet {index}";;
        walletAddressTxt.text = wallet.displayAddress;
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        WalletComponent.Instance.SetCurrentWallet(wallet);
        manager.ShowScreen("WalletSettings", wallet);
    }
}
