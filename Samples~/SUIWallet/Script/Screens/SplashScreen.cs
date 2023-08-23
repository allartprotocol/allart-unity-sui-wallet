using SimpleScreen;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreen : BaseScreen
{

    public override void ShowScreen(object data = null)
    {
        base.ShowScreen(data);
        //PlayerPrefs.DeleteAll();
        StartCoroutine(StartWallet());
    }

    private IEnumerator StartWallet()
    {
        yield return new WaitForSeconds(2f);
        if(PlayerPrefs.HasKey("wallets"))
            GoTo("LoginScreen");
        else
            GoTo("IntroPage");
    }
}
