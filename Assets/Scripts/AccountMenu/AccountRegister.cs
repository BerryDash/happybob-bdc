using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AccountRegister : MonoBehaviour
{
    public TMP_Text registerPanelStatusText;
    public TMP_InputField registerUsernameInput;
    public TMP_InputField registerEmailInput;
    public TMP_InputField registerRetypeEmailInput;
    public TMP_InputField registerPasswordInput;
    public TMP_InputField registerRetypePasswordInput;
    public Button registerBackButton;
    public Button registerSubmitButton;

    void Awake()
    {
        registerBackButton.onClick.AddListener(() => AccountHandler.instance.SwitchPanel(1));
        registerSubmitButton.onClick.AddListener(() => SubmitRegister());
    }

    void OnEnable()
    {
        registerUsernameInput.text = "";
        registerEmailInput.text = "";
        registerRetypeEmailInput.text = "";
        registerPasswordInput.text = "";
        registerRetypePasswordInput.text = "";
        registerPanelStatusText.text = "";
    }

    async void SubmitRegister()
    {
        registerBackButton.interactable = false;
        if (
            registerUsernameInput.text == string.Empty ||
            registerEmailInput.text == string.Empty ||
            registerRetypeEmailInput.text == string.Empty ||
            registerPasswordInput.text == string.Empty ||
            registerRetypePasswordInput.text == string.Empty
        )
        {
            registerBackButton.interactable = true;
            Tools.UpdateStatusText(registerPanelStatusText, "All input fields must be filled", Color.red);
            return;
        }
        if (registerEmailInput.text != registerRetypeEmailInput.text)
        {
            registerBackButton.interactable = true;
            Tools.UpdateStatusText(registerPanelStatusText, "Emails don't match", Color.red);
            return;
        }
        if (registerPasswordInput.text != registerRetypePasswordInput.text)
        {
            registerBackButton.interactable = true;
            Tools.UpdateStatusText(registerPanelStatusText, "Passwords don't match", Color.red);
            return;
        }
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("username", registerUsernameInput.text);
        dataForm.AddField("email", registerEmailInput.text);
        dataForm.AddField("password", registerPasswordInput.text);
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "registerAccount.php", dataForm.form);
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            registerBackButton.interactable = true;
            Tools.UpdateStatusText(registerPanelStatusText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
        if (response == "-999")
        {
            registerBackButton.interactable = true;
            Tools.UpdateStatusText(registerPanelStatusText, "Server error while fetching data", Color.red);
            return;
        }
        else if (response == "-998")
        {
            registerBackButton.interactable = true;
            Tools.UpdateStatusText(registerPanelStatusText, "Client version too outdated to access servers", Color.red);
            return;
        }
        else if (response == "-997")
        {
            registerBackButton.interactable = true;
            Tools.UpdateStatusText(registerPanelStatusText, "Encryption/decryption issues", Color.red);
            return;
        }
        else if (response == "-996")
        {
            registerBackButton.interactable = true;
            Tools.UpdateStatusText(registerPanelStatusText, "Can't send requests on self-built instance", Color.red);
            return;
        }
        else
        {
            var jsonResponse = JObject.Parse(response);
            if ((bool)jsonResponse["success"])
            {
                AccountHandler.instance.SwitchPanel(2);
            }
            else
            {
                Tools.UpdateStatusText(registerPanelStatusText, (string)jsonResponse["message"], Color.red);
            }
        }
        registerBackButton.interactable = true;
    }
}