using System.Collections;
using System.Collections.Generic;
using SimpleScreen;
using UnityEngine;
using UnityEngine.UI;

public class AddWalletScreen : BaseScreen {

    public Button createBtn;
    public Button importPhraseBtn;

    // Start is called before the first frame update
    void Start()
    {
        createBtn.onClick.AddListener(OnCreate);
        importPhraseBtn.onClick.AddListener(OnImportPhrase);
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
