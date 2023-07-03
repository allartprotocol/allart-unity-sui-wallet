﻿using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace SimpleScreen
{
    public class BaseScreen : MonoBehaviour, IScreen
    {
        private SimpleScreenManager _manager;
        protected CanvasGroup _canvasGroup;

        public SimpleScreenManager manager
        {
            get { return _manager; }
            set
            {
                _manager = value;
                tween = (IScreenAnimation)GetComponent(typeof(IScreenAnimation));
                _canvasGroup = GetComponent<CanvasGroup>();
            }
        }

        public void GoTo(string page, object data = null)
        {
            manager.ShowScreen(this, page, data);
        }

        public IScreenAnimation tween { get; set; }

        private void Awake()
        {
            tween = (IScreenAnimation)GetComponent(typeof(IScreenAnimation));
        }

        public virtual void HideScreen()
        {
            if (tween != null)
            {
                tween.TweenOut();
                if (_canvasGroup != null)
                {
                    _canvasGroup.interactable = false;
                    _canvasGroup.blocksRaycasts = false;
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public virtual void InitScreen()
        {
            if (tween != null)
            {
                tween.PlayInstant();
                if (_canvasGroup != null)
                {
                    _canvasGroup.interactable = false;
                    _canvasGroup.blocksRaycasts = false;
                }
            }
            else
            {
                gameObject.SetActive(false);
            }

            var backButton = GetComponentInChildren<BackButton>();
            if (backButton != null)
            {
                backButton.Init(manager);
            }
        }

        public virtual void ShowScreen(object data = null)
        {
            if (tween != null)
            {
                tween.TweenIn();
                if (_canvasGroup != null)
                {
                    _canvasGroup.interactable = true;
                    _canvasGroup.blocksRaycasts = true;
                }
            }
            else { gameObject.SetActive(true); }
        }

        internal T GetInputByName<T>(string name) where T : Object
        {
            var inputs = GetComponentsInChildren<T>();
            return inputs.First(e => e.name == name);
        }
    }
}
