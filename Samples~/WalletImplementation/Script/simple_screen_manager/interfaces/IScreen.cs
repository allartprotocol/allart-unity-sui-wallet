using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleScreen { 
    public interface IScreen 
    {
        SimpleScreenManager manager { get; set; }
        void ShowScreen(object data = null);
        void HideScreen();    
    }

}
