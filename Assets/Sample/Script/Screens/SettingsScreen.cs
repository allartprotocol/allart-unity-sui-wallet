using SimpleScreen;
using System;
using UnityEngine.UI;

public class SettingsScreen : BaseScreen
{
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

    private void OnRemoveWallets()
    {
        manager.ShowScreen("RemoveWalletsScreen");
    }

    private void OnNetwork()
    {
        manager.ShowScreen("NetworkSelectScreen");
    }

    private void OnAutolockTimer()
    {
        throw new NotImplementedException();
    }

    private void OnChangePassword()
    {
        manager.ShowScreen("ChangePassword");

    }
}