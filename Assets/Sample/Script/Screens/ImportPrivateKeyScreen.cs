using System.Collections;
using System.Collections.Generic;
using System.Text;
using AllArt.SUI.Wallets;
using Chaos.NaCl;
using SimpleScreen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImportPrivateKeyScreen : BaseScreen
{
    public TMP_InputField mnemonicField;

    public Button continueBtn;
    private string privateKey;

    void Start()
    {
        continueBtn.onClick.AddListener(OnContinue);
    }

    private void OnContinue()
    {
        string privateKey = mnemonicField.text;

        if (string.IsNullOrEmpty(privateKey))
        {
            Debug.Log("Please enter private key");
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Please enter private key");
            return;
        }

        this.privateKey = privateKey.Trim();

        Wallet wal = CreateAndEncryptWallet(this.privateKey, WalletComponent.Instance.password);

        if(wal != null)
        {
            WalletComponent.Instance.SetCurrentWallet(wal);
            WalletComponent.Instance.RestoreAllWallets(WalletComponent.Instance.password);
            Debug.Log(wal.publicKey);
            Debug.Log(wal.privateKey);
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

    public Wallet CreateAndEncryptWallet(string privateKey, string password)
    {
        return WalletComponent.Instance.CreateWalletFromPrivateKey(privateKey, password);
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