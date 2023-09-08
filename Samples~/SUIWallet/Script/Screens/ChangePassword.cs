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

    override public void ShowScreen(object data = null)
    {
        base.ShowScreen(data);
        oldPassword.text = "";
        newPassword.text = "";
        confirmPassword.text = "";
    }

    private void PasswordChange()
    {
        Debug.Log("PasswordChange");
        Debug.Log(oldPassword.text);
        Debug.Log(newPassword.text);
        Debug.Log(confirmPassword.text);
        if(string.IsNullOrEmpty(oldPassword.text) || string.IsNullOrEmpty(newPassword.text) || string.IsNullOrEmpty(confirmPassword.text))
        {
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Please fill in all the fields");
            return;
        }

        if(oldPassword.text.Length < 3 || newPassword.text.Length < 3 || confirmPassword.text.Length < 3)
        {
            Debug.Log("Password must be at least 3 characters long");
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Password must be at least 3 characters long");
            return;
        }

        if(oldPassword.text == newPassword.text)
        {
            Debug.Log("New password must be different from old password");
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "New password must be different from current password");
            return;
        }

        if(newPassword.text != confirmPassword.text)
        {
            Debug.Log("New password and confirm password does not match");
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "New password and confirm password does not match");
            return;
        }

        try
        {
            if(WalletComponent.Instance.CheckPasswordValidity(oldPassword.text) == false)
            {
                Debug.Log("Password is not valid");
                InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Password is not valid");
                return;
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Invalid password");
            return;
        }


        if(newPassword.text != confirmPassword.text)
        {
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "New password and confirm password does not match");
            return;
        }

        WalletComponent.Instance.ChangePassword(oldPassword.text, newPassword.text);
        WalletComponent.Instance.SetPassword(newPassword.text);
        InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Info, "Password updated");
        manager.GoBack();
    }
}
