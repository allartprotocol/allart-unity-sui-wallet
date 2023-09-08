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

    private long GetAmmountFromBalanceChange(){
        long ammount = 0;
        foreach (var effect in suiTransactionBlockResponse.balanceChanges)
        {
            if(effect.owner.AddressOwner == WalletComponent.Instance.currentWallet.publicKey)
            {
                ammount = long.Parse(effect.amount);
            }
        }
        return ammount;
    }

    private long GetAmountFromInputs(){
        long ammount = 0;
        foreach(var input in suiTransactionBlockResponse.transaction.data.transaction.inputs)
        {
            if(input.valueType == "u64")
            {
                ammount = long.Parse(input.value);
            }
        }
        return ammount;
    }

    private async Task<CoinMetadata> GetTypeFromBalanceChanges(){
        string type = "";
        CoinMetadata coinMetadata = null;
        foreach (var effect in suiTransactionBlockResponse.balanceChanges)
        {
            if(effect.owner.AddressOwner == WalletComponent.Instance.currentWallet.publicKey)
            {
                type = effect.coinType;
            }
        }

        if(WalletComponent.Instance.coinMetadatas.ContainsKey(type))
            coinMetadata = WalletComponent.Instance.coinMetadatas[type];
        else{
            coinMetadata = await WalletComponent.Instance.GetCoinMetadata(type);
        }

        return coinMetadata;
    }

    async Task<string> GetBalanceChange() {
        long balanceChangeAmount = 0;
        long inputAmount = 0;
        string change = $"0";

        if(suiTransactionBlockResponse.balanceChanges == null)
            return "0";

        inputAmount = GetAmountFromInputs();
        balanceChangeAmount = GetAmmountFromBalanceChange();
        CoinMetadata coinMetadata = await GetTypeFromBalanceChanges(); 
        decimal gasUsedFloat = CalculateGasUsed(suiTransactionBlockResponse);

        if(coinMetadata == null)
            return change;

        decimal decimalChange = 0;

        if(inputAmount == 0)
        {
            balanceChangeAmount = balanceChangeAmount > 0 ? balanceChangeAmount : balanceChangeAmount + (long)gasUsedFloat;
            decimalChange = (decimal)balanceChangeAmount / (decimal)Mathf.Pow(10, coinMetadata.decimals);
        }
        else{
            decimalChange = (decimal)inputAmount / (decimal)Mathf.Pow(10, coinMetadata.decimals);
        }

        if(decimalChange > 0)
            change = $"+{decimalChange.ToString("0.############")} {coinMetadata.symbol}";
        else if(decimalChange < 0)
            change = $"-{decimalChange.ToString("0.############")} {coinMetadata.symbol}";

        return change;
    }

    private string GetReceiver()
    {
        foreach (var input in suiTransactionBlockResponse.transaction.data.transaction.inputs)
        {
            if(input.valueType == "address")
            {
                return input.value;
            }
        }
        return "Unknown Receiver";
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

        string coinType = "";
        foreach (var effect in suiTransactionBlockResponse.balanceChanges)
        {
            if(effect.owner.AddressOwner == WalletComponent.Instance.currentWallet.publicKey)
            {
                coinType = effect.coinType;
            }
        }

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

        DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeMilliseconds((long)ulong.Parse(suiTransactionBlockResponse.timestampMs));
        date.text = dateTime.ToString("MMMM d, yyyy 'at' h:mm tt");

        if (suiTransactionBlockResponse.transaction.data.sender == WalletComponent.Instance.currentWallet.publicKey)
            type.text = "Sent";
        else
            type.text = "Received";
        decimal gasUsedFloat = CalculateGasUsed(suiTransactionBlockResponse);

        status.text = suiTransactionBlockResponse.effects.status.status == "success" ? "Succeeded" : "Failed";
        sender.text = Wallet.DisplaySuiAddress(suiTransactionBlockResponse.transaction.data.sender);
        network.text = "SUI";
        var feeText = (gasUsedFloat / (decimal)Mathf.Pow(10, 9)).ToString("0.############");
        fee.text = $"~{feeText} SUI";
        try
        {
            balanceChange.text = await GetBalanceChange();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private decimal CalculateGasUsed(SuiTransactionBlockResponse suiTransactionBlockResponse)
    {
        var gasUsed = suiTransactionBlockResponse.effects.gasUsed;
        decimal gasUsedFloat = 0;
        if (gasUsed != null && gasUsed != default)
        {
            if (gasUsed.computationCost != null)
                gasUsedFloat += decimal.Parse(gasUsed.computationCost);
            if (gasUsed.storageCost != null)
                gasUsedFloat += decimal.Parse(gasUsed.storageCost);
            if (gasUsed.storageRebate != null)
                gasUsedFloat -= decimal.Parse(gasUsed.storageRebate);
        }
        return gasUsedFloat;
    }
}
