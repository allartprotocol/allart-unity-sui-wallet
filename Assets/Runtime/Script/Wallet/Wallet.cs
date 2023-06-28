using AllArt.SUI.Wallet;
using Chaos.NaCl;
using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Wallet
{
    public string walletName { get; private set; }
    public string mnemonic { get; private set; }
    public string publicKey
    {
        get
        {
            return keyPair.suiAddress;
        }
    }
    public string password { get; private set; }

    private KeyPair keyPair;

    public Wallet()
    {
    }

    public Wallet(string mnemonic, string password = "", string walletName = "")
    {
        this.walletName = walletName;
        this.mnemonic = mnemonic;
        this.password = password;

        keyPair = RestoreAccount(mnemonic);
    }

    public KeyPair CreateAccount()
    {
        string mnemonic = Mnemonics.GenerateNewMnemonic();
        keyPair = Mnemonics.GenerateKeyPairFromMnemonic(mnemonic);
        return keyPair;
    }

    public KeyPair RestoreAccount(string mnemonic)
    {
        keyPair = Mnemonics.GenerateKeyPairFromMnemonic(mnemonic);
        return keyPair;
    }

    public static List<string> GetWalletSavedKeys()
    {
        if (PlayerPrefs.HasKey("wallets"))
        {
            string wallets = PlayerPrefs.GetString("wallets");
            return new List<string>(wallets.Split(','));
        }
        return new List<string>();
    }

    public void SaveWallet()
    {
        List<string> wallets = GetWalletSavedKeys();

        if (string.IsNullOrEmpty(walletName))
        {
            walletName = $"wallet_{wallets.Count + 1}";
        }

        if (!wallets.Contains(walletName))
        {
            wallets.Add(walletName);
            PlayerPrefs.SetString("wallets", string.Join(",", wallets.ToArray()));
        }

        Debug.Log($"Mnemonic {mnemonic}");
        string encodedKeyPair = Mnemonics.EncryptMnemonicWithPassword(mnemonic, password);
        Debug.Log($"Encoded {encodedKeyPair}");
        PlayerPrefs.SetString(walletName, encodedKeyPair);
    }

    public static Wallet RestoreWallet(string walletName, string password)
    {
        string encodedKeyPair = PlayerPrefs.GetString(walletName);
        Debug.Log($"Encoded {encodedKeyPair}");
        string mnemonic = Mnemonics.DecryptMnemonicWithPassword(encodedKeyPair, password);
        Debug.Log($"Decoded {mnemonic}");
        if(IsValid(mnemonic))
        {
            return new Wallet(mnemonic, password, walletName);
        }
        return null;
    }

    private static bool IsValid(string mnemonic)
    {
        return Mnemonics.IsValidMnemonic(mnemonic);
    }

    internal void RemoveWallet()
    {
        List<string> wallets = GetWalletSavedKeys();
        wallets.Remove(walletName);
        PlayerPrefs.SetString("wallets", string.Join(",", wallets.ToArray()));
        PlayerPrefs.DeleteKey(walletName);
    }

    public byte[] Sign(byte[] message)
    {
        var signature = new byte[64];
        Ed25519.Sign(new ArraySegment<byte>(signature), new ArraySegment<byte>(message), new ArraySegment<byte>(keyPair.privateKey));
        return signature;
    }

    public string SignData(byte[] data)
    {
        
        var digest = ComputeBlake2bHash(data);
        var signature = Sign(digest);

        var list = new List<byte>
        {
            0x00
        };
        list.AddRange(signature);
        list.AddRange(keyPair.publicKey);

        return CryptoBytes.ToBase64String(list.ToArray());
    }

    private byte[] ComputeBlake2bHash(byte[] data)
    {
        var hashAlgorithm = new Blake2bDigest(256);
        hashAlgorithm.BlockUpdate(data, 0, data.Length);
        byte[] digest = new byte[32];
        hashAlgorithm.DoFinal(digest, 0);

        return digest;
    }

    public static byte[] GetMessageWithIntent(byte[] message)
    {
        var INTENT_BYTES = new byte[] { 0, 0, 0 };

        var messageWithIntent = new byte[INTENT_BYTES.Length + message.Length];
        Buffer.BlockCopy(INTENT_BYTES, 0, messageWithIntent, 0, INTENT_BYTES.Length);
        Buffer.BlockCopy(message, 0, messageWithIntent, INTENT_BYTES.Length, message.Length);
        return messageWithIntent;
    }
}