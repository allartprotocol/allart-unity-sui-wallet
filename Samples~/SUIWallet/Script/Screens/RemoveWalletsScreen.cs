using SimpleScreen;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoveWalletsScreen : BaseScreen
{
    public Button removeWallets;

    private void Start()
    {
        removeWallets.onClick.AddListener(OnRemoveWallets);
    }

    private void OnRemoveWallets()
    {
        WalletComponent.Instance.RemoveAllWallets();
        GoTo("SplashScreen");
    }
}
