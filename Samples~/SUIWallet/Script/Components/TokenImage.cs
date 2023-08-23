using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TokenImage : MonoBehaviour
{
    public Image tokenImage;
    public TextMeshProUGUI tokenName;
    public Mask tokenBackground;
    // Start is called before the first frame update
    
    public void Init(Sprite image, string name){
        tokenBackground = GetComponent<Mask>();
        if(image == null)
        {
            tokenImage.gameObject.SetActive(false);
            tokenBackground.showMaskGraphic = true;
            return;
        }
        tokenBackground.showMaskGraphic = false;
        tokenImage.gameObject.SetActive(true);
        tokenImage.sprite = image;
    }
}
