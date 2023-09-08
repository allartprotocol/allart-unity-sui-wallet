using AllArt.SUI.Wallets;
using SimpleScreen;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowSecretKey : BaseScreen
{

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
        GUIUtility.systemCopyBuffer = wallet.privateKey;
        InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Info, "Copied to clipboard");
    }

    private void OnDone()
    {
        manager.GoBack();
        manager.GoBack();
    }

    public override void ShowScreen(object data = null)
    {
        base.ShowScreen(data);
        wallet = data as Wallet;

        if (wallet != null)
        {
            phraseText.text = wallet.privateKey;
        }
    }

}
