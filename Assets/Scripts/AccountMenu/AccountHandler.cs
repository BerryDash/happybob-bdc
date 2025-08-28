using UnityEngine;

public class AccountHandler : MonoBehaviour
{
    public static AccountHandler instance;
    public AccountLoggedIn accountLoggedIn;
    public AccountLoggedOut accountLoggedOut;
    public AccountLogin accountLogin;
    public AccountRegister accountRegister;
    public AccountChangeUsername accountChangeUsername;
    public AccountChangePassword accountChangePassword;
    public AccountRefreshLogin accountRefreshLogin;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (BazookaManager.Instance.GetAccountID() != null && BazookaManager.Instance.GetAccountName() != null && BazookaManager.Instance.GetAccountSession() != null)
        {
            SwitchPanel(0);
        }
        else
        {
            SwitchPanel(1);
        }
    }

    public void SwitchPanel(int panel)
    {
        switch (panel)
        {
            case 0:
                accountLoggedIn.gameObject.SetActive(true);
                accountLoggedOut.gameObject.SetActive(false);
                accountLogin.gameObject.SetActive(false);
                accountRegister.gameObject.SetActive(false);
                accountChangeUsername.gameObject.SetActive(false);
                accountChangePassword.gameObject.SetActive(false);
                accountRefreshLogin.gameObject.SetActive(false);
                break;
            case 1:
                accountLoggedIn.gameObject.SetActive(false);
                accountLoggedOut.gameObject.SetActive(true);
                accountLogin.gameObject.SetActive(false);
                accountRegister.gameObject.SetActive(false);
                accountChangeUsername.gameObject.SetActive(false);
                accountChangePassword.gameObject.SetActive(false);
                accountRefreshLogin.gameObject.SetActive(false);
                break;
            case 2:
                accountLoggedIn.gameObject.SetActive(false);
                accountLoggedOut.gameObject.SetActive(false);
                accountLogin.gameObject.SetActive(true);
                accountRegister.gameObject.SetActive(false);
                accountChangeUsername.gameObject.SetActive(false);
                accountChangePassword.gameObject.SetActive(false);
                accountRefreshLogin.gameObject.SetActive(false);
                break;
            case 3:
                accountLoggedIn.gameObject.SetActive(false);
                accountLoggedOut.gameObject.SetActive(false);
                accountLogin.gameObject.SetActive(false);
                accountRegister.gameObject.SetActive(true);
                accountChangeUsername.gameObject.SetActive(false);
                accountChangePassword.gameObject.SetActive(false);
                accountRefreshLogin.gameObject.SetActive(false);
                break;
            case 4:
                accountLoggedIn.gameObject.SetActive(false);
                accountLoggedOut.gameObject.SetActive(false);
                accountLogin.gameObject.SetActive(false);
                accountRegister.gameObject.SetActive(false);
                accountChangeUsername.gameObject.SetActive(true);
                accountChangePassword.gameObject.SetActive(false);
                accountRefreshLogin.gameObject.SetActive(false);
                break;
            case 5:
                accountLoggedIn.gameObject.SetActive(false);
                accountLoggedOut.gameObject.SetActive(false);
                accountLogin.gameObject.SetActive(false);
                accountRegister.gameObject.SetActive(false);
                accountChangeUsername.gameObject.SetActive(false);
                accountChangePassword.gameObject.SetActive(true);
                accountRefreshLogin.gameObject.SetActive(false);
                break;
            case 6:
                accountLoggedIn.gameObject.SetActive(false);
                accountLoggedOut.gameObject.SetActive(false);
                accountLogin.gameObject.SetActive(false);
                accountRegister.gameObject.SetActive(false);
                accountChangeUsername.gameObject.SetActive(false);
                accountChangePassword.gameObject.SetActive(false);
                accountRefreshLogin.gameObject.SetActive(true);
                break;
        }
    }
}