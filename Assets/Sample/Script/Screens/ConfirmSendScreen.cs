using Chaos.NaCl;
using Newtonsoft.Json;
using SimpleScreen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmSendScreen : BaseScreen {

    public TextMeshProUGUI to;
    public TextMeshProUGUI fee;

    public Button confirmBtn;
    private TransferData TransferData;

    private void Start()
    {
        confirmBtn.onClick.AddListener(OnConfirm);
    }

    private async void OnConfirm()
    {
        var wallet = WalletComponent.Instance.currentWallet;
        ObjectResponseQuery query = new ObjectResponseQuery();
        var filter = new MatchAllDataFilter();
        filter.MatchAll = new List<ObjectDataFilter>
        {
            new StructTypeDataFilter() { StructType = "0x2::coin::Coin<0x2::sui::SUI>" },
            new OwnerDataFilter() { AddressOwner = wallet.publicKey }
        };
        query.filter = filter;
        ObjectDataOptions options = new ObjectDataOptions();
        query.options = options;
        var res = await WalletComponent.Instance.GetOwnedObjects(wallet.publicKey, query, null, 3);
        Debug.Log(JsonConvert.SerializeObject(res));
        var res_pay = await WalletComponent.Instance.PaySui(wallet, res.data[0].data.objectId, TransferData.to,
            ulong.Parse(TransferData.amount), "1000000000");

        Debug.Log(JsonConvert.SerializeObject(res_pay));
        var signature = wallet.SignData(Wallet.GetMessageWithIntent(CryptoBytes.FromBase64String(res_pay.txBytes)));

        var transaction = await WalletComponent.Instance.client.ExecuteTransactionBlock(res_pay.txBytes,
            new string[] { signature }, new ObjectDataOptions(), ExecuteTransactionRequestType.WaitForEffectsCert);
    }

    public override void ShowScreen(object data = null)
    {
        base.ShowScreen(data);

        var confirmSendData = data as TransferData;
        Debug.Log(confirmSendData.to);
        if (confirmSendData != null)
        {
            TransferData = confirmSendData;
            to.text = confirmSendData.to;
            Debug.LogError("ConfirmSendScreen: data is null");
            return;
        }
    }


}
