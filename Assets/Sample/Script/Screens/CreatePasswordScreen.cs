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
        if (!string.IsNullOrEmpty(passwordField.text) &&
                   !string.IsNullOrEmpty(confirmPasswordField.text))
        {
            if (passwordField.text == confirmPasswordField.text)
            {
                if (termsToggle.isOn)
                {
                    PlayerPrefs.SetString("password", passwordField.text);
                    PlayerPrefs.Save();
                    if (encryptMnem != null)
                    {
                        encryptMnem?.Invoke(passwordField.text);
                    }
                    return true;
                }
                else
                {
                    Debug.Log("Please accept terms and conditions");
                    return false;
                }
            }
            else
            {
                Debug.Log("Password does not match");
                return false;
            }
        }
        return false;
    }

    private void Transition()
    {
        Debug.Log(manager.previousScreen.name);
        if (manager.previousScreen.name == "IntroPage")
        {
            manager.ShowScreen("CreateRecoveryPhrase");
        }
        else
        {
            manager.ShowScreen("WalletSuccessScreen");
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
        if(data is Func<string, bool>)
        {
            encryptMnem = (Func<string, bool>)data;
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
    }
}