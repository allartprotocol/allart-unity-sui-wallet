using AllArt.SUI.Extensions;
using Chaos.NaCl;
using dotnetstandard_bip39;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace AllArt.SUI.Wallet { 
    public static class Mnemonics
    {
        private const uint HardenedOffset = 0x80000000;
        private const string Curve = "ed25519 seed";
        public static string derivePath = "m/44'/784'/0'/0'/0'";

        public static string GenerateNewMnemonic()
        {
            BIP39 p = new BIP39();
            string mnemonic = p.GenerateMnemonic(128, BIP39Wordlist.English);
            return mnemonic;
        }

        public static bool CheckMnemonicValidity(string mnemonic)
        {
            string[] mnemonicWords = mnemonic.Split(' ');
            if (mnemonicWords.Length == 12 || mnemonicWords.Length == 24)
                return true;
            return false;
        }

        public static byte[] StringToByteArrayFastest(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

        public static byte[] GetBIP39SeedBytes(string seed)
        {
            return StringToByteArrayFastest(MnemonicToSeedHex(seed));
        }

        public static string MnemonicToSeedHex(string seed)
        {

            BIP39 p = new BIP39();
            return p.MnemonicToSeedHex(seed, string.Empty);
        }

        public static byte[] GenerateSeedFromMnemonic(string mnemonic)
        {
            return GetBIP39SeedBytes(mnemonic);
        }

        public static (byte[] Key, byte[] ChainCode) DerivePath(string path, byte[] _masterKey, byte[] _chainCode)
        {
            //if (!IsValidPath(path))
            //    throw new FormatException("Invalid derivation path");

            var segments = path
                        .Split('/')
                        .Slice(1)
                        .Select(a => a.Replace("'", ""))
                        .Select(a => Convert.ToUInt32(a, 10));

            var results = segments
                        .Aggregate(
                            (_masterKey, _chainCode),
                            (masterKeyFromSeed, next) =>
                                GetChildKeyDerivation(masterKeyFromSeed._masterKey, masterKeyFromSeed._chainCode, next + HardenedOffset));

            return results;
        }

        private static (byte[] Key, byte[] ChainCode) GetChildKeyDerivation(byte[] key, byte[] chainCode, uint index)
        {
            using (var buffer = new MemoryStream())
            {
                buffer.Write(new byte[] { 0 }, 0, 1);
                buffer.Write(key, 0, key.Length);
                byte[] indexBytes = new byte[4];
                BinaryPrimitives.WriteUInt32BigEndian(indexBytes, index);
                buffer.Write(indexBytes, 0, indexBytes.Length);

                return HmacSha512(chainCode, buffer.ToArray());
            }
        }

        internal static (byte[] Key, byte[] ChainCode) HmacSha512(byte[] key, byte[] data)
        {
            using (var hmac = new HMACSHA512(key))
            {
                var hash = hmac.ComputeHash(data);
                return (hash.Take(32).ToArray(), hash.Skip(32).ToArray());
            }
        }

        public static KeyPair GenerateKeyPairFromMnemonic(string mnemonics)
        {
            byte[] bip39seed = GenerateSeedFromMnemonic(mnemonics);


            var publicKey = new byte[Ed25519.PublicKeySizeInBytes];
            var privateKey = new byte[Ed25519.ExpandedPrivateKeySizeInBytes];

            var hmac = HmacSha512(Encoding.UTF8.GetBytes(Curve), bip39seed);
            var derivepath = DerivePath(derivePath, hmac.Key, hmac.ChainCode);

            Ed25519.KeyPairFromSeed(publicKey, privateKey, derivepath.Key);

            return new KeyPair(publicKey, privateKey);
        }
        public static string EncryptMnemonicWithPassword(string mnemonic, string password)
        {
            return StringCipher.Encrypt(mnemonic, password);
        }

        public static string DecryptMnemonicWithPassword(string encryptedMnemonic, string password)
        {
            return StringCipher.Decrypt(encryptedMnemonic, password);
        }
    }

}
