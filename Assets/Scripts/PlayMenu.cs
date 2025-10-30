using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayMenu : MonoBehaviour
{
    [SerializeField] private GameObject selectionMenu;
    [SerializeField] private GameObject customMenu;
    [SerializeField] private Button customButton;
    [SerializeField] private Button customBackButton;
    [SerializeField] private Button customPlayButton;
    [SerializeField] private Button customNormalizeButton;

    [SerializeField] private TMP_InputField normalBerryChance;
    [SerializeField] private TMP_InputField poisonBerryChance;
    [SerializeField] private TMP_InputField slowBerryChance;
    [SerializeField] private TMP_InputField ultraBerryChance;
    [SerializeField] private TMP_InputField speedyBerryChance;
    [SerializeField] private TMP_InputField randomBerryChance;
    [SerializeField] private TMP_InputField antiBerryChance;
    [SerializeField] private TMP_InputField nothingBerryChance;
    [SerializeField] private TMP_InputField timeSlowBerryChance;
    [SerializeField] private TMP_InputField purpleBerryChance;
    [SerializeField] private TMP_InputField timeFreezeBerryChance;
    [SerializeField] private TMP_InputField evilBerryChance;

    [SerializeField] private TMP_Text validateTotalText;

    private readonly float defaultNormalBerryChance = 27.5f;
    private readonly float defaultPoisonBerryChance = 12.5f;
    private readonly float defaultSlowBerryChance = 10f;
    private readonly float defaultUltraBerryChance = 10f;
    private readonly float defaultSpeedyBerryChance = 10f;
    private readonly float defaultRandomBerryChance = 5f;
    private readonly float defaultAntiBerryChance = 5f;
    private readonly float defaultNothingBerryChance = 0f;
    private readonly float defaultTimeSlowBerryChance = 5f;
    private readonly float defaultPurpleBerryChance = 5f;
    private readonly float defaultTimeFreezeBerryChance = 5f;
    private readonly float defaultEvilBerryChance = 5f;

    void Awake()
    {
        customButton.onClick.AddListener(() =>
        {
            selectionMenu.SetActive(false);
            customMenu.SetActive(true);
        });
        customBackButton.onClick.AddListener(() =>
        {
            customMenu.SetActive(false);
            selectionMenu.SetActive(true);

            normalBerryChance.text = defaultNormalBerryChance.ToString();
            poisonBerryChance.text = defaultPoisonBerryChance.ToString();
            slowBerryChance.text = defaultSlowBerryChance.ToString();
            ultraBerryChance.text = defaultUltraBerryChance.ToString();
            speedyBerryChance.text = defaultSpeedyBerryChance.ToString();
            randomBerryChance.text = defaultRandomBerryChance.ToString();
            antiBerryChance.text = defaultAntiBerryChance.ToString();
            nothingBerryChance.text = defaultNothingBerryChance.ToString();
            timeSlowBerryChance.text = defaultTimeSlowBerryChance.ToString();
            purpleBerryChance.text = defaultPurpleBerryChance.ToString();
            timeFreezeBerryChance.text = defaultTimeFreezeBerryChance.ToString();
            evilBerryChance.text = defaultEvilBerryChance.ToString();
            ValidateTotal();
        });
        customNormalizeButton.onClick.AddListener(() =>
        {
            float normalBerry = GetValueFrom(normalBerryChance);
            float poisonBerry = GetValueFrom(poisonBerryChance);
            float slowBerry = GetValueFrom(slowBerryChance);
            float ultraBerry = GetValueFrom(ultraBerryChance);
            float speedyBerry = GetValueFrom(speedyBerryChance);
            float randomBerry = GetValueFrom(randomBerryChance);
            float antiBerry = GetValueFrom(antiBerryChance);
            float nothingBerry = GetValueFrom(nothingBerryChance);
            float timeSlowBerry = GetValueFrom(timeSlowBerryChance);
            float purpleBerry = GetValueFrom(purpleBerryChance);
            float timeFreezeBerry = GetValueFrom(timeFreezeBerryChance);
            float evilBerry = GetValueFrom(evilBerryChance);

            int divideBy = 0;

            divideBy += normalBerry > 0 ? 1 : 0;
            divideBy += poisonBerry > 0 ? 1 : 0;
            divideBy += slowBerry > 0 ? 1 : 0;
            divideBy += ultraBerry > 0 ? 1 : 0;
            divideBy += speedyBerry > 0 ? 1 : 0;
            divideBy += randomBerry > 0 ? 1 : 0;
            divideBy += antiBerry > 0 ? 1 : 0;
            divideBy += nothingBerry > 0 ? 1 : 0;
            divideBy += timeSlowBerry > 0 ? 1 : 0;
            divideBy += purpleBerry > 0 ? 1 : 0;
            divideBy += timeFreezeBerry > 0 ? 1 : 0;
            divideBy += evilBerry > 0 ? 1 : 0;

            float addedChances = normalBerry + poisonBerry + slowBerry + ultraBerry + speedyBerry + randomBerry + antiBerry + nothingBerry + timeSlowBerry + purpleBerry + timeFreezeBerry + evilBerry;
            float difference = addedChances - 100f;

            if (normalBerry > 0) NormalizeOne(normalBerryChance, normalBerry, divideBy, difference);
            if (poisonBerry > 0) NormalizeOne(poisonBerryChance, poisonBerry, divideBy, difference);
            if (slowBerry > 0) NormalizeOne(slowBerryChance, slowBerry, divideBy, difference);
            if (ultraBerry > 0) NormalizeOne(ultraBerryChance, ultraBerry, divideBy, difference);
            if (speedyBerry > 0) NormalizeOne(speedyBerryChance, speedyBerry, divideBy, difference);
            if (randomBerry > 0) NormalizeOne(randomBerryChance, randomBerry, divideBy, difference);
            if (antiBerry > 0) NormalizeOne(antiBerryChance, antiBerry, divideBy, difference);
            if (nothingBerry > 0) NormalizeOne(nothingBerryChance, nothingBerry, divideBy, difference);
            if (timeSlowBerry > 0) NormalizeOne(timeSlowBerryChance, timeSlowBerry, divideBy, difference);
            if (purpleBerry > 0) NormalizeOne(purpleBerryChance, purpleBerry, divideBy, difference);
            if (timeFreezeBerry > 0) NormalizeOne(timeFreezeBerryChance, timeFreezeBerry, divideBy, difference);
            if (purpleBerry > 0) NormalizeOne(evilBerryChance, evilBerry, divideBy, difference);

            normalBerry = GetValueFrom(normalBerryChance);
            poisonBerry = GetValueFrom(poisonBerryChance);
            slowBerry = GetValueFrom(slowBerryChance);
            ultraBerry = GetValueFrom(ultraBerryChance);
            speedyBerry = GetValueFrom(speedyBerryChance);
            randomBerry = GetValueFrom(randomBerryChance);
            antiBerry = GetValueFrom(antiBerryChance);
            nothingBerry = GetValueFrom(nothingBerryChance);
            timeSlowBerry = GetValueFrom(timeSlowBerryChance);
            purpleBerry = GetValueFrom(purpleBerryChance);
            timeFreezeBerry = GetValueFrom(timeFreezeBerryChance);
            evilBerry = GetValueFrom(evilBerryChance);

            if (normalBerry > 0) normalBerry = (float)Math.Floor(normalBerry);
            if (poisonBerry > 0) poisonBerry = (float)Math.Floor(poisonBerry);
            if (slowBerry > 0) slowBerry = (float)Math.Floor(slowBerry);
            if (ultraBerry > 0) ultraBerry = (float)Math.Floor(ultraBerry);
            if (speedyBerry > 0) speedyBerry = (float)Math.Floor(speedyBerry);
            if (randomBerry > 0) randomBerry = (float)Math.Floor(randomBerry);
            if (antiBerry > 0) antiBerry = (float)Math.Floor(antiBerry);
            if (nothingBerry > 0) nothingBerry = (float)Math.Floor(nothingBerry);
            if (timeSlowBerry > 0) timeSlowBerry = (float)Math.Floor(timeSlowBerry);
            if (purpleBerry > 0) purpleBerry = (float)Math.Floor(purpleBerry);
            if (timeFreezeBerry > 0) timeFreezeBerry = (float)Math.Floor(timeFreezeBerry);
            if (evilBerry > 0) evilBerry = (float)Math.Floor(evilBerry);

            float addedChances2 = normalBerry + poisonBerry + slowBerry + ultraBerry + speedyBerry + randomBerry + antiBerry + nothingBerry + timeSlowBerry + purpleBerry + timeFreezeBerry + evilBerry;
            float difference2 = addedChances2 - 100f;
            bool fixedValues = false;

            if (normalBerry > 0)
            {
                NormalizeTwo(normalBerryChance, fixedValues, normalBerry, difference2);
                fixedValues = true;
            }
            if (poisonBerry > 0)
            {
                NormalizeTwo(poisonBerryChance, fixedValues, poisonBerry, difference2);
                fixedValues = true;
            }
            if (slowBerry > 0)
            {
                NormalizeTwo(slowBerryChance, fixedValues, slowBerry, difference2);
                fixedValues = true;
            }
            if (ultraBerry > 0)
            {
                NormalizeTwo(ultraBerryChance, fixedValues, ultraBerry, difference2);
                fixedValues = true;
            }
            if (speedyBerry > 0)
            {
                NormalizeTwo(speedyBerryChance, fixedValues, speedyBerry, difference2);
                fixedValues = true;
            }
            if (randomBerry > 0)
            {
                NormalizeTwo(randomBerryChance, fixedValues, randomBerry, difference2);
                fixedValues = true;
            }
            if (antiBerry > 0)
            {
                NormalizeTwo(antiBerryChance, fixedValues, antiBerry, difference2);
                fixedValues = true;
            }
            if (nothingBerry > 0)
            {
                NormalizeTwo(nothingBerryChance, fixedValues, nothingBerry, difference2);
                fixedValues = true;
            }
            if (nothingBerry > 0)
            {
                NormalizeTwo(nothingBerryChance, fixedValues, nothingBerry, difference2);
                fixedValues = true;
            }
            if (timeSlowBerry > 0)
            {
                NormalizeTwo(timeSlowBerryChance, fixedValues, timeSlowBerry, difference2);
                fixedValues = true;
            }
            if (purpleBerry > 0)
            {
                NormalizeTwo(purpleBerryChance, fixedValues, purpleBerry, difference2);
                fixedValues = true;
            }
            if (timeFreezeBerry > 0)
            {
                NormalizeTwo(timeFreezeBerryChance, fixedValues, timeFreezeBerry, difference2);
                fixedValues = true;
            }
            if (evilBerry > 0)
            {
                NormalizeTwo(evilBerryChance, fixedValues, evilBerry, difference2);
                fixedValues = true;
            }

            ValidateTotal();
        });
        customPlayButton.onClick.AddListener(async () =>
        {
            GameObject obj = new("CustomGameTempData");
            obj.AddComponent<CustomGameTempData>();
            CustomGameTempData customGameTempData = obj.GetComponent<CustomGameTempData>();
            customGameTempData.normalBerryChance = GetValueFrom(normalBerryChance);
            customGameTempData.poisonBerryChance = GetValueFrom(poisonBerryChance);
            customGameTempData.slowBerryChance = GetValueFrom(slowBerryChance);
            customGameTempData.ultraBerryChance = GetValueFrom(ultraBerryChance);
            customGameTempData.speedyBerryChance = GetValueFrom(speedyBerryChance);
            customGameTempData.randomBerryChance = GetValueFrom(randomBerryChance);
            customGameTempData.antiBerryChance = GetValueFrom(antiBerryChance);
            customGameTempData.nothingBerryChance = GetValueFrom(nothingBerryChance);
            customGameTempData.timeSlowBerryChance = GetValueFrom(timeSlowBerryChance);
            customGameTempData.purpleBerryChance = GetValueFrom(purpleBerryChance);
            customGameTempData.timeFreezeBerryChance = GetValueFrom(timeFreezeBerryChance);
            customGameTempData.evilBerryChance = GetValueFrom(evilBerryChance);
            await SceneManager.LoadSceneAsync("CustomGamePlayer");
        });

        normalBerryChance.onSelect.AddListener((value) => OnSelect(value, normalBerryChance));
        normalBerryChance.onDeselect.AddListener((value) => OnDeselect(value, normalBerryChance));
        poisonBerryChance.onSelect.AddListener((value) => OnSelect(value, poisonBerryChance));
        poisonBerryChance.onDeselect.AddListener((value) => OnDeselect(value, poisonBerryChance));
        slowBerryChance.onSelect.AddListener((value) => OnSelect(value, slowBerryChance));
        slowBerryChance.onDeselect.AddListener((value) => OnDeselect(value, slowBerryChance));
        ultraBerryChance.onSelect.AddListener((value) => OnSelect(value, ultraBerryChance));
        ultraBerryChance.onDeselect.AddListener((value) => OnDeselect(value, ultraBerryChance));
        speedyBerryChance.onSelect.AddListener((value) => OnSelect(value, speedyBerryChance));
        speedyBerryChance.onDeselect.AddListener((value) => OnDeselect(value, speedyBerryChance));
        randomBerryChance.onSelect.AddListener((value) => OnSelect(value, randomBerryChance));
        randomBerryChance.onDeselect.AddListener((value) => OnDeselect(value, randomBerryChance));
        antiBerryChance.onSelect.AddListener((value) => OnSelect(value, antiBerryChance));
        antiBerryChance.onDeselect.AddListener((value) => OnDeselect(value, antiBerryChance));
        nothingBerryChance.onSelect.AddListener((value) => OnSelect(value, nothingBerryChance));
        nothingBerryChance.onDeselect.AddListener((value) => OnDeselect(value, nothingBerryChance));
        timeSlowBerryChance.onSelect.AddListener((value) => OnSelect(value, timeSlowBerryChance));
        timeSlowBerryChance.onDeselect.AddListener((value) => OnDeselect(value, timeSlowBerryChance));
        purpleBerryChance.onSelect.AddListener((value) => OnSelect(value, purpleBerryChance));
        purpleBerryChance.onDeselect.AddListener((value) => OnDeselect(value, purpleBerryChance));
        timeFreezeBerryChance.onSelect.AddListener((value) => OnSelect(value, timeFreezeBerryChance));
        timeFreezeBerryChance.onDeselect.AddListener((value) => OnDeselect(value, timeFreezeBerryChance));
        evilBerryChance.onSelect.AddListener((value) => OnSelect(value, evilBerryChance));
        evilBerryChance.onDeselect.AddListener((value) => OnDeselect(value, evilBerryChance));
    }

    void ValidateTotal()
    {
        customBackButton.interactable = false;
        customPlayButton.interactable = false;
        customNormalizeButton.interactable = false;
        float total = 0f;
        total += GetValueFrom(normalBerryChance);
        total += GetValueFrom(poisonBerryChance);
        total += GetValueFrom(slowBerryChance);
        total += GetValueFrom(ultraBerryChance);
        total += GetValueFrom(speedyBerryChance);
        total += GetValueFrom(randomBerryChance);
        total += GetValueFrom(antiBerryChance);
        total += GetValueFrom(nothingBerryChance);
        total += GetValueFrom(timeSlowBerryChance);
        total += GetValueFrom(purpleBerryChance);
        total += GetValueFrom(timeFreezeBerryChance);
        total += GetValueFrom(evilBerryChance);
        if (total == 100f)
        {
            customBackButton.interactable = true;
            customPlayButton.interactable = true;
            validateTotalText.gameObject.SetActive(false);
        }
        else
        {
            customNormalizeButton.interactable = true;
            validateTotalText.text = "Total must add up to 100%!";
            validateTotalText.gameObject.SetActive(true);
        }
    }

    private float GetValueFrom(TMP_InputField inputField)
    {
        return GetValueFroText(inputField.text);
    }

    private float GetValueFroText(string text)
    {
        try
        {
            return float.Parse(text.Replace("%", "").Trim());
        }
        catch
        {
            return 0f;
        }
    }

    void OnSelect(string value, TMP_InputField inputField)
    {
        validateTotalText.gameObject.SetActive(false);
        customBackButton.interactable = false;
        customPlayButton.interactable = false;
        customNormalizeButton.interactable = false;
        inputField.text = value.Replace("%", "");
        inputField.stringPosition = inputField.text.Length;
    }

    void OnDeselect(string value, TMP_InputField inputField)
    {
        if (float.TryParse(value, out var value2) && value2 < 0f)
        {
            value = "0";
        }
        inputField.text = value + "%";
        ValidateTotal();
    }

    void NormalizeOne(TMP_InputField inputField, float berryChance, int divideBy, float difference)
    {
        inputField.text = (berryChance - (difference / divideBy)).ToString() + "%";
        inputField.stringPosition = inputField.text.Length;
    }

    void NormalizeTwo(TMP_InputField inputField, bool fixedValues, float berryChance, float difference2)
    {
        if (!fixedValues)
        {
            inputField.text = (berryChance - difference2).ToString() + "%";
            inputField.stringPosition = inputField.text.Length;
        }
        else
        {
            inputField.text = berryChance.ToString() + "%";
            inputField.stringPosition = inputField.text.Length;
        }
    }
}