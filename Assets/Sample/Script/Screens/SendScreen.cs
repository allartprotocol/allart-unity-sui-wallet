using SimpleScreen;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SendScreen : BaseScreen
{
    public TMP_InputField to;
    public TMP_InputField amount;

    public Button continueBtn;

    private void Start()
    {
        continueBtn.onClick.AddListener(OnContinue);
    }

    public override void ShowScreen(object data = null)
    {
        base.ShowScreen(data);
    }

    private void OnContinue()
    {
        manager.ShowScreen("SendConfirmScreen", new TransferData()
        {
            to = to.text,
            amount = amount.text
        });
    }
}
