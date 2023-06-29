using SimpleScreen;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WalletListScreen : BaseScreen
{
    public Button addWallet;
    public GameObject walletItemPrefab;

    public Transform container;

    public List<WalletListObject> walletListObjects = new List<WalletListObject>();

    // Start is called before the first frame update
    void Start()
    {
        addWallet.onClick.AddListener(OnAddWallet);
    }

    private void OnAddWallet()
    {
        manager.ShowScreen("ImportPrivateScreen");
    }

    void PopulateDropdownWithWallets(Dictionary<string, Wallet> wallets)
    {
        foreach (var walletListObject in walletListObjects)
        {
            Destroy(walletListObject.gameObject);
        }

        walletListObjects.Clear();

        foreach (var wallet in wallets)
        {
            if(walletListObjects.Exists(x => x.walletName == wallet.Key))
            {
                continue;
            }
            var walletItem = Instantiate(walletItemPrefab, container);
            var walletListObject = walletItem.GetComponent<WalletListObject>();
            walletListObject.SetWallet(wallet.Value, wallet.Key, manager);
            walletListObjects.Add(walletListObject);            
        }
    }

    public override void ShowScreen(object data = null)
    {
        base.ShowScreen(data);
        PopulateDropdownWithWallets(WalletComponent.Instance.GetAllWallets());
    }
}
