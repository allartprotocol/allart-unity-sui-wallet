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

    // public static string FormatDecimal(decimal number)
    // {
    //     string formattedString = number.ToString("N7", CultureInfo.InvariantCulture);
    //     Debug.Log(formattedString);
    //     if (formattedString.Length <= 7)
    //     {
    //         return formattedString;
    //     }

    //     // Checking if the number has many zeros after the period followed by a non-zero digit
    //     var match = System.Text.RegularExpressions.Regex.Match(formattedString, @"\.\d*0{3,}\d");
    //     if (match.Success)
    //     {
    //         Debug.Log("Matched: " + match.Value);
    //         // display full number wihout E notation
    //         return number.ToString("0.###############");
            
    //     }

    //     return formattedString.Substring(0, 8);
    // }

    public static string FormatDecimal(decimal number)
    {
        string strNumber = number.ToString("0.################"); // Convert number to string without E notation
        
        if (strNumber.Length <= 7)
        {
            return strNumber;
        }

        int periodIndex = strNumber.IndexOf('.');
        if (periodIndex != -1) // if there's a decimal point
        {
            int firstNonZeroAfterPeriod = strNumber.Substring(periodIndex).IndexOfAny(new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' });
            Debug.Log(firstNonZeroAfterPeriod);
            if (firstNonZeroAfterPeriod != -1 && firstNonZeroAfterPeriod + periodIndex + 1 >= 7)
            {
                // Taking all characters up to the first non-zero decimal
                return strNumber.Substring(0, firstNonZeroAfterPeriod + periodIndex + 1);
            }
        }

        Debug.Log("No decimal point or no non-zero digit after decimal point");

        // If we reached here, we just limit the number to 7 characters
        return RemoveTrailingZeroes(strNumber.Substring(0, 8));
    }

    public static string RemoveTrailingZeroes(string strNumber)
    {
        
        if (strNumber.Contains('.'))
        {
            strNumber = strNumber.TrimEnd('0');  // Remove trailing zeroes
            strNumber = strNumber.TrimEnd('.');  // Remove trailing period, if any
        }
        
        return strNumber;
    }

    public static string ParseDecimalValueToString<T>(T value, bool overrideMinimum = false) where T : struct, IConvertible
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

        // if decimal value is less than 0.000001, return 0
        if (decimalValue < 0.000001m && !overrideMinimum)
        {
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
            return FormatDecimal(decimalValue);
            // string decimalString = decimalValue.ToString("G7", CultureInfo.InvariantCulture);
            // return decimalString.Length > 8 ? decimalValue.ToString("G7", CultureInfo.InvariantCulture).Substring(0, 8) : decimalString;
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
