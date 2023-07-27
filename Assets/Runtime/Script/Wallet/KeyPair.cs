using Chaos.NaCl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace AllArt.SUI.Wallets
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
                byte[] first32Bytes = new byte[32];
                Array.Copy(privateKey, 0, first32Bytes, 0, 32);
                var hash = BitConverter.ToString(first32Bytes).Replace("-", "").ToLowerInvariant();
                return hash[..64];
            }
        }

        public static bool IsSuiAddressInCorrectFormat(string address){
            if(address.Length != 66){
                return false;
            }
            if(!address.StartsWith("0x")){
                return false;
            }
            return true;
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


        public static byte[] GetPrivateKeyFromSuiSecret(string suiSecret)
        {
            suiSecret = suiSecret.Replace("0x", "");
            byte[] hashBytes = new byte[32];
            for (int i = 0; i < 32; i++)
            {
                hashBytes[i] = Convert.ToByte(suiSecret.Substring(i * 2, 2), 16);
            }

            byte[] privateKey = new byte[64];
            Array.Copy(hashBytes, 0, privateKey, 0, 32);

            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(privateKey, 32, 32);
            }

            return privateKey;
        }

        internal static KeyPair GenerateKeyPairFromPrivateKey(string privateKey)
        {
            privateKey = privateKey.Replace("0x", "");
            byte[] privateKeyBytes = CryptoBytes.FromHexString(privateKey);
            byte[] publicKey = Ed25519.PublicKeyFromSeed(privateKeyBytes);

            return new KeyPair(publicKey, privateKeyBytes);
        }
    }

}
