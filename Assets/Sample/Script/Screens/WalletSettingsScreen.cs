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
        wallet = data as Wallet;
        walletPubKey.text = wallet.publicKey;
    }

    private void OnRemoveWallets()
    {
        manager.ShowScreen("RemoveWalletsScreen", wallet);
    }


    private void OnExportPrivate()
    {
        manager.ShowScreen("ExportPrivate", wallet);
    }

    private void OnExportPhrase()
    {
        manager.ShowScreen("ExportPhrase", wallet);

    }
}
