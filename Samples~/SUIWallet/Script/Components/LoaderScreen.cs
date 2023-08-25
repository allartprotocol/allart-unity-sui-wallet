using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoaderScreen : MonoBehaviour
{
    public static LoaderScreen instance;
    public GameObject holder;

    private void Awake()
    {
        instance = this;
        holder.SetActive(false);
    }

    public TextMeshProUGUI loadingText;

    public void ShowLoading(string text)
    {
        holder.SetActive(true);
        if(string.IsNullOrEmpty(text))
            loadingText.text = "Loading...";
        else
            loadingText.text = text;
    }

    public void HideLoading()
    {
        holder.SetActive(false);
    }
}
