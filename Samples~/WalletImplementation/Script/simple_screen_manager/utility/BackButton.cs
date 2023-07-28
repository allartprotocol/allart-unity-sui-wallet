using SimpleScreen;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    private Button btn;
    private SimpleScreenManager screenManager;

    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnBack);
    }

    private void OnBack()
    {
        screenManager.GoBack();
    }

    public void Init(SimpleScreenManager screenManager)
    {
        this.screenManager = screenManager;
    }
}
