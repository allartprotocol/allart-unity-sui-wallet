using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WalletUtility
{
     public static string ShortenAddress(string address)
        {
            return address[..6] + "..." + address[^4..];
        }

}
