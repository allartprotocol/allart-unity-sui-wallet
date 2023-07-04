using AllArt.SUI.Wallet;
using SimpleScreen;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImportMnemonicScreen : BaseScreen
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

        if (string.IsNullOrEmpty(mnemonic))
        {
            Debug.Log("Please enter mnemonic");
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Please enter mnemonic");
            return;
        }

        mnemonic = Mnemonics.SanitizeMnemonic(mnemonic);
        Debug.Log(mnemonic);

        if (!Mnemonics.IsValidMnemonic(mnemonicField.text))
        {
            Debug.Log("Invalid mnemonic");
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Invalid mnemonic");
            return;
        }

        if(WalletComponent.Instance.DoesWalletWithMnemonicExists(mnemonic))
        {
            Debug.Log("Wallet already exist");
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Wallet already exist");
            return;
        }

        this.mnemonic = mnemonic;
        Wallet wal = CreateAndEncryptMnemonic(WalletComponent.Instance.password);
        if(wal != null)
        {
            WalletComponent.Instance.SetCurrentWallet(wal);
            WalletComponent.Instance.RestoreAllWallets(WalletComponent.Instance.password);
        }
        else
        {
            Debug.Log("Wallet creation failed");
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Wallet creation failed");
            return;
        }
        InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Info, "Wallet imported successfully");
        GoTo("MainScreen");
    }


    public override void InitScreen()
    {
        base.InitScreen();
    }

    public Wallet CreateAndEncryptMnemonic(string password)
    {
        Debug.Log(mnemonic + " " + password);
        return WalletComponent.Instance.CreateWallet(this.mnemonic, password);
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
