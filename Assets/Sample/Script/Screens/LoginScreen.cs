using SimpleScreen;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginScreen : BaseScreen
{
    public Button loginBtn;

    void Start()
    {
        loginBtn.onClick.AddListener(OnLogin);
    }

    private void OnLogin()
    {
        manager.ShowScreen("MnemonicScreen");
    }
}
