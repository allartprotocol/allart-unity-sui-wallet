using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CopyButton : MonoBehaviour
{
    public TextMeshProUGUI text;
    private Button btn;
    public void Copy()
    {
        GUIUtility.systemCopyBuffer = text.text;
        InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Info, "Copied to clipboard");
    }
    
    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(Copy);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
