using AllArt.SUI.Wallets;
using SimpleScreen;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WalletSettingsScreen : BaseScreen
{
    public Button exportPhrase;
    public Button exportPrivate;
    public Button removeWallets;
    public CopyButton copyButton;

    public TextMeshProUGUI walletPubKey;
    private Wallet wallet;

    private void Start()
    {
        exportPhrase.onClick.AddListener(OnExportPhrase);
        exportPrivate.onClick.AddListener(OnExportPrivate);
        removeWallets.onClick.AddListener(OnRemoveWallets);
    }

    public override void ShowScreen(object data = null)
    {
        base.ShowScreen(data);
        if(data == null)
        {
            return;
        }
        wallet = data as Wallet;
        walletPubKey.text = wallet.displayAddress;

        exportPhrase.gameObject.SetActive(!string.IsNullOrEmpty(WalletComponent.Instance.currentWallet.mnemonic));

        copyButton.SetText(wallet.publicKey, wallet.displayAddress);
    }

    private void OnCopy()
    {
        GUIUtility.systemCopyBuffer = wallet.publicKey;
    }

    private void OnRemoveWallets()
    {
        GoTo("RemoveWalletScreen", wallet);
    }


    private void OnExportPrivate()
    {
        GoTo("ExportPrivate", wallet);
    }

    private void OnExportPhrase()
    {
        GoTo("ExportPhrase", wallet);

    }
}
