using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PriceChangeText : MonoBehaviour
{
    public TextMeshProUGUI textField;
    public string positive = "#78B51C";

    public string negative = "#E12329";


    //convert hex to Color
    public static Color HexToColor(string hex)
    {
        Color color = new Color();
        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }

    public void SetText(string text)
    {
        textField = GetComponent<TextMeshProUGUI>();

        if(string.IsNullOrEmpty(text))
            return;

        if(text.Contains("-"))
            this.textField.color = HexToColor(negative);
        else
            this.textField.color = HexToColor(positive);

        this.textField.text = text.Contains("-") ? text : $"+{text}";
    }
}
