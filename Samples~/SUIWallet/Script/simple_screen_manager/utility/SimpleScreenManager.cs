using System.Collections.Generic;
using Org.BouncyCastle.Asn1.X509;
using UnityEngine;

namespace SimpleScreen { 
    public class SimpleScreenManager : MonoBehaviour
    {
        public BaseScreen[] screens;
        private Dictionary<string, BaseScreen> screensDict = new Dictionary<string, BaseScreen>();
        [HideInInspector]
        public BaseScreen currentScreen;
        [HideInInspector]
        public BaseScreen previousScreen;

        public static SimpleScreenManager instance;

        private Stack<BaseScreen> _screenQueue = new Stack<BaseScreen>();        

        public int historyCount => _screenQueue.Count;
        public Transform mainHolder;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        public void Hide(){
            mainHolder?.gameObject.SetActive(false);
        }

        public void Show(){
            mainHolder?.gameObject.SetActive(true);
        }

        public void ToggleAllScreens()
        {
            mainHolder?.gameObject.SetActive(!mainHolder.gameObject.activeSelf);
        }

        public void GoBack() { 
            if(_screenQueue.Count == 0)
            {
                return;
            }
            var prevScreen = _screenQueue.Pop();
            var curScreen = _screenQueue.Peek();
            var previousScreen = _screenQueue.Count > 1 ? _screenQueue.ToArray()[1] : null;

            if(prevScreen != null)
                prevScreen.HideScreen();
            curScreen.ShowScreen();
            this.previousScreen = previousScreen;
            currentScreen = curScreen;
        }

        private void Start()
        {
            PopulateDictionary();
        }

        public void ClearHistory(BaseScreen baseScreen)
        {
            _screenQueue.Clear();
            if(baseScreen != null)
                _screenQueue.Push(baseScreen);
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
                _screenQueue.Push(currentScreen);
                screens[0].ShowScreen();
            }
        }

        private void SetupScreen(BaseScreen screen, bool hide = true)
        {
            screen.manager = this;
            if(hide)
                screen.InitScreen();
            screensDict.Add(screen.gameObject.name, screen);
        }

        public void ShowScreen(BaseScreen curScreen, BaseScreen screen)
        {
            previousScreen = curScreen;
            currentScreen = screen;
            _screenQueue.Push(currentScreen);
            curScreen?.HideScreen();
            screen.ShowScreen();
        }

        public void ShowScreen(string name)
        {
            currentScreen?.HideScreen();
            previousScreen = currentScreen;
            currentScreen = screensDict[name];
            _screenQueue.Push(currentScreen);
            screensDict[name].ShowScreen(null);
        }

        public void ShowScreen(string name, object data = default)
        {
            currentScreen?.HideScreen();
            previousScreen = currentScreen;
            currentScreen = screensDict[name];
            _screenQueue.Push(currentScreen);
            screensDict[name].ShowScreen(data);
        }

        public void ShowScreen(BaseScreen curScreen, int index)
        {
            curScreen?.HideScreen();
            previousScreen = curScreen;
            currentScreen = screens[index];
            _screenQueue.Push(currentScreen);
            screens[index].ShowScreen();
        }

        public void ShowScreen(BaseScreen curScreen, string name, object data = null)
        {
            curScreen?.HideScreen();
            previousScreen = curScreen;
            currentScreen = screensDict[name];
            _screenQueue.Push(currentScreen);
            screensDict[name].ShowScreen(data);
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
