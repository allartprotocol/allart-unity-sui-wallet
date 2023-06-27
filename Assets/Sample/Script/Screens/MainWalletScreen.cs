using SimpleScreen;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainWalletScreen : BaseScreen
{
    public TextMeshProUGUI walletPubText;
    public TextMeshProUGUI walletBalanceText;

    public TMP_Dropdown walletsDropdown;

    public Button receiveBtn;
    public Button sendBtn;

    public Transform objectListContent;
    public GameObject objectListItemPrefab;

    private void Start()
    {
        receiveBtn.onClick.AddListener(OnReceive);
        sendBtn.onClick.AddListener(OnSend);
        walletsDropdown.onValueChanged.AddListener(OnWalletSelected);
    }

    private void OnWalletSelected(int value)
    {

    }

    private void OnSend()
    {
    }

    private void OnReceive()
    {
    }

    private void LoadWalletData()
    {
        var wallet = WalletComponent.Instance.GetWalletByIndex(0);

        walletPubText.text = wallet.publicKey;

        walletBalanceText.text = "0";
    }

    public override void ShowScreen(object data = null)
    {
        base.ShowScreen(data);

        var wallet = WalletComponent.Instance.GetAllWallets();

        if (wallet.Count > 0)
        {
            PopulateDropdownWithWallets(wallet, walletsDropdown);
            walletsDropdown.value = 0;
        }
        else
        {
            Debug.Log("No wallet found");
        }
    }

    void PopulateDropdownWithWallets(Dictionary<string, Wallet> wallets, TMP_Dropdown walletsDropdown)
    {
        foreach (var wallet in wallets)
        {
            walletsDropdown.options.Add(new TMP_Dropdown.OptionData(wallet.Key));
        }
    }

    public override void HideScreen()
    {
        base.HideScreen();
    }
}
