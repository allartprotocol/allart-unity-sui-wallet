using SimpleScreen;
using TMPro;
using UnityEngine.UI;

public class CreationSuccessScreen : BaseScreen
{
    public Button continueBtn;
    public TextMeshProUGUI messageText;

    // Start is called before the first frame update
    void Start()
    {
        continueBtn.onClick.AddListener(OnContinue);
    }

    override public void ShowScreen(object data = null)
    {
        base.ShowScreen(data);

        if (data is not string message)
        {
            messageText.text = "Wallet Imported Successfully!";
            return;
        }

        messageText.text = message;
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
