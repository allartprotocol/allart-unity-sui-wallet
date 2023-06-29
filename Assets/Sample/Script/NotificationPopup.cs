using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NotificationPopup : MonoBehaviour
{
    public TextMeshProUGUI titleText;

    internal void SetInfo(Color color, string message)
    {
        titleText.text = message;
        GetComponentInChildren<Image>().color = color;
        //GetComponentInChildren<UITween>().TweenPosition(Vector2.zero);
        SetDestroyTimer();
    }

    void SetDestroyTimer() { 
        StartCoroutine(DestroyAfter());
    }

    IEnumerator DestroyAfter() {
        yield return new WaitForSeconds(4);
        //GetComponentInChildren<UITween>().TweenPosition(GetComponentInChildren<UITween>().startPosition);
        //yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
