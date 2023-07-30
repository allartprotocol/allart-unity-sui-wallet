using System;
using System.Collections;
using System.Collections.Generic;
using SimpleScreen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignScreen : BaseScreen
{
    public Button signBtn;
    public Button cancelBtn;

    public TextMeshProUGUI message;

    private byte[] messageToSign;

    // Start is called before the first frame update
    void Start()
    {        
        signBtn.onClick.AddListener(OnSign);
        cancelBtn.onClick.AddListener(OnCancel);
    }

    private void OnCancel()
    {
        GoTo("MainScreen");
    }

    private void OnSign()
    {
        try{
            WalletComponent.Instance.SignMessageWithCurrentWallet(messageToSign);
        }
        catch(Exception ex)
        {
            InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Error, "Failed to sign message");
        }
        WalletEvents.onRequestSignMessageResponse?.Invoke(messageToSign);
        GoTo("MainScreen");
    }

    override public void ShowScreen(object data = null)
    {
        base.ShowScreen(data);
        messageToSign = (byte[])data;
        message.text = $"Sign this message for authenticating with your wallet: '{WalletComponent.Instance.currentWallet.publicKey}'";
    }

}
