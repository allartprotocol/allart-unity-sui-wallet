using AllArt.SUI.Wallet;
using SimpleScreen;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowPhrase : BaseScreen { 
    
    public TextMeshProUGUI phraseText;
    public Wallet wallet;

    public Button doneBtn;
    public Button copyBtn;

    public void Start()
    {
        
        doneBtn.onClick.AddListener(OnDone);
        copyBtn.onClick.AddListener(OnCopy);

    }

    private void OnCopy()
    {
        GUIUtility.systemCopyBuffer = wallet.mnemonic;
    }

    private void OnDone()
    {
        manager.ShowScreen("MainScreen");
    }

    public override void ShowScreen(object data = null)
    {
        base.ShowScreen(data);
        wallet = data as Wallet;

        if (wallet != null)
        {
            phraseText.text = wallet.mnemonic;
        }
    }

}
