using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using SimpleScreen;

public class CreatePasswordScreen : BaseScreen
{
    public TMP_InputField passwordField;
    public TMP_InputField confirmPasswordField;

    public Button continueBtn;

    public Toggle termsToggle;

    void Start()
    {
        continueBtn.onClick.AddListener(OnContinue);
    }

    private void OnContinue()
    {
        
    }

    public override void InitScreen()
    {
        base.InitScreen();
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
