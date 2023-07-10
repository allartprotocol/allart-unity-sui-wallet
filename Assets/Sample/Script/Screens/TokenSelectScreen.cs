using AllArt.SUI.RPC.Response.Types;
using SimpleScreen;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TokenSelectScreen : BaseScreen
{
    public GameObject tokenSelectPrefab;

    public GameObject tokenSelectContainer;
    private List<TokenSelect> tokenSelects = new List<TokenSelect>();

    public override void ShowScreen(object data = null)
    {
        base.ShowScreen(data);
        PopulateTokenList();
    }

    public override void HideScreen()
    {
        base.HideScreen();
    }

    async void PopulateTokenList()
    {
        foreach (var tokenSelect in tokenSelects)
        {
            Destroy(tokenSelect.gameObject);
        }

        tokenSelects.Clear();
        foreach (var coin in WalletComponent.Instance.coinMetadatas)
        {
            Balance balance = await WalletComponent.Instance.GetBalance(WalletComponent.Instance.currentWallet.publicKey, coin.Key);
            if(balance == null)
            {
                continue;
            }
            var tokenSelect = Instantiate(tokenSelectPrefab, tokenSelectContainer.transform);
            var tokenSelectComponent = tokenSelect.GetComponent<TokenSelect>();
            var coinMetadata = WalletComponent.Instance.coinMetadatas[coin.Key];

            tokenSelectComponent.InitComponent(coin.Value, WalletComponent.Instance.coinGeckoData[coinMetadata.symbol], balance, manager);
            tokenSelects.Add(tokenSelectComponent);
        }
    }
}
