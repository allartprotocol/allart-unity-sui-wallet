using AllArt.SUI.RPC.Response;
using AllArt.SUI.Wallets;
using SimpleScreen;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TransactionDoneScreen : BaseScreen
{
    public Button done;
    public Button viewTransactionbtn;
    public Image statusImage;

    public Sprite success;
    public Sprite fail;

    public TextMeshProUGUI statusText;
    public TextMeshProUGUI messageText;

    private TransferData status;

    // Start is called before the first frame update
    void Start()
    {
        done.onClick.AddListener(OnDone);
        viewTransactionbtn.onClick.AddListener(OnViewTransaction);
    }

    private void OnViewTransaction()
    {
        if(status == null)
        {
            return;
        }
        string network = WalletComponent.Instance.nodeType switch
        {
            ENodeType.MainNet => "mainnet",
            ENodeType.TestNet => "testnet",
            _ => "devnet",
        };
        //https://suiscan.xyz/mainnet/tx/AN9KDjQEbYVdUAnuhC3HLFguDnWkkqJkx71MGTxRMMdb
        //open browser with the transaction hash
        Application.OpenURL($"https://suiscan.xyz/{network}/tx/{status.response.result.digest}");
    }

    private void OnDone()
    {
        GoTo("MainScreen");
    }

    public override void ShowScreen(object data = null)
    {
        base.ShowScreen(data);
        status = (TransferData)data;
        viewTransactionbtn.gameObject.SetActive(true);
        try{
            if(status == null)
            {
                statusText.text = "Failed";
                statusImage.sprite = fail;
                // messageText.text = $"An error occured while trying to send {status.amount} {status.coin.symbol} to {status.to}";
                viewTransactionbtn.gameObject.SetActive(false);
                return;
            }
            else
            {
                if(status.response.result.effects.status.status == "failure")
                {
                    statusImage.sprite = fail;
                    statusText.text = "Failed";
                    messageText.text = $"An error occured while trying to send {status.amount} {status.coin.symbol} to {Wallet.DisplaySuiAddress(status.to)}";
                    return;
                }
                else{
                    statusImage.sprite = success;
                    statusText.text = "Sent";
                    messageText.text = $"{status.amount} {status.coin.symbol} was successfully sent to {Wallet.DisplaySuiAddress(status.to)}";
                }                
            }
        }
        catch(Exception e)
        {
            statusImage.sprite = fail;
            statusText.text = "Failed";
            messageText.text = $"An error occured while trying to send {status.amount} {status.coin.symbol} to {Wallet.DisplaySuiAddress(status.to)}";
        }
        
    }

}
