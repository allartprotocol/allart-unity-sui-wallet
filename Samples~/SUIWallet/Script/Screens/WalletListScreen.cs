using AllArt.SUI.Wallets;
using SimpleScreen;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WalletListScreen : BaseScreen
{
    public Button addWallet;
    public GameObject walletItemPrefab;

    public Transform container;

    public Button settingsButton;
    public Button lockButton;

    public List<WalletListObject> walletListObjects = new List<WalletListObject>();

    // Start is called before the first frame update
    void Start()
    {
        addWallet.onClick.AddListener(OnAddWallet);
        settingsButton.onClick.AddListener(OnSettings);
        lockButton.onClick.AddListener(OnLock);
    }

    private void OnLock()
    {
        WalletComponent.Instance.LockWallet();
        GoTo("LoginScreen");
    }

    private void OnSettings()
    {
        GoTo("SettingsScreen");
    }

    private void OnAddWallet()
    {
        GoTo("AddWalletScreen");
    }

    void PopulateDropdownWithWallets(Dictionary<string, AllArt.SUI.Wallets.Wallet> wallets)
    {
        foreach (var walletListObject in walletListObjects)
        {
            Destroy(walletListObject.gameObject);
        }

        walletListObjects.Clear();

        int index = 1;
        foreach (var wallet in wallets)
        {
            if (walletListObjects.Exists(x => x.walletName == wallet.Key))
            {
                continue;
            }
            var walletItem = Instantiate(walletItemPrefab, container);
            var walletListObject = walletItem.GetComponent<WalletListObject>();
            walletListObject.SetWallet(index, wallet.Value, wallet.Key, manager);
            walletListObjects.Add(walletListObject);
            index++;
        }

        addWallet.transform.SetAsLastSibling();
    }

    public override void ShowScreen(object data = null)
    {
        base.ShowScreen(data);
        PopulateDropdownWithWallets(WalletComponent.Instance.GetAllWallets());
    }
}
