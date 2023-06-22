using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WordComponent : MonoBehaviour
{
    public TextMeshProUGUI index;
    public TextMeshProUGUI word;

    public void SetData(int index, string word)
    {
        this.index.text = index.ToString();
        this.word.text = word;
    }

    public void SetData(string index, string word)
    {
        this.index.text = index;
        this.word.text = word;
    }

    public void Clear() { 
        this.word.text = "";
    }

    public void SetIndex(int index)
    {
        this.index.text = index.ToString();
    }
}
