using AllArt.SUI.Wallets;
using SimpleScreen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RestoreScreen : BaseScreen
{
    public TMP_InputField mnemonicField;
    public List<WordComponent> wordComponents = new();
    private List<TMP_InputField> tMP_InputFields = new();

    public Button continueBtn;

    private string mnemonic;
    private int currentIndex = 0;

    void Start()
    {
        continueBtn.onClick.AddListener(OnContinue);
        wordComponents = GetComponentsInChildren<WordComponent>().ToList();
        foreach (var item in wordComponents)
        {
            var input = item.GetComponentInChildren<TMP_InputField>();
            tMP_InputFields.Add(input);
            input.onValueChanged.AddListener((string value) => OnWordChanged(value, input));
            input.onSelect.AddListener((string value) => OnWordSelected(value, input));
            input.onSubmit.AddListener((string value) => OnWordSubmitted(value, input));
        }
    }

    private void OnWordSubmitted(string value, TMP_InputField input)
    {
        Debug.Log(input.text);
        GoToNextInput(input);
    }

    private void GoToNextInput(TMP_InputField input)
    {
        currentIndex = tMP_InputFields.IndexOf(input);
        if (currentIndex == tMP_InputFields.Count - 1)
        {
            tMP_InputFields[0].Select();
        }
        else
        {
            tMP_InputFields[currentIndex + 1].Select();
        }
    }

    private void OnWordSelected(string value, TMP_InputField input)
    {
        currentIndex = tMP_InputFields.IndexOf(input);
    }

    private void OnWordChanged(string value, TMP_InputField input)
    {
        Debug.Log(value);
        if(string.IsNullOrEmpty(value))
        {
            return;
        }

        // string updatedValue = value.Trim().ToLower();
        // var words = updatedValue.Split(' ');
        // if(words.Length == 12)
        // {
        //     mnemonic = updatedValue;
        //     PopulateInputsWithMnemonic(mnemonic);
        //     return;
        // }

        if(value.Contains(" "))
        {
            input.text = value.Replace(" ", "");
            GoToNextInput(input);
            return;
        }
    }

    private void PopulateInputsWithMnemonic(string mnemonic){
        Debug.Log(mnemonic);
        var words = mnemonic.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            tMP_InputFields[i].SetTextWithoutNotify(words[i]);
        }
    }

    public string GetMnemonic()
    {
        string mnemonic = "";
        foreach (var item in tMP_InputFields)
        {
            mnemonic += item.text + " ";
        }
        return mnemonic.Trim();
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

        if(!Mnemonics.IsValidMnemonic(mnemonic))
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
        Wallet wallet = WalletComponent.Instance.CreateWallet(this.mnemonic, password);
        wallet.SaveWallet();
        WalletComponent.Instance.RestoreAllWallets(password);
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