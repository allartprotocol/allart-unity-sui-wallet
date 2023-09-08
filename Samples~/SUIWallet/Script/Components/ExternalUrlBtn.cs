using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExternalUrlBtn : MonoBehaviour
{
    public string url;
    private Button  btn;

    // Start is called before the first frame update
    void Start()
    {
        
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnBtnClick);

    }

    private void OnBtnClick()
    {
        //open url in browser
        Application.OpenURL(url);
    }

}
