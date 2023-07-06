using SimpleScreen;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForgotPasswordScreen : BaseScreen
{
    public Button continueBtn;

    // Start is called before the first frame update
    void Start()
    {
        continueBtn.onClick.AddListener(OnContinue);
    }

    private void OnContinue()
    {
        WalletComponent.Instance.RemoveAllWallets();
        manager.ClearHistory(null);
        GoTo("RestoreScreen");
    }

}
