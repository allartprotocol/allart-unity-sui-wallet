using SimpleScreen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CreationSuccessScreen : BaseScreen
{
    public Button continueBtn;

    // Start is called before the first frame update
    void Start()
    {
        continueBtn.onClick.AddListener(OnContinue);
    }

    private void OnContinue()
    {
        GoTo("LoginScreen");
    }

}
