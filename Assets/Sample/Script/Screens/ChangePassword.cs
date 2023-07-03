using SimpleScreen;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangePassword : BaseScreen
{
    public TMP_InputField oldPassword;
    public TMP_InputField newPassword;
    public TMP_InputField confirmPassword;

    public Button change;

    // Start is called before the first frame update
    void Start()
    {
        change.onClick.AddListener(PasswordChange);
    }

    private void PasswordChange()
    {
        if(string.IsNullOrEmpty(oldPassword.text) || string.IsNullOrEmpty(newPassword.text) || string.IsNullOrEmpty(confirmPassword.text))
        {
            //manager.ShowPopup("Error", "Please fill all the fields");
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Please fill all the fields");
            return;
        }

        try
        {

            if(!WalletComponent.Instance.CheckPasswordValidity(oldPassword.text))
            {
                //manager.ShowPopup("Error", "Password is not valid");
                InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Password is not valid");
                return;
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            //manager.ShowPopup("Error", "Invalid password");
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Invalid password");
            return;
        }


        if(newPassword.text != confirmPassword.text)
        {
            //manager.ShowPopup("Error", "New password and confirm password does not match");
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "New password and confirm password does not match");
            return;
        }

        InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Info, "Password changed successfully");
        WalletComponent.Instance.ChangePassword(oldPassword.text, newPassword.text);
        WalletComponent.Instance.SetPassword(newPassword.text);
        GoTo("MainScreen");
    }
}
