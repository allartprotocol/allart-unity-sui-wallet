using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExternalRequestsExample : MonoBehaviour
{
    public Button requestSignMessageBtn;
    public Button toggleWalletBtn;

    // Start is called before the first frame update
    void Start()
    {
        
        requestSignMessageBtn.onClick.AddListener(OnRequestSignMessage);
        toggleWalletBtn.onClick.AddListener(OnToggleWallet);

    }

    private void OnRequestSignMessage()
    {
        WalletComponent.Instance.RequestSignMessage(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
    }

    private void OnToggleWallet()
    {
        WalletComponent.Instance.ToggleWallet();
    }
}
