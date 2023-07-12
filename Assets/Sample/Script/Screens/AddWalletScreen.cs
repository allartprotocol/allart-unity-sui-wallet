using System;
using System.Collections;
using System.Collections.Generic;
using SimpleScreen;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AddWalletScreen : BaseScreen {

    public Button createBtn;
    public Button importPhraseBtn;
    public Button importPrivateKeyBtn;

    // Start is called before the first frame update
    void Start()
    {
        createBtn.onClick.AddListener(OnCreate);
        importPhraseBtn.onClick.AddListener(OnImportPhrase);
        importPrivateKeyBtn.onClick.AddListener(OnImportPrivateKey);
    }

    

    private void OnImportPrivateKey()
    {
        GoTo("ImportPrivateKeyScreen");
    }

    private void OnImportPhrase()
    {
        GoTo("ImportPrivateScreen");
    }

    private void OnCreate()
    {
        GoTo("CreateRecoveryPhrase");
    }
}
