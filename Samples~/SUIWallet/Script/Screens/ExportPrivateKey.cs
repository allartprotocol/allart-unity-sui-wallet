using AllArt.SUI.Wallets;
using SimpleScreen;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExportPrivateKey : BaseScreen
{
    public Button loginBtn;
    public TMP_InputField password;

    public Toggle terms;
    private Wallet wallet;

    void Start()
    {
        loginBtn.onClick.AddListener(OnLogin);
    }

    public override void ShowScreen(object data = null)
    {
        base.ShowScreen(data);
        wallet = data as Wallet;
        password.text = "";
        terms.isOn = false;
    }

    private void OnLogin()
    {
        bool valid = false;
        if(terms.isOn == false)
        {
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Please accept agreement");
            return;
        }
        try
        {
            if (string.IsNullOrEmpty(password.text))
            {
                InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Please enter password");
                return;
            }
            else
            {
                valid = WalletComponent.Instance.CheckPasswordValidity(password.text);
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Invalid password");
        }

        if (valid)
        {
            WalletComponent.Instance.SetPassword(password.text);
            GoTo("DisplaySecretKey", wallet);
        }
        else
        {
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Invalid password");
        }
    }
}
