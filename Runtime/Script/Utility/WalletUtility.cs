using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WalletUtility
{
    /// <summary>
    /// Shortens the given SUI address by replacing the middle part with ellipsis.
    /// </summary>
    /// <param name="address">The SUI address to shorten.</param>
    /// <returns>The shortened SUI address.</returns>
     public static string ShortenAddress(string address)
    {
        return address[..6] + "..." + address[^4..];
    }
}
