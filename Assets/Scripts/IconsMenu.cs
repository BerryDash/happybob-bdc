using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Iconsmenu : MonoBehaviour
{
    public GameObject iconsPanel;
    public GameObject overlaysPanel;
    public Button backButton;
    public Sprite defaultIcon;
    public Button placeholderButton;
    public TMP_Text selectionText;
    public Image previewBird;
    public Image previewOverlay;
    public Button[] icons;
    public Button[] overlays;
    public ColorPanel iconColorPanel;
    public ColorPanel overlayColorPanel;

    private void Start()
    {
        iconColorPanel.Init(BazookaManager.Instance.GetColorSettingIcon(), Color.white);
        iconColorPanel.OnColorChanged += color =>
        {
            BazookaManager.Instance.SetColorSettingIcon(color);
            foreach (var icon in icons)
            {
                icon.transform.GetChild(0).GetComponent<Image>().color = new Color(
                    int.Parse(color[0].ToString()) / 255f,
                    int.Parse(color[1].ToString()) / 255f,
                    int.Parse(color[2].ToString()) / 255f
                );
            }
        };
        overlayColorPanel.Init(BazookaManager.Instance.GetColorSettingOverlay(), Color.white);
        overlayColorPanel.OnColorChanged += color =>
        {
            BazookaManager.Instance.SetColorSettingOverlay(color);
            foreach (var overlay in overlays)
            {
                var img = overlay.transform.GetChild(0).TryGetComponent<Image>(out var image) ? image : null;
                if (img == null) continue;

                img.color = new Color(
                    int.Parse(color[0].ToString()) / 255f,
                    int.Parse(color[1].ToString()) / 255f,
                    int.Parse(color[2].ToString()) / 255f
                );
            }
        };

        defaultIcon = Tools.GetIconForUser(BazookaManager.Instance.GetAccountID() ?? 0);
        icons[0].transform.GetChild(0).GetComponent<Image>().sprite = defaultIcon;

        SwitchToIcon();
        SelectOverlay(BazookaManager.Instance.GetBirdOverlay());
        SelectIcon(BazookaManager.Instance.GetBirdIcon());

        if (BazookaManager.Instance.GetBirdIcon() == 7)
        {
            SelectOverlay(0);
            placeholderButton.interactable = false;
        }
        placeholderButton.onClick.AddListener(ToggleKit);
        backButton.onClick.AddListener(async () =>
        {
            await SceneManager.LoadSceneAsync("MainMenu");
        });
        previewBird.GetComponentInParent<Button>().onClick.AddListener(() =>
        {
            var scale = previewBird.transform.localScale;
            if (scale.x == -1)
            {
                scale.x = 1;
            }
            else
            {
                scale.x = -1;
            }
            previewBird.transform.localScale = scale;
        });
        for (int i = 0; i < icons.Length; i++)
        {
            int index = i;
            icons[i].onClick.AddListener(() => SelectIcon(index + 1));
        }
        for (int i = 0; i < overlays.Length; i++)
        {
            int index = i;
            overlays[i].onClick.AddListener(() => SelectOverlay(index));
        }
    }

    private void SwitchToIcon()
    {
        iconsPanel.SetActive(true);
        overlaysPanel.SetActive(false);
        selectionText.text = "Icon selection";
        placeholderButton.GetComponentInChildren<TMP_Text>().text = "Overlays";
        iconColorPanel.gameObject.SetActive(true);
        overlayColorPanel.gameObject.SetActive(true);
    }

    private void SwitchToOverlay()
    {
        iconsPanel.SetActive(false);
        overlaysPanel.SetActive(true);
        selectionText.text = "Overlay selection";
        placeholderButton.GetComponentInChildren<TMP_Text>().text = "Icons";
    }

    private void ToggleKit()
    {
        if (GetCurrentKit() == 1)
        {
            SwitchToOverlay();
        }
        else if (GetCurrentKit() == 2)
        {
            SwitchToIcon();
        }
    }

    private int GetCurrentKit()
    {
        if (iconsPanel.activeSelf)
        {
            return 1;
        }
        if (overlaysPanel.activeSelf)
        {
            return 2;
        }
        return 0;
    }

    private void SelectIcon(int iconID)
    {
        BazookaManager.Instance.SetBirdIcon(iconID);
        for (int i = 0; i < icons.Length; i++)
        {
            icons[i].interactable = iconID != i + 1;
        }
        previewBird.sprite = Resources.Load<Sprite>("Icons/Icons/bird_" + iconID);
        if (iconID == 1)
        {
            previewBird.sprite = defaultIcon;
        }
        if (iconID == 7)
        {
            SelectOverlay(0);
            placeholderButton.interactable = false;
        }
        else
        {
            placeholderButton.interactable = true;
        }
    }

    private void SelectOverlay(int overlayID)
    {
        BazookaManager.Instance.SetBirdOverlay(overlayID);
        for (int i = 0; i < overlays.Length; i++)
        {
            overlays[i].interactable = overlayID != i;
        }
        previewOverlay.rectTransform.localPosition = new Vector3(-32f, 44.50001f, 0f);
        previewOverlay.gameObject.SetActive(true);
        if (overlayID == 8)
        {
            previewOverlay.rectTransform.localPosition = new Vector3(-35.36f, 31.6f, 0f);
        }
        else if (overlayID == 11)
        {
            previewOverlay.rectTransform.localPosition = new Vector3(-31.44f, 43.50004f, 0f);
        }
        else if (overlayID == 13)
        {
            previewOverlay.rectTransform.localPosition = new Vector3(-35.28575f, 31.3667f, 0f);
        }

        if (overlayID == 0)
        {
            previewOverlay.gameObject.SetActive(false);
            previewOverlay.sprite = null;
        }
        else
        {
            previewOverlay.sprite = Resources.Load<Sprite>("Icons/Overlays/overlay_" + overlayID);
        }
    }
}