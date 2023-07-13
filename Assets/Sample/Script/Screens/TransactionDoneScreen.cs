using SimpleScreen;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TransactionDoneScreen : BaseScreen
{
    public Button done;
    public Image statusImage;

    public Sprite success;
    public Sprite fail;

    public TextMeshProUGUI statusText;

    // Start is called before the first frame update
    void Start()
    {
        done.onClick.AddListener(OnDone);
    }

    private void OnDone()
    {
        GoTo("MainScreen");
    }

    public override void ShowScreen(object data = null)
    {
        base.ShowScreen(data);
        var status = (bool)data;

        if(status)
        {
            statusImage.sprite = success;
            statusText.text = "Sent";
        }
        else
        {
            statusText.text = "Failed";
            statusImage.sprite = fail;
        }
    }

}
