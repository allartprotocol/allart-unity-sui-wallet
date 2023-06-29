using SimpleScreen;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransactionDoneScreen : BaseScreen
{
    public Button done;

    // Start is called before the first frame update
    void Start()
    {
        done.onClick.AddListener(OnDone);
    }

    private void OnDone()
    {
        manager.ShowScreen("MainScreen");
    }

}
