using Chaos.NaCl;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllArt.SUI.Wallet
{
    public class KeyPair { 

        public byte[] publicKey { get; set; }
        public byte[] privateKey { get; set; }

        public string publicKeyString { get; set; }
        public string suiAddress { get; set; }

        public string suiSecret
        {
            get
            {
                return ToSuiAddress(privateKey);
            }
        }

        public KeyPair(byte[] publicKey, byte[] privateKey)
        {

            this.publicKey = publicKey;
            this.privateKey = privateKey;
            this.publicKeyString = CryptoBytes.ToBase64String(publicKey);
            this.suiAddress = ToSuiAddress(publicKey);
        }

        public string ToSuiAddress(byte[] publicKeyBytes)
        {
            var hashAlgorithm = new Org.BouncyCastle.Crypto.Digests.Blake2bDigest(256);
            var addressBytes = new byte[publicKeyBytes.Length + 1];
            addressBytes[0] = 0x00;

            Array.Copy(publicKeyBytes, 0, addressBytes, 1, publicKeyBytes.Length);
            hashAlgorithm.BlockUpdate(addressBytes, 0, addressBytes.Length);

            byte[] result = new byte[64];
            hashAlgorithm.DoFinal(result, 0);

            string hashString = BitConverter.ToString(result);
            hashString = hashString.Replace("-", "").ToLowerInvariant();
            return "0x" + hashString.Substring(0, 64);
        }
    }

}
