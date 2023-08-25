using SimpleScreen;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    private Button btn;
    private SimpleScreenManager screenManager;

    void Start()
    {
        if(btn != null)
        {
            return;
        }
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
        if(btn == null)
        {
            btn = GetComponent<Button>();
            btn.onClick.AddListener(OnBack);

        }
    }
}
