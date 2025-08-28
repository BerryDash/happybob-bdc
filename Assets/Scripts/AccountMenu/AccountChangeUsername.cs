using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AccountChangeUsername : MonoBehaviour
{
    public TMP_Text changeUsernameStatusText;
    public TMP_InputField changeUsernameCurrentUsernameInput;
    public TMP_InputField changeUsernameNewUsernameInput;
    public Button changeUsernameBackButton;
    public Button changeUsernameSubmitButton;

    void Awake()
    {
        changeUsernameBackButton.onClick.AddListener(() => AccountHandler.instance.SwitchPanel(0));
        changeUsernameSubmitButton.onClick.AddListener(() => ChangeUsername());
    }

    void OnEnable()
    {
        changeUsernameCurrentUsernameInput.text = "";
        changeUsernameNewUsernameInput.text = "";
        changeUsernameStatusText.text = "";
    }

    async void ChangeUsername()
    {
        changeUsernameBackButton.interactable = false;
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("oldusername", changeUsernameCurrentUsernameInput.text);
        dataForm.AddField("newusername", changeUsernameNewUsernameInput.text);
        dataForm.AddField("token", BazookaManager.Instance.GetAccountSession());
        dataForm.AddField("username", BazookaManager.Instance.GetAccountName());
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "changeAccountUsername.php", dataForm.form);
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            changeUsernameBackButton.interactable = true;
            Tools.UpdateStatusText(changeUsernameStatusText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
        if (response == "-999")
        {
            changeUsernameBackButton.interactable = true;
            Tools.UpdateStatusText(changeUsernameStatusText, "Server error while fetching data", Color.red);
            return;
        }
        else if (response == "-998")
        {
            changeUsernameBackButton.interactable = true;
            Tools.UpdateStatusText(changeUsernameStatusText, "Client version too outdated to access servers", Color.red);
            return;
        }
        else if (response == "-997")
        {
            changeUsernameBackButton.interactable = true;
            Tools.UpdateStatusText(changeUsernameStatusText, "Encryption/decryption issues", Color.red);
            return;
        }
        else if (response == "-996")
        {
            changeUsernameBackButton.interactable = true;
            Tools.UpdateStatusText(changeUsernameStatusText, "Can't send requests on self-built instance", Color.red);
            return;
        }
        else
        {
            var jsonResponse = JObject.Parse(response);
            if ((bool)jsonResponse["success"])
            {
                BazookaManager.Instance.SetAccountName(changeUsernameNewUsernameInput.text);
                AccountHandler.instance.SwitchPanel(0);
                Tools.UpdateStatusText(AccountHandler.instance.accountLoggedIn.loggedInText, "Username changed successfully", Color.green);
            }
            else
            {
                Tools.UpdateStatusText(changeUsernameStatusText, (string)jsonResponse["message"], Color.red);
            }
        }
        changeUsernameBackButton.interactable = true;
    }
}