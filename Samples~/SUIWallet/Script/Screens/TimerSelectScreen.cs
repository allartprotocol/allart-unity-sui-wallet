using System;
using System.Collections;
using System.Collections.Generic;
using SimpleScreen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class TimerSelectScreen : BaseScreen {

    public List<Button> buttons;
    public List<string> labels = new List<string>();
    public List<float> times = new List<float>();

    public override void ShowScreen(object data = null){
        base.ShowScreen(data);
        
        for (int i = 0; i < buttons.Count; i++){
            buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = labels[i];
        }

        if(PlayerPrefs.HasKey("timeout")){
            float timer = PlayerPrefs.GetFloat("timeout");
            for (int i = 0; i < times.Count; i++){
                if(times[i] == timer){
                    buttons[i].transform.Find("check").gameObject.SetActive(true);
                }
                else{
                    buttons[i].transform.Find("check").gameObject.SetActive(false);
                }
            }
        }
        else{
            foreach (var button in buttons)
            {
                button.transform.Find("check").gameObject.SetActive(false);
            }
        }
    }

    public void OnClick(int i)
    {
        Debug.Log(i);
        PlayerPrefs.SetFloat("timeout", times[i]);
        InfoPopupManager.instance.AddNotif(InfoPopupManager.InfoType.Info, "Auto-lock timer updated");
        WalletComponent.Instance.StartTimer();
        manager.GoBack();
    }
}
