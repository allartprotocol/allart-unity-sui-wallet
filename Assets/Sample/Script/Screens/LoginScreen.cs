using SimpleScreen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginScreen : BaseScreen
{
    public Button loginBtn;
    public TMP_InputField password;

    public Button forgotPasswordBtn;

    void Start()
    {
        loginBtn.onClick.AddListener(OnLogin);
        forgotPasswordBtn.onClick.AddListener(OnForgotPassword);
    }

    private void OnForgotPassword()
    {
        manager.ShowScreen("ForgotPassword");
    }

    private void OnLogin()
    {
        bool valid = false;
        try {             
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
            manager.ShowScreen("MainScreen");
        }
        else
        {
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Invalid password");
        }
    }
}
