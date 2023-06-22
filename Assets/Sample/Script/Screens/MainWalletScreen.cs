using SimpleScreen;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainWalletScreen : BaseScreen
{
    public TextMeshProUGUI walletPubText;
    public TextMeshProUGUI walletBalanceText;

    public Button receiveBtn;
    public Button sendBtn;

    public Transform objectListContent;
    public GameObject objectListItemPrefab;

    private void Start()
    {
        receiveBtn.onClick.AddListener(OnReceive);
        sendBtn.onClick.AddListener(OnSend);
    }

    private void OnSend()
    {
    }

    private void OnReceive()
    {
    }

    public override void ShowScreen(object data = null)
    {
        base.ShowScreen(data);
    }

    public override void HideScreen()
    {
        base.HideScreen();
    }
}
