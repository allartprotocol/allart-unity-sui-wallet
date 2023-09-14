using SimpleScreen;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreen : BaseScreen
{
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI networkText;

    public Button changePassword;
    public Button autolockTimer;
    public Button network;
    public Button removeWallets;

    private void Start()
    {
        changePassword.onClick.AddListener(OnChangePassword);
        autolockTimer.onClick.AddListener(OnAutolockTimer);
        network.onClick.AddListener(OnNetwork);
        removeWallets.onClick.AddListener(OnRemoveWallets);
    }

    override public void ShowScreen(object data = null)
    {
        base.ShowScreen(data);
        var time = PlayerPrefs.GetFloat("timeout", -1);
        var network = (ENodeType)PlayerPrefs.GetInt("nodeType", 0);
        var timestring = ConvertSecondsToMinutesOrHours((int)time);
        timeText.text = time == -1 ? "Never" : timestring;
        networkText.text = network.ToString();
    }

    public static string ConvertSecondsToMinutesOrHours(int seconds)
{
    if (seconds < 60)
    {
        return seconds + " seconds";
    }
    else if (seconds < 3600)
    {
        int minutes = seconds / 60;
        int remainingSeconds = seconds % 60;
        return minutes + " min" + (remainingSeconds > 0 ? " and " + remainingSeconds + " sec" : "");
    }
    else
    {
        int hours = seconds / 3600;
        int remainingSeconds = seconds % 3600;
        int minutes = remainingSeconds / 60;
        int remainingSeconds2 = remainingSeconds % 60;
        if (remainingSeconds2 == 0)
        {
            return hours + " hours";
        }
        else
        {
            return hours + " hours" + (minutes > 0 ? " and " + minutes + " min" : "") + (remainingSeconds2 > 0 ? " and " + remainingSeconds2 + " sec" : "");
        }
    }
}

    private void OnRemoveWallets()
    {
        GoTo("RemoveAccountScreen");
    }

    private void OnNetwork()
    {
        GoTo("NetworkSelectScreen");
    }

    private void OnAutolockTimer()
    {
        GoTo("TimerSelect");
    }

    private void OnChangePassword()
    {
        GoTo("ChangePassword");

    }
}
