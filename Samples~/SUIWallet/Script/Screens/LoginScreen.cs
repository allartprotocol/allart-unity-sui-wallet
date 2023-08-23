using SimpleScreen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginScreen : BaseScreen
{
    public Button loginBtn;
    public TMP_InputField password;

    public Button forgotPasswordBtn;
    private byte[] messageToSign;

    void Start()
    {
        loginBtn.onClick.AddListener(OnLogin);
        forgotPasswordBtn.onClick.AddListener(OnForgotPassword);
    }

    public override void ShowScreen(object data = null)
    {
        base.ShowScreen(data);
        messageToSign = (byte[])data;
        password.text = "";
    }

    private void OnForgotPassword()
    {
        GoTo("ForgotPassword");
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
            return;
        }
        
        if (valid)
        {
            WalletComponent.Instance.SetPassword(password.text);
            if(messageToSign != null)
            {
                WalletEvents.onRequestSignMessageResponse?.Invoke(messageToSign);
                GoTo("SignScreen");
                return;
            }
            GoTo("MainScreen");
        }
        else
        {
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Invalid password");
        }
    }
}
