using AllArt.SUI.Wallets;
using SimpleScreen;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        walletName.text = wallet.displayAddress;
    }

    private void OnRemoveWallets()
    {
        WalletComponent.Instance.RemoveWallet(wallet);
        if(WalletComponent.Instance.GetAllWallets().Count == 0)
        {
            WalletComponent.Instance.RemoveAllWallets();
            GoTo("SplashScreen");
            return;
        }
        else{
            var wallets = WalletComponent.Instance.GetAllWallets();
            WalletComponent.Instance.SetCurrentWallet(wallets[wallets.Keys.First()]);
        }
        GoTo("MainScreen");
    }
}
