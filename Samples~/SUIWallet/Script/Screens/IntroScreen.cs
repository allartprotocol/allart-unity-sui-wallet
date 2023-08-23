using SimpleScreen;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroScreen : BaseScreen
{
    public Button createBtn;
    public Button restoreBtn;

    private void Start()
    {
        createBtn.onClick.AddListener(GoToCreate);
        restoreBtn.onClick.AddListener(GoToRestore);
    }

    private void GoToRestore()
    {
        GoTo("RestoreScreen");
    }

    private void GoToCreate()
    {
        GoTo("PasswordCreate");
    }
}
