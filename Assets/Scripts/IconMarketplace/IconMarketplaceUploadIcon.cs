using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SimpleFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class IconMarketplaceUploadIcon : MonoBehaviour
{
    public IconMarketplaceManager marketplaceManager;
    public TMP_Text statusText;
    public Button backButton;
    public Button uploadButton;
    public Button selectButton;
    public TMP_InputField birdNameInput;
    public TMP_InputField birdPriceInput;
    public Image birdImage;
    public GameObject birdImageNone;
    private string birdData = null;

    void Awake()
    {
        backButton.onClick.AddListener(() => marketplaceManager.SwitchPanel(0));
        uploadButton.onClick.AddListener(() =>
        {
            if (birdNameInput.text.Trim() == string.Empty)
            {
                Tools.UpdateStatusText(statusText, "Bird name can't be empty", Color.red);
            }
            else if (birdPriceInput.text.Trim() == string.Empty)
            {
                Tools.UpdateStatusText(statusText, "Bird price can't be empty", Color.red);
            }
            else if (birdData == null)
            {
                Tools.UpdateStatusText(statusText, "You must upload an image", Color.red);
            }
            else
            {
                UploadIcon();
            }
        });
        selectButton.onClick.AddListener(async () =>
        {
            string path = await OpenFilePicker();
            if (path == null) return;
            byte[] fileContent;
            try
            {
                fileContent = File.ReadAllBytes(path);
                if (fileContent.Length > 1024 * 1024)
                {
                    Tools.UpdateStatusText(statusText, "File size exceeds 1 MB limit", Color.red);
                    return;
                }
                birdData = Convert.ToBase64String(fileContent);
                Tools.RenderFromBase64(birdData, birdImage);
                birdImageNone.SetActive(false);
                birdImage.gameObject.SetActive(true);
            }
            catch (Exception e)
            {
                Tools.UpdateStatusText(statusText, "Failed to read file: " + e.Message, Color.red);
                return;
            }
        });
    }

    async Task<string> OpenFilePicker()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".png"));

        var tcs = new TaskCompletionSource<string>();

        FileBrowser.ShowLoadDialog(
            (paths) => tcs.SetResult(paths.Length > 0 ? paths[0] : null),
            () => tcs.SetResult(null),
            FileBrowser.PickMode.Files,
            false
        );

        return await tcs.Task;
    }

    async void UploadIcon()
    {
        uploadButton.interactable = false;
        backButton.interactable = false;
        selectButton.interactable = false;
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("token", BazookaManager.Instance.GetAccountSession());
        dataForm.AddField("username", BazookaManager.Instance.GetAccountName());
        dataForm.AddField("name", birdNameInput.text);
        dataForm.AddField("price", birdPriceInput.text);
        dataForm.AddField("filecontent", birdData);
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "uploadMarketplaceIcon.php", dataForm.form);
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            uploadButton.interactable = true;
            backButton.interactable = true;
            selectButton.interactable = true;
            Tools.UpdateStatusText(statusText, "Failed to make HTTP request", Color.red);
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
        if (response == "-999")
        {
            uploadButton.interactable = true;
            backButton.interactable = true;
            selectButton.interactable = true;
            Tools.UpdateStatusText(statusText, "Server error while fetching data", Color.red);
            return;
        }
        else if (response == "-998")
        {
            uploadButton.interactable = true;
            backButton.interactable = true;
            selectButton.interactable = true;
            Tools.UpdateStatusText(statusText, "Client version too outdated to access servers", Color.red);
            return;
        }
        else if (response == "-997")
        {
            uploadButton.interactable = true;
            backButton.interactable = true;
            selectButton.interactable = true;
            Tools.UpdateStatusText(statusText, "Encryption/decryption issues", Color.red);
            return;
        }
        else if (response == "-996")
        {
            uploadButton.interactable = true;
            backButton.interactable = true;
            selectButton.interactable = true;
            Tools.UpdateStatusText(statusText, "Can't send requests on self-built instance", Color.red);
            return;
        }
        else
        {
            var jsonResponse = JObject.Parse(response);
            if ((bool)jsonResponse["success"])
            {
                Reset();
                Tools.UpdateStatusText(statusText, (string)jsonResponse["message"], Color.green);
            }
            else
            {
                Tools.UpdateStatusText(statusText, (string)jsonResponse["message"], Color.red);
            }
        }
        uploadButton.interactable = true;
        backButton.interactable = true;
        selectButton.interactable = true;
    }

    internal void Reset()
    {
        birdImage.sprite = null;
        birdImageNone.SetActive(true);
        birdImage.gameObject.SetActive(false);
        birdData = null;
        birdNameInput.text = string.Empty;
        birdPriceInput.text = string.Empty;
        statusText.text = string.Empty;
    }
}
