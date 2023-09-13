using AllArt.SUI.RPC.Response;
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

    void PopulateTokenList()
    {
        foreach (var tokenSelect in tokenSelects)
        {
            Destroy(tokenSelect.gameObject);
        }

        tokenSelects.Clear();
        foreach (var coin in WalletComponent.Instance.coinMetadatas)
        {
            Balance balance = WalletComponent.Instance.GetCurrentWalletBalanceForType(coin.Key);
            if(balance == null || balance.totalBalance == 0)
            {
                continue;
            }
            var tokenSelect = Instantiate(tokenSelectPrefab, tokenSelectContainer.transform);
            var tokenSelectComponent = tokenSelect.GetComponent<TokenSelect>();
            var coinMetadata = WalletComponent.Instance.coinMetadatas[coin.Key];

            SUIMarketData coinGeckoData = WalletComponent.Instance.GetCoinMarketData(coinMetadata.symbol);

            tokenSelectComponent.InitComponent(coin.Value, coinGeckoData, balance, manager);
            tokenSelects.Add(tokenSelectComponent);
        }
    }
}
