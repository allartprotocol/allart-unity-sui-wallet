using SimpleScreen;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RemoveWalletScreen : BaseScreen
{
    public TextMeshProUGUI walletName;
    public Button removeWallets;
    public Wallet wallet;

    private void Start()
    {
        removeWallets.onClick.AddListener(OnRemoveWallets);
    }

    public override void ShowScreen(object data = null)
    {
        base.ShowScreen(data);
        if (data == null) return;
        wallet = (Wallet)data;
        walletName.text = wallet.publicKey;
    }

    private void OnRemoveWallets()
    {
        WalletComponent.Instance.RemoveWallet(wallet);
        GoTo("WalletsListScreen");
    }
}
