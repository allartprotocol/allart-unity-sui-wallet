using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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

    public static string ParseDecimalValueToString<T>(T value) where T : struct, IConvertible
    {
        decimal decimalValue;

        try
        {
            decimalValue = Convert.ToDecimal(value);
        }
        catch (Exception)
        {
            return "0";
        }

        if(decimalValue == 0 || Math.Abs(decimalValue) < 0.000001m)
        {
            Debug.Log(decimalValue + " " + (decimalValue < 0.000001m)) ;
            return "0";
        }

        if (decimalValue >= 1000000000) // billion
        {
            return $"{(decimalValue / 1000000000):N1}B";
        }
        else if (decimalValue >= 1000000) // million
        {
            return $"{(decimalValue / 1000000):N1}M";
        }
        else
        {
            string decimalString = decimalValue.ToString("G7", CultureInfo.InvariantCulture);
            return decimalString.Length > 8 ? decimalValue.ToString("G7", CultureInfo.InvariantCulture).Substring(0, 8) : decimalString;
        }
    }

    // decimal string value parsing
    public static decimal ParseDecimalValueFromString(string value)
    {
        decimal decimalValue;

        try
        {
            decimalValue = Convert.ToDecimal(value);
        }
        catch (Exception)
        {
            return 0;
        }

        return decimalValue;
    }
}
