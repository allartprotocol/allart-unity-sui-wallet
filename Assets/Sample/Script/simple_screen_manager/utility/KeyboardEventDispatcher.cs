using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardEventDispatcher : MonoBehaviour
{

    public Action onEnterPressed;
    public static KeyboardEventDispatcher instance;

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            onEnterPressed?.Invoke();
        }    
    }
}
