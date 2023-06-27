using System.Collections.Generic;
using UnityEngine;

namespace SimpleScreen { 
    public class SimpleScreenManager : MonoBehaviour
    {
        public BaseScreen[] screens;
        private Dictionary<string, BaseScreen> screensDict = new Dictionary<string, BaseScreen>();
        public BaseScreen currentScreen;
        public BaseScreen previousScreen;

        public static SimpleScreenManager instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        private void Start()
        {
            PopulateDictionary();
        }

        private void PopulateDictionary()
        {
            if (screens != null && screens.Length > 0)
            {
                int i = 0;
                foreach (BaseScreen screen in screens)
                {
                    SetupScreen(screen, !(i == 0));
                    i++;
                }


                currentScreen = screens[0];
                screens[0].ShowScreen();
            }
        }

        private void SetupScreen(BaseScreen screen, bool hide = true)
        {
            screen.manager = this;
            if(hide)
                screen.InitScreen();//gameObject.SetActive(false);
            screensDict.Add(screen.gameObject.name, screen);
        }

        public void ShowScreen(BaseScreen curScreen, BaseScreen screen)
        {
            curScreen?.HideScreen();
            screen.ShowScreen();
            previousScreen = curScreen;
            currentScreen = screen;
        }

        public void ShowScreen(string name, object data = null)
        {
            currentScreen?.HideScreen();
            screensDict[name].ShowScreen(data);
            previousScreen = currentScreen;
            currentScreen = screensDict[name];
        }

        public void ShowScreen(BaseScreen curScreen, int index)
        {
            curScreen?.HideScreen();
            screens[index].ShowScreen();
            previousScreen = curScreen;
            currentScreen = screens[index];
        }

        public void ShowScreen(BaseScreen curScreen, string name, object data = null)
        {
            curScreen?.HideScreen();
            screensDict[name].ShowScreen(data);
            previousScreen = curScreen;
            currentScreen = screensDict[name];
        }

        public void HideScreen(string name) {
            screensDict[name].HideScreen();
            currentScreen = null;
        }

        public void HideAll(int screenIndex = 0) {
            foreach (BaseScreen screen in screens)
            {
                screen.HideScreen();
            }
            screens[screenIndex].ShowScreen();
        }
    }
}
