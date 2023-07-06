using SimpleScreen;
using UnityEngine.UI;

public class CreationSuccessScreen : BaseScreen
{
    public Button continueBtn;

    // Start is called before the first frame update
    void Start()
    {
        continueBtn.onClick.AddListener(OnContinue);
    }

    private void OnContinue()
    {
        if (string.IsNullOrEmpty(WalletComponent.Instance.password))
        {
            GoTo("LoginScreen");
            return;
        }
        GoTo("MainScreen");
    }

}
