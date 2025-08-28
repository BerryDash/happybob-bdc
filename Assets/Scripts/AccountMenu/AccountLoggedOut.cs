using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AccountLoggedOut : MonoBehaviour
{
    public Button loggedOutLoginButton;
    public Button loggedOutRegisterButton;
    public Button loggedOutBackButton;

    void Awake()
    {
        loggedOutLoginButton.onClick.AddListener(() => AccountHandler.instance.SwitchPanel(2));
        loggedOutRegisterButton.onClick.AddListener(() => AccountHandler.instance.SwitchPanel(3));
        loggedOutBackButton.onClick.AddListener(async () => await SceneManager.LoadSceneAsync("MainMenu"));
    }
}