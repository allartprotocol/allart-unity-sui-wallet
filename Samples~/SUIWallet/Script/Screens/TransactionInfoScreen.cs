using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllArt.SUI.RPC.Response;
using AllArt.SUI.Wallets;
using Newtonsoft.Json;
using SimpleScreen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TransactionInfoScreen : BaseScreen {

    SuiTransactionBlockResponse suiTransactionBlockResponse;

    public TextMeshProUGUI date;
    public TextMeshProUGUI status;

    public TextMeshProUGUI senderLabel;
    public TextMeshProUGUI sender;
    public TextMeshProUGUI network;
    public TextMeshProUGUI fee;

    public TextMeshProUGUI type;
    public TextMeshProUGUI balanceChange;

    public Sprite failImage;
    public Sprite successImage;

    public TokenImage tokenImage;

    public UnityEngine.UI.Image statusImage;

    public Button suiExplorerBtn;

    private void Start() {
        suiExplorerBtn.onClick.AddListener(OnSuiExplorer);
    }

    async Task<string> GetBalanceChange(CoinMetadata coinMetadata = null) {
        long balanceChangeAmount = 0;
        long inputAmount = 0;
        string change = $"0";

        if(suiTransactionBlockResponse.balanceChanges == null)
            return "0";

        inputAmount = PayTransactionParsing.GetAmountFromInputs(suiTransactionBlockResponse);
        balanceChangeAmount = PayTransactionParsing.GetAmmountFromBalanceChange(suiTransactionBlockResponse, WalletComponent.Instance.currentWallet);
        
        decimal gasUsedFloat = PayTransactionParsing.CalculateGasUsed(suiTransactionBlockResponse);

        if(coinMetadata == null)
            return change;

        decimal decimalChange = 0;
        Debug.Log("inputAmount: " + inputAmount);

        if(inputAmount == 0)
        {
            balanceChangeAmount = balanceChangeAmount > 0 ? balanceChangeAmount : balanceChangeAmount + (long)gasUsedFloat;
            decimalChange = (decimal)balanceChangeAmount / (decimal)Mathf.Pow(10, coinMetadata.decimals);
        }
        else{
            decimalChange = (decimal)inputAmount / (decimal)Mathf.Pow(10, coinMetadata.decimals);
        }

        Debug.Log(decimalChange);

        if(suiTransactionBlockResponse.transaction.data.sender == WalletComponent.Instance.currentWallet.publicKey)
            decimalChange = Math.Abs(decimalChange) * -1;

        if(decimalChange > 0)
            change = $"+{WalletUtility.ParseDecimalValueToString(decimalChange, true)} {coinMetadata.symbol}";
        else if(decimalChange < 0)
            change = $"{WalletUtility.ParseDecimalValueToString(decimalChange, true)} {coinMetadata.symbol}";

        return change;
    }

  

    private void OnSuiExplorer()
    {
        string network = WalletComponent.Instance.nodeType switch
        {
            ENodeType.MainNet => "mainnet",
            ENodeType.TestNet => "testnet",
            _ => "devnet",
        };

        //open browser with the transaction hash
        Application.OpenURL($"https://suiscan.xyz/{network}/tx/{suiTransactionBlockResponse.digest}");//$"https://suiexplorer.com/txblock/{suiTransactionBlockResponse.digest}?network={network}");
    }

    public async override void ShowScreen(object data)
    {
        base.ShowScreen(data);
        tokenImage = GetComponentInChildren<TokenImage>();

        suiTransactionBlockResponse = data as SuiTransactionBlockResponse;

        string coinType = PayTransactionParsing.GetCoinTypeFromObjectChanges(suiTransactionBlockResponse, WalletComponent.Instance.currentWallet);
        Debug.Log(coinType);

        CoinMetadata coinMetadata = null;

        if(WalletComponent.Instance.coinMetadatas.ContainsKey(coinType))
            coinMetadata = WalletComponent.Instance.coinMetadatas[coinType];
        else{
            coinMetadata = await WalletComponent.Instance.GetCoinMetadata(coinType);
        }

        if(coinMetadata == null)
            tokenImage.Init(null, "");
        else{
            Sprite icon = WalletComponent.Instance.GetCoinImage(coinMetadata.symbol);
            tokenImage.Init(icon, coinMetadata.symbol);
        }

        DateTimeOffset dateTime = PayTransactionParsing.GetDateTimeFromBlock(suiTransactionBlockResponse);
        date.text = dateTime.ToString("MMMM d, yyyy 'at' h:mm tt");

        if (suiTransactionBlockResponse.transaction.data.sender == WalletComponent.Instance.currentWallet.publicKey)
        {
            senderLabel.text = "To";
            sender.text = Wallet.DisplaySuiAddress(PayTransactionParsing.GetReceiver(suiTransactionBlockResponse));
            type.text = "Sent";
        }
        else
        {
            senderLabel.text = "From";
            sender.text = Wallet.DisplaySuiAddress(suiTransactionBlockResponse.transaction.data.sender);
            type.text = "Received";
        }

        decimal gasUsedFloat = PayTransactionParsing.CalculateGasUsed(suiTransactionBlockResponse);

        status.text = suiTransactionBlockResponse.effects.status.status == "success" ? "Succeeded" : "Failed";
        network.text = "SUI";
        var feeText = (gasUsedFloat / (decimal)Mathf.Pow(10, 9)).ToString("0.############");
        fee.text = $"~{feeText} SUI";
        try
        {
            balanceChange.text = await GetBalanceChange(coinMetadata);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
