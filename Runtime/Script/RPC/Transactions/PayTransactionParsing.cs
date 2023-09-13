

using System;
using System.Threading.Tasks;
using AllArt.SUI.RPC.Response;
using AllArt.SUI.Wallets;
using UnityEngine;

public static class PayTransactionParsing {
    public static long GetAmmountFromBalanceChange(SuiTransactionBlockResponse suiTransactionBlockResponse, Wallet wallet){
        long ammount = 0;
        foreach (var effect in suiTransactionBlockResponse.balanceChanges)
        {
            if(effect.owner.AddressOwner == wallet.publicKey)
            {
                ammount = long.Parse(effect.amount);
            }
        }
        return ammount;
    }

    public static long GetAmountFromInputs(SuiTransactionBlockResponse suiTransactionBlockResponse){
        long ammount = 0;
        foreach(var input in suiTransactionBlockResponse.transaction.data.transaction.inputs)
        {
            if(input.valueType == "u64")
            {
                ammount = long.Parse((string)input.value);
            }
        }
        return ammount;
    }

    public static string GetCoinTypeFromObjectChanges(SuiTransactionBlockResponse suiTransactionBlockResponse, Wallet wallet) {
        string coinType = "";
        foreach (var objectChange in suiTransactionBlockResponse.objectChanges)
        {
            Debug.Log(objectChange.owner.AddressOwner + " " + objectChange.type + " " + objectChange.objectType + " " + wallet.publicKey);            
            coinType = objectChange.objectType;
            if(!coinType.Contains(SUIConstantVars.suiCoinType))
            {
                break;
            }
        }

        if(!string.IsNullOrEmpty(coinType))
        {
            coinType = coinType.Replace("0x2::coin::Coin<", "");
            coinType = coinType.Replace(">", "");
        }

        return coinType;
    }

    public static string GetTypeFromBalanceChanges(SuiTransactionBlockResponse suiTransactionBlockResponse, Wallet wallet){
        string type = "";
        foreach (var effect in suiTransactionBlockResponse.balanceChanges)
        {
            if(effect.owner.AddressOwner == wallet.publicKey)
            {
                type = effect.coinType;
            }
        }

        return type;
    }

    public static string GetReceiver(SuiTransactionBlockResponse suiTransactionBlockResponse)
    {
        foreach (var input in suiTransactionBlockResponse.transaction.data.transaction.inputs)
        {
            if(input.valueType == "address")
            {
                return (string)input.value;
            }
        }
        return "Unknown Receiver";
    }

    public static decimal CalculateGasUsed(SuiTransactionBlockResponse suiTransactionBlockResponse)
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

    public static DateTimeOffset GetDateTimeFromBlock(SuiTransactionBlockResponse suiTransactionBlockResponse)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds((long)ulong.Parse(suiTransactionBlockResponse.timestampMs));
    }
}