using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TokenImage : MonoBehaviour
{
    public Image tokenImage;
    public TextMeshProUGUI tokenName;
    // Start is called before the first frame update
    
    public void Init(Sprite image, string name){
        tokenName.text = name;
        if(image == null)
        {
            tokenImage.gameObject.SetActive(false);
            return;
        }
        tokenImage.gameObject.SetActive(true);
        tokenImage.sprite = image;
    }
}
