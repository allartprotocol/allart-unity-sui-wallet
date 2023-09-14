using AllArt.SUI.Wallets;
using SimpleScreen;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateMnemonicScreen : BaseScreen
{
    public List<WordComponent> words = new List<WordComponent>();

    public Button continueBtn;
    public Button copyBtn;

    public Toggle saveConfirmationToggle;
    private string mnemonic;

    void Start()
    {
        continueBtn.onClick.AddListener(OnContinue);
        copyBtn.onClick.AddListener(OnCopy);
    }

    private void OnCopy()
    {
        GUIUtility.systemCopyBuffer = mnemonic;
        InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Info, "Copied to clipboard");
    }

    private void OnContinue()
    {
        if (!saveConfirmationToggle.isOn)
        {
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Please confirm that you have saved your mnemonic");
            return;
        }

        string password = WalletComponent.Instance.password;

        Wallet wallet = WalletComponent.Instance.CreateWallet(mnemonic, password);
        wallet.SaveWallet();
        WalletComponent.Instance.RestoreAllWallets(password);
        WalletComponent.Instance.SetCurrentWallet(wallet);
        GoTo("WalletSuccessScreen", "Wallet Created successfully!");
    }

    private void PopulateWords()
    {
        mnemonic = Mnemonics.GenerateNewMnemonic();
        string[] words = mnemonic.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            this.words[i].SetData(i + 1, words[i]);
        }
    }

    public override void InitScreen()
    {
        base.InitScreen();
    }

    public override void ShowScreen(object data = null)
    {
        base.ShowScreen(data);
        PopulateWords();
        WalletComponent.Instance.SetCurrentWallet(null);
        saveConfirmationToggle.isOn = false;
    }

    public override void HideScreen()
    {
        base.HideScreen();
    }
}
