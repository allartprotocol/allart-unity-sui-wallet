using SimpleScreen;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkSelectScreen : BaseScreen
{
    public Button testNet;
    public Button mainNet;
    public Button devNet;

    // Start is called before the first frame update
    void Start()
    {
        testNet.onClick.AddListener(OnTestNet);
        mainNet.onClick.AddListener(OnMainNet);
        devNet.onClick.AddListener(OnDevNet);
    }

    private void CheckSelection() {
        switch (WalletComponent.Instance.nodeType)
        {
            case ENodeType.TestNet:
                testNet.transform.Find("check").gameObject.SetActive(true);
                mainNet.transform.Find("check").gameObject.SetActive(false);
                devNet.transform.Find("check").gameObject.SetActive(false);
                break;
            case ENodeType.MainNet:
                testNet.transform.Find("check").gameObject.SetActive(false);
                mainNet.transform.Find("check").gameObject.SetActive(true);
                devNet.transform.Find("check").gameObject.SetActive(false);
                break;
            case ENodeType.DevNet:
                testNet.transform.Find("check").gameObject.SetActive(false);
                mainNet.transform.Find("check").gameObject.SetActive(false);
                devNet.transform.Find("check").gameObject.SetActive(true);
                break;
        }
    }

    override public void ShowScreen(object data = null)
    {
        base.ShowScreen(data);
        CheckSelection();
    }

    private void OnTestNet()
    {
        WalletComponent.Instance.SetNodeAddress(ENodeType.TestNet);
        manager.GoBack();
    }

    private void OnMainNet()
    {
        WalletComponent.Instance.SetNodeAddress(ENodeType.MainNet);
        manager.GoBack();
    }

    private void OnDevNet()
    {
        WalletComponent.Instance.SetNodeAddress(ENodeType.DevNet);
        manager.GoBack();
    }
}
