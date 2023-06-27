using SimpleScreen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginScreen : BaseScreen
{
    public Button loginBtn;
    public TMP_InputField password;

    void Start()
    {
        loginBtn.onClick.AddListener(OnLogin);
    }

    private void OnLogin()
    {
        bool valid = WalletComponent.Instance.CheckPasswordValidity(password.text);
        Debug.Log(password.text);
        if (valid)
        {
            manager.ShowScreen("MainScreen");
        }
    }
}
