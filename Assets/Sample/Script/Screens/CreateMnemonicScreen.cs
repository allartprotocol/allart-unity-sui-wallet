using AllArt.SUI.Wallet;
using SimpleScreen;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateMnemonicScreen : BaseScreen
{
    public List<WordComponent> words = new List<WordComponent>();

    public Button continueBtn;
    public Button copyBtn;

    public Toggle saveConfirmationToggle;
    private string mnemonic;

    // Start is called before the first frame update
    void Start()
    {
        continueBtn.onClick.AddListener(OnContinue);
        copyBtn.onClick.AddListener(OnCopy);
    }

    private void OnCopy()
    {

    }

    private void OnContinue()
    {

    }

    private void PopulateWords() { 
        mnemonic = Mnemonics.GenerateNewMnemonic();
        string[] words = mnemonic.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            this.words[i].SetData(i + 1, words[i]);
        }
    }

    public override void InitScreen()
    {
        base.InitScreen();
    }

    public override void ShowScreen(object data = null)
    {
        base.ShowScreen(data);
        PopulateWords();
    }

    public override void HideScreen()
    {
        base.HideScreen();
    }
}
