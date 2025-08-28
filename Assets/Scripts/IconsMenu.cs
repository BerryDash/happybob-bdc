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
    public GameObject marketplaceIconsPanel;
    public GameObject marketplaceIconsContent;
    public GameObject marketplaceIconsSample;
    public Button backButton;
    public Sprite defaultIcon;
    public Button placeholderButton;
    public TMP_Text selectionText;
    public Image previewBird;
    public Image previewOverlay;
    public Button[] icons;
    public Button[] overlays;
    private Dictionary<string, Button> customIcons = new();
    public GameObject previewBirdObject;
    public ColorPanel iconColorPanel;
    public ColorPanel overlayColorPanel;

    private void Start()
    {
        var customIconData = BazookaManager.Instance.GetCustomBirdIconData();
        foreach (var icon in customIconData.Data)
        {
            var iconEntry = Instantiate(marketplaceIconsSample, marketplaceIconsContent.transform);
            iconEntry.name = "MarketplaceIcon";
            var button = iconEntry.GetComponent<Button>();
            customIcons[icon.UUID] = button;
            button.onClick.AddListener(() =>
            {
                SelectCustomIcon(icon);
            });
            Tools.RenderFromBase64(icon.Data, iconEntry.transform.GetChild(0).GetComponent<Image>());
            iconEntry.SetActive(true);
        }
        iconColorPanel.Init(customIconData.Selected == null ? BazookaManager.Instance.GetColorSettingIcon() : JArray.Parse("[255,255,255]"), Color.white);
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
        overlayColorPanel.Init(customIconData.Selected == null ? BazookaManager.Instance.GetColorSettingOverlay() : JArray.Parse("[255,255,255]"), Color.white);
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

        if (customIconData.Selected == null)
        {
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
        }
        else
        {
            SwitchToMarketplaceIcons();
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
        marketplaceIconsPanel.SetActive(false);
        selectionText.text = "Icon selection";
        placeholderButton.GetComponentInChildren<TMP_Text>().text = "Overlays";
        iconColorPanel.gameObject.SetActive(true);
        overlayColorPanel.gameObject.SetActive(true);
    }

    private void SwitchToOverlay()
    {
        iconsPanel.SetActive(false);
        overlaysPanel.SetActive(true);
        marketplaceIconsPanel.SetActive(false);
        selectionText.text = "Overlay selection";
        placeholderButton.GetComponentInChildren<TMP_Text>().text = "Marketplace";
    }

    private void SwitchToMarketplaceIcons()
    {
        iconsPanel.SetActive(false);
        overlaysPanel.SetActive(false);
        marketplaceIconsPanel.SetActive(true);
        selectionText.text = "Marketplace Icons selection";
        placeholderButton.GetComponentInChildren<TMP_Text>().text = "Icons";
        iconColorPanel.gameObject.SetActive(false);
        overlayColorPanel.gameObject.SetActive(false);
        var customIconData = BazookaManager.Instance.GetCustomBirdIconData();
        foreach (var btn in customIcons.Values) btn.interactable = true;
        if (customIconData.Selected != null)
        {
            foreach (var icon in customIconData.Data)
            {
                if (icon.UUID == customIconData.Selected)
                {
                    SelectCustomIcon(icon);
                }
                customIcons[icon.UUID].interactable = icon.UUID != customIconData.Selected;
            }
        }
    }

    private void ToggleKit()
    {
        if (GetCurrentKit() == 1)
        {
            SwitchToOverlay();
        }
        else if (GetCurrentKit() == 2)
        {
            SwitchToMarketplaceIcons();
        }
        else if (GetCurrentKit() == 3)
        {
            defaultIcon = Tools.GetIconForUser(BazookaManager.Instance.GetAccountID() ?? 0);
            icons[0].transform.GetChild(0).GetComponent<Image>().sprite = defaultIcon;

            SwitchToIcon();
            SelectOverlay(BazookaManager.Instance.GetBirdOverlay());
            SelectIcon(BazookaManager.Instance.GetBirdIcon());
            iconColorPanel.SetColor(BazookaManager.Instance.GetColorSettingIcon());
            overlayColorPanel.SetColor(BazookaManager.Instance.GetColorSettingOverlay());

            if (BazookaManager.Instance.GetBirdIcon() == 7)
            {
                SelectOverlay(0);
                placeholderButton.interactable = false;
            }
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
        if (marketplaceIconsPanel.activeSelf)
        {
            return 3;
        }
        return 0;
    }

    private void SelectIcon(int iconID)
    {
        var customData = BazookaManager.Instance.GetCustomBirdIconData();
        customData.Selected = null;
        BazookaManager.Instance.SetCustomBirdIconData(customData);
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
        var customData = BazookaManager.Instance.GetCustomBirdIconData();
        customData.Selected = null;
        BazookaManager.Instance.SetCustomBirdIconData(customData);
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

    void SelectCustomIcon(MarketplaceIconType icon)
    {
        var customData = BazookaManager.Instance.GetCustomBirdIconData();
        customData.Selected = icon.UUID;
        BazookaManager.Instance.SetCustomBirdIconData(customData);
        Tools.RenderFromBase64(icon.Data, previewBird);
        previewBird.color = Color.white;
        previewOverlay.gameObject.SetActive(false);
        previewOverlay.sprite = null;
        previewOverlay.color = Color.white;
        foreach (var loopIcon in customData.Data)
        {
            customIcons[loopIcon.UUID].interactable = loopIcon.UUID != icon.UUID;
        }
    }
}