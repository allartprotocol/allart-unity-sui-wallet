using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CopyButton : MonoBehaviour
{
    public TextMeshProUGUI text;
    private string textValue;
    private Button btn;
    public void Copy()
    {
        GUIUtility.systemCopyBuffer = textValue;
        InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Info, "Copied to clipboard");
    }
    
    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(Copy);
    }

    public void SetText(string text, string displayText)
    {
        this.textValue = text;
        this.text.text = displayText;
    }
}
