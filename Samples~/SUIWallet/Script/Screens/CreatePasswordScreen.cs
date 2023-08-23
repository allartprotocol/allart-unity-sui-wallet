using SimpleScreen;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatePasswordScreen : BaseScreen
{
    public TMP_InputField passwordField;
    public TMP_InputField confirmPasswordField;

    public Button continueBtn;

    public Toggle termsToggle;

    void Start()
    {
        continueBtn.onClick.AddListener(OnContinue);
    }

    private void OnContinue()
    {
        if (ValidateAndConfirmPassword())
        {
            Transition();
        }
    }

    private bool ValidateAndConfirmPassword()
    {
        if (string.IsNullOrEmpty(passwordField.text) &&
            string.IsNullOrEmpty(confirmPasswordField.text))
        {
            Debug.Log("Please fill all the fields");
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Please fill all the fields");
            return false;
        }

        if(passwordField.text.Contains(" ") || confirmPasswordField.text.Contains(" "))
        {
            Debug.Log("Password cannot contain spaces");
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Password cannot contain spaces");
            return false;
        }

        if (passwordField.text == confirmPasswordField.text)
        {
            if(passwordField.text.Length < 3)
            {
                Debug.Log("Password must be at least 3 characters long");
                InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Password must be at least 3 characters long");
                return false;
            }

            if (termsToggle.isOn)
            {
                WalletComponent.Instance.SetPassword(passwordField.text);
                if (encryptMnem != null)
                {
                    encryptMnem?.Invoke(passwordField.text);
                }
                InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Info, "Password created successfully");
                return true;
            }
            else
            {
                Debug.Log("Please accept terms and conditions");
                InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Please accept terms and conditions");
                return false;
            }
        }
        else
        {
            Debug.Log("Password does not match");
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Password does not match");
            return false;
        }
    }

    private void Transition()
    {
        Debug.Log(manager.previousScreen.name);
        if (manager.previousScreen.name == "IntroPage")
        {
            GoTo("CreateRecoveryPhrase");
        }
        else
        {
            GoTo("WalletSuccessScreen");
        }
    }

    public override void InitScreen()
    {
        base.InitScreen();
    }

    private Func<string, bool> encryptMnem;

    public override void ShowScreen(object data = null)
    {
        base.ShowScreen(data);
        if(data is Func<string, bool> func)
        {
            encryptMnem = func;
        }
        else
        {
            encryptMnem = null;
        }
        ClearInput();
    }

    public override void HideScreen()
    {
        base.HideScreen();
    }

    void ClearInput()
    {
        passwordField.text = "";
        confirmPasswordField.text = "";
        termsToggle.isOn = false;
    }
}
