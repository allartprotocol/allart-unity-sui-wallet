using AllArt.SUI.Wallet;
using SimpleScreen;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RestoreScreen : BaseScreen
{
    public TMP_InputField mnemonicField;

    public Button continueBtn;
    private string mnemonic;

    void Start()
    {
        continueBtn.onClick.AddListener(OnContinue);
    }

    private void OnContinue()
    {
        string mnemonic = mnemonicField.text;

        if(string.IsNullOrEmpty(mnemonic))
        {
            Debug.Log("Please enter mnemonic");
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Please enter mnemonic");
            return;
        }

        mnemonic = Mnemonics.SanitizeMnemonic(mnemonic);
        Debug.Log(mnemonic);

        if(!Mnemonics.IsValidMnemonic(mnemonicField.text))
        {
            Debug.Log("Invalid mnemonic");
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Invalid mnemonic");
            return;
        }

        this.mnemonic = mnemonic;        
        GoTo("PasswordCreate", (Func<string, bool>)CreateAndEncryptMnemonic);
    }


    public override void InitScreen()
    {
        base.InitScreen();
    }

    public bool CreateAndEncryptMnemonic(string password) {
        Debug.Log(mnemonic + " " + password);
        WalletComponent.Instance.CreateNewWallet(this.mnemonic, password);
        return true;
    }

    public override void ShowScreen(object data = null)
    {
        base.ShowScreen(data);
        mnemonicField.text = "";
    }

    public override void HideScreen()
    {
        base.HideScreen();
    }
}