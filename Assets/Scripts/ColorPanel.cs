using System;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorPanel : MonoBehaviour
{
    public Slider rSlider;
    public Slider gSlider;
    public Slider bSlider;
    public ColorPickerUI colorPickerUI;
    public GameObject manualModeUI;
    public TMP_InputField hexValue;
    public Image previewImage;
    public Button resetButton;
    public Button switchModeButton;
    public event Action<JArray> OnColorChanged;
    public Color defaultColor;

    public void Init(Color color, Color defaultColorArg)
    {
        defaultColor = defaultColorArg;
        rSlider.value = color.r * 255f;
        gSlider.value = color.g * 255f;
        bSlider.value = color.b * 255f;

        SyncAll();

        rSlider.onValueChanged.AddListener(_ => SyncAll());
        gSlider.onValueChanged.AddListener(_ => SyncAll());
        bSlider.onValueChanged.AddListener(_ => SyncAll());

        hexValue.onValueChanged.AddListener(value =>
        {
            var hex = value.StartsWith("#") ? value : "#" + value;
            if (hex.Length == 7 && ColorUtility.TryParseHtmlString(hex, out var col))
            {
                rSlider.SetValueWithoutNotify(col.r * 255f);
                gSlider.SetValueWithoutNotify(col.g * 255f);
                bSlider.SetValueWithoutNotify(col.b * 255f);
                SyncAll();
            }
        });

        resetButton.onClick.AddListener(() =>
        {
            rSlider.value = defaultColor.r * 255f;
            gSlider.value = defaultColor.g * 255f;
            bSlider.value = defaultColor.b * 255f;
        });

        switchModeButton.onClick.AddListener(() =>
        {
            bool enableManual = !manualModeUI.activeSelf;
            manualModeUI.SetActive(enableManual);
            colorPickerUI.gameObject.SetActive(!enableManual);
        });

        colorPickerUI.OnColorChanged += color =>
        {
            rSlider.SetValueWithoutNotify(color.r * 255);
            gSlider.SetValueWithoutNotify(color.g * 255);
            bSlider.SetValueWithoutNotify(color.b * 255);
            SyncAll(fromPicker: true);
        };
    }

    public void Init(JArray color, Color defaultColorArg)
    {

        Init(new Color((int)color[0] / 255f, (int)color[1] / 255f, (int)color[2] / 255f), defaultColorArg);
    }

    void SyncAll(bool fromPicker = false)
    {
        var col = new Color(rSlider.value / 255f, gSlider.value / 255f, bSlider.value / 255f);

        if (!fromPicker) colorPickerUI.SetSelectedColor(rSlider.value, gSlider.value, bSlider.value);

        if (previewImage != null) previewImage.color = col;
        hexValue.SetTextWithoutNotify("#" + ColorUtility.ToHtmlStringRGB(col));
        OnColorChanged?.Invoke(new JArray(
            (int)rSlider.value,
            (int)gSlider.value,
            (int)bSlider.value
        ));
    }

    public void SetColor(Color col)
    {
        rSlider.SetValueWithoutNotify(col.r * 255f);
        gSlider.SetValueWithoutNotify(col.g * 255f);
        bSlider.SetValueWithoutNotify(col.b * 255f);
        SyncAll();
    }

    public void SetColor(JArray color)
    {
        rSlider.SetValueWithoutNotify((int)color[0]);
        gSlider.SetValueWithoutNotify((int)color[1]);
        bSlider.SetValueWithoutNotify((int)color[2]);
        SyncAll();
    }
}
