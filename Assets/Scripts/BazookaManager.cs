using System.IO;
using System.Numerics;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class BazookaManager : MonoBehaviour
{
    public static BazookaManager Instance;
    private bool firstLoadDone = false;
    public JObject saveFile = new()
    {
        ["version"] = "0"
    };

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (!firstLoadDone)
            {
                firstLoadDone = true;
                Load();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnApplicationQuit()
    {
        Save();
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Save();
        }
    }

    public void Load()
    {
        string path = Path.Join(Application.persistentDataPath, SensitiveInfo.BAZOOKA_MANAGER_FILE_KEY + ".dat");
        if (!File.Exists(path))
        {
            File.Create(path).Dispose();
        }
        else
        {
            try
            {
                var tempSaveFile = JObject.Parse(SensitiveInfo.DecryptRaw(File.ReadAllBytes(path), SensitiveInfo.BAZOOKA_MANAGER_KEY));
                if (tempSaveFile != null) saveFile = tempSaveFile;
            }
            catch
            {
                Debug.LogWarning("Failed to load save file");
            }
        }
        if (saveFile["version"] == null || saveFile["version"].ToString() != "0")
        {
            saveFile["version"] = "0";
        }
        if (!PlayerPrefs.HasKey("LegacyConversion"))
        {
            PlayerPrefs.SetInt("LegacyConversion", 1);
            if (PlayerPrefs.HasKey("HighScoreV2"))
            {
                try
                {
                    SetGameStoreHighScore(BigInteger.Parse(PlayerPrefs.GetString("HighScoreV2", "0")));
                }
                catch
                {
                    Debug.LogError("Failed to migrate HighScoreV2 to new save format");
                }
                PlayerPrefs.DeleteKey("HighScoreV2");
            }
            if (PlayerPrefs.HasKey("TotalNormalBerries"))
            {
                try
                {
                    SetGameStoreTotalNormalBerries(BigInteger.Parse(PlayerPrefs.GetString("TotalNormalBerries", "0")));
                }
                catch
                {
                    Debug.LogError("Failed to migrate TotalNormalBerries to new save format");
                }
                PlayerPrefs.DeleteKey("TotalNormalBerries");
            }
            if (PlayerPrefs.HasKey("TotalPoisonBerries"))
            {
                try
                {
                    SetGameStoreTotalPoisonBerries(BigInteger.Parse(PlayerPrefs.GetString("TotalPoisonBerries", "0")));
                }
                catch
                {
                    Debug.LogError("Failed to migrate TotalPoisonBerries to new save format");
                }
                PlayerPrefs.DeleteKey("TotalPoisonBerries");
            }
            if (PlayerPrefs.HasKey("TotalSlowBerries"))
            {
                try
                {
                    SetGameStoreTotalSlowBerries(BigInteger.Parse(PlayerPrefs.GetString("TotalSlowBerries", "0")));
                }
                catch
                {
                    Debug.LogError("Failed to migrate TotalSlowBerries to new save format");
                }
                PlayerPrefs.DeleteKey("TotalSlowBerries");
            }
            if (PlayerPrefs.HasKey("TotalUltraBerries"))
            {
                try
                {
                    SetGameStoreTotalUltraBerries(BigInteger.Parse(PlayerPrefs.GetString("TotalUltraBerries", "0")));
                }
                catch
                {
                    Debug.LogError("Failed to migrate TotalUltraBerries to new save format");
                }
                PlayerPrefs.DeleteKey("TotalUltraBerries");
            }
            if (PlayerPrefs.HasKey("TotalSpeedyBerries"))
            {
                try
                {
                    SetGameStoreTotalSpeedyBerries(BigInteger.Parse(PlayerPrefs.GetString("TotalSpeedyBerries", "0")));
                }
                catch
                {
                    Debug.LogError("Failed to migrate TotalSpeedyBerries to new save format");
                }
                PlayerPrefs.DeleteKey("TotalSpeedyBerries");
            }
            if (PlayerPrefs.HasKey("TotalAttempts"))
            {
                try
                {
                    SetGameStoreTotalAttepts(BigInteger.Parse(PlayerPrefs.GetString("TotalAttempts", "0")));
                }
                catch
                {
                    Debug.LogError("Failed to migrate TotalAttempts to new save format");
                }
                PlayerPrefs.DeleteKey("TotalAttempts");
            }
            if (PlayerPrefs.HasKey("pastOverlay"))
            {
                PlayerPrefs.DeleteKey("pastOverlay");
            }
            if (PlayerPrefs.HasKey("gameSession") && PlayerPrefs.GetString("gameSession") != null && PlayerPrefs.GetString("gameSession").Length == 512)
            {
                try
                {
                    SetAccountSession(PlayerPrefs.GetString("gameSession"));
                }
                catch
                {
                    Debug.LogError("Failed to migrate gameSession to new save format");
                }
                PlayerPrefs.DeleteKey("gameSession");
            }
            if (PlayerPrefs.HasKey("userName"))
            {
                try
                {
                    SetAccountSession(PlayerPrefs.GetString("userName"));
                }
                catch
                {
                    Debug.LogError("Failed to migrate userName to new save format");
                }
                PlayerPrefs.DeleteKey("userName");
            }
            if (PlayerPrefs.HasKey("userId"))
            {
                try
                {
                    SetAccountID(PlayerPrefs.GetInt("userId", 0));
                }
                catch
                {
                    Debug.LogError("Failed to migrate userId to new save format");
                }
                PlayerPrefs.DeleteKey("userId");
            }
            if (PlayerPrefs.HasKey("userId"))
            {
                try
                {
                    SetAccountID(PlayerPrefs.GetInt("userId", 0));
                }
                catch
                {
                    Debug.LogError("Failed to migrate userId to new save format");
                }
                PlayerPrefs.DeleteKey("userId");
            }
            if (PlayerPrefs.HasKey("musicVolume"))
            {
                try
                {
                    SetSettingMusicVolume(PlayerPrefs.GetFloat("musicVolume", 1f));
                }
                catch
                {
                    Debug.LogError("Failed to migrate musicVolume to new save format");
                }
                PlayerPrefs.DeleteKey("musicVolume");
            }
            if (PlayerPrefs.HasKey("sfxVolume"))
            {
                try
                {
                    SetSettingSFXVolume(PlayerPrefs.GetFloat("sfxVolume", 1f));
                }
                catch
                {
                    Debug.LogError("Failed to migrate sfxVolume to new save format");
                }
                PlayerPrefs.DeleteKey("sfxVolume");
            }
            if (PlayerPrefs.HasKey("BirdColor"))
            {
                try
                {
                    var birdColor = PlayerPrefs.GetString("BirdColor", "255;255;255").Split(";");
                    SetColorSettingIcon(new JArray(birdColor[0], birdColor[1], birdColor[2]));
                }
                catch
                {
                    Debug.LogError("Failed to migrate BirdColor to new save format");
                }
                PlayerPrefs.DeleteKey("BirdColor");
            }
            if (PlayerPrefs.HasKey("OverlayColor"))
            {
                try
                {
                    var overlayColor = PlayerPrefs.GetString("OverlayColor", "255;255;255").Split(";");
                    SetColorSettingOverlay(new JArray(overlayColor[0], overlayColor[1], overlayColor[2]));
                }
                catch
                {
                    Debug.LogError("Failed to migrate OverlayColor to new save format");
                }
                PlayerPrefs.DeleteKey("OverlayColor");
            }
            if (PlayerPrefs.HasKey("OverlayColor"))
            {
                try
                {
                    var overlayColor = PlayerPrefs.GetString("OverlayColor", "255;255;255").Split(";");
                    SetColorSettingOverlay(new JArray(overlayColor[0], overlayColor[1], overlayColor[2]));
                }
                catch
                {
                    Debug.LogError("Failed to migrate OverlayColor to new save format");
                }
                PlayerPrefs.DeleteKey("OverlayColor");
            }
            if (PlayerPrefs.HasKey("BackgroundColor"))
            {
                try
                {
                    var bgColor = PlayerPrefs.GetString("BackgroundColor", "58;58;58").Split(";");
                    SetColorSettingOverlay(new JArray(bgColor[0], bgColor[1], bgColor[2]));
                }
                catch
                {
                    Debug.LogError("Failed to migrate BackgroundColor to new save format");
                }
                PlayerPrefs.DeleteKey("BackgroundColor");
            }
            if (PlayerPrefs.HasKey("BackgroundColor"))
            {
                try
                {
                    var bgColor = PlayerPrefs.GetString("BackgroundColor", "58;58;58").Split(";");
                    SetColorSettingOverlay(new JArray(bgColor[0], bgColor[1], bgColor[2]));
                }
                catch
                {
                    Debug.LogError("Failed to migrate BackgroundColor to new save format");
                }
                PlayerPrefs.DeleteKey("BackgroundColor");
            }
            if (PlayerPrefs.HasKey("icon"))
            {
                try
                {
                    SetBirdIcon(PlayerPrefs.GetInt("icon", 1));
                }
                catch
                {
                    Debug.LogError("Failed to migrate icon to new save format");
                }
                PlayerPrefs.DeleteKey("icon");
            }
            if (PlayerPrefs.HasKey("overlay"))
            {
                try
                {
                    SetBirdOverlay(PlayerPrefs.GetInt("overlay", 1));
                }
                catch
                {
                    Debug.LogError("Failed to migrate overlay to new save format");
                }
                PlayerPrefs.DeleteKey("overlay");
            }
            if (PlayerPrefs.HasKey("overlay"))
            {
                try
                {
                    SetBirdOverlay(PlayerPrefs.GetInt("overlay", 1));
                }
                catch
                {
                    Debug.LogError("Failed to migrate overlay to new save format");
                }
                PlayerPrefs.DeleteKey("overlay");
            }
            if (PlayerPrefs.HasKey("Setting1"))
            {
                try
                {
                    SetSettingFullScreen(PlayerPrefs.GetInt("Setting1", 1) == 1);
                }
                catch
                {
                    Debug.LogError("Failed to migrate Setting1 to new save format");
                }
                PlayerPrefs.DeleteKey("Setting1");
            }
            if (PlayerPrefs.HasKey("Setting2"))
            {
                try
                {
                    SetSettingShowFPS(PlayerPrefs.GetInt("Setting2", 1) == 1);
                }
                catch
                {
                    Debug.LogError("Failed to migrate Setting2 to new save format");
                }
                PlayerPrefs.DeleteKey("Setting2");
            }
            if (PlayerPrefs.HasKey("Setting3"))
            {
                try
                {
                    SetSettingVsync(PlayerPrefs.GetInt("Setting3", 1) == 1);
                }
                catch
                {
                    Debug.LogError("Failed to migrate Setting3 to new save format");
                }
                PlayerPrefs.DeleteKey("Setting3");
            }
            if (PlayerPrefs.HasKey("Setting4"))
            {
                try
                {
                    SetSettingHideSocials(PlayerPrefs.GetInt("Setting4", 1) == 1);
                }
                catch
                {
                    Debug.LogError("Failed to migrate Setting4 to new save format");
                }
                PlayerPrefs.DeleteKey("Setting4");
            }
        }
    }

    public void Save()
    {
#if UNITY_EDITOR
        return;
#else
        string path = Path.Join(Application.persistentDataPath, SensitiveInfo.BAZOOKA_MANAGER_FILE_KEY + ".dat");
        var encoded = SensitiveInfo.EncryptRaw(saveFile.ToString(Newtonsoft.Json.Formatting.None), SensitiveInfo.BAZOOKA_MANAGER_KEY);
        if (encoded == null) return;
        using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        fileStream.Write(encoded, 0, encoded.Length);
        fileStream.Flush(true);
#endif
    }

    public void ResetSave()
    {
        saveFile = new JObject
        {
            ["version"] = "0"
        };
        Save();
    }

    //Bird stuff

    public void SetBirdIcon(int value)
    {
        if (saveFile["bird"] == null) saveFile["bird"] = new JObject();
        saveFile["bird"]["icon"] = value;
    }

    public void UnsetBirdIcon()
    {
        if (saveFile["bird"] == null) return;
        if (saveFile["bird"]["icon"] == null) return;
        (saveFile["bird"] as JObject)?.Remove("icon");
    }

    public int GetBirdIcon()
    {
        if (saveFile["bird"] == null) return 1;
        if (saveFile["bird"]["icon"] == null) return 1;
        return int.Parse(saveFile["bird"]["icon"].ToString());
    }

    public void SetBirdOverlay(int value)
    {
        if (saveFile["bird"] == null) saveFile["bird"] = new JObject();
        saveFile["bird"]["overlay"] = value;
    }


    public void UnsetBirdOverlay()
    {
        if (saveFile["bird"] == null) return;
        if (saveFile["bird"]["overlay"] == null) return;
        (saveFile["bird"] as JObject)?.Remove("overlay");
    }
    public int GetBirdOverlay()
    {
        if (saveFile["bird"] == null) return 0;
        if (saveFile["bird"]["overlay"] == null) return 0;
        return int.Parse(saveFile["bird"]["overlay"].ToString());
    }
    
    public void SetCustomBirdIconData(MarketplaceIconStorageType value)
    {
        if (saveFile["bird"] == null) saveFile["bird"] = new JObject();
        saveFile["bird"]["customIcon"] = JObject.FromObject(value);
    }

    public void UnsetCustomBirdIconData()
    {
        if (saveFile["bird"] == null) return;
        if (saveFile["bird"]["customIcon"] == null) return;
        (saveFile["bird"] as JObject)?.Remove("customIcon");
    }

    public MarketplaceIconStorageType GetCustomBirdIconData()
    {
        if (saveFile["bird"] == null) return new();
        if (saveFile["bird"]["customIcon"] == null) return new();
        return saveFile["bird"]["customIcon"].ToObject<MarketplaceIconStorageType>();
    }

    //Settings stuff

    public void SetSettingFullScreen(bool value)
    {
        if (saveFile["settings"] == null) saveFile["settings"] = new JObject();
        saveFile["settings"]["fullScreen"] = value;
    }

    public bool GetSettingFullScreen()
    {
        if (saveFile["settings"] == null) return true;
        if (saveFile["settings"]["fullScreen"] == null) return true;
        return bool.Parse(saveFile["settings"]["fullScreen"].ToString());
    }

    public void SetSettingShowFPS(bool value)
    {
        if (saveFile["settings"] == null) saveFile["settings"] = new JObject();
        saveFile["settings"]["showFps"] = value;
    }

    public bool GetSettingShowFPS()
    {
        if (saveFile["settings"] == null) return false;
        if (saveFile["settings"]["showFps"] == null) return false;
        return bool.Parse(saveFile["settings"]["showFps"].ToString());
    }

    public void SetSettingVsync(bool value)
    {
        if (saveFile["settings"] == null) saveFile["settings"] = new JObject();
        saveFile["settings"]["vsync"] = value;
    }

    public bool GetSettingVsync()
    {
        if (saveFile["settings"] == null) return true;
        if (saveFile["settings"]["vsync"] == null) return true;
        return bool.Parse(saveFile["settings"]["vsync"].ToString());
    }

    public void SetSettingHideSocials(bool value)
    {
        if (saveFile["settings"] == null) saveFile["settings"] = new JObject();
        saveFile["settings"]["hideSocials"] = value;
    }

    public bool? GetSettingHideSocials()
    {
        if (saveFile["settings"] == null) return null;
        if (saveFile["settings"]["hideSocials"] == null) return null;
        return bool.Parse(saveFile["settings"]["hideSocials"].ToString());
    }

    public void SetSettingMusicVolume(float value)
    {
        if (saveFile["settings"] == null) saveFile["settings"] = new JObject();
        saveFile["settings"]["musicVolume"] = value;
    }

    public float GetSettingMusicVolume()
    {
        if (saveFile["settings"] == null) return 1f;
        if (saveFile["settings"]["musicVolume"] == null) return 1f;
        return float.Parse(saveFile["settings"]["musicVolume"].ToString());
    }

    public void SetSettingSFXVolume(float value)
    {
        if (saveFile["settings"] == null) saveFile["settings"] = new JObject();
        saveFile["settings"]["sfxVolume"] = value;
    }

    public float GetSettingSFXVolume()
    {
        if (saveFile["settings"] == null) return 1f;
        if (saveFile["settings"]["sfxVolume"] == null) return 1f;
        return float.Parse(saveFile["settings"]["sfxVolume"].ToString());
    }

    public void SetColorSettingBackground(JArray value)
    {
        if (saveFile["settings"] == null) saveFile["settings"] = new JObject();
        if (saveFile["settings"]["colors"] == null) saveFile["settings"]["colors"] = new JObject();
        saveFile["settings"]["colors"]["background"] = value;
    }

    public JArray GetColorSettingBackground()
    {
        if (saveFile["settings"] == null) return new JArray(58, 58, 58);
        if (saveFile["settings"]["colors"] == null) return new JArray(58, 58, 58);
        if (saveFile["settings"]["colors"]["background"] == null) return new JArray(58, 58, 58);
        return JArray.Parse(saveFile["settings"]["colors"]["background"].ToString());
    }

    public void SetColorSettingIcon(JArray value)
    {
        if (saveFile["settings"] == null) saveFile["settings"] = new JObject();
        if (saveFile["settings"]["colors"] == null) saveFile["settings"]["colors"] = new JObject();
        saveFile["settings"]["colors"]["icon"] = value;
    }

    public JArray GetColorSettingIcon()
    {
        if (saveFile["settings"] == null) return new JArray(255, 255, 255);
        if (saveFile["settings"]["colors"] == null) return new JArray(255, 255, 255);
        if (saveFile["settings"]["colors"]["icon"] == null) return new JArray(255, 255, 255);
        return JArray.Parse(saveFile["settings"]["colors"]["icon"].ToString());
    }

    public void UnsetColorSettingIcon()
    {
        if (saveFile["settings"] == null) return;
        if (saveFile["settings"]["colors"] == null) return;
        if (saveFile["settings"]["colors"]["icon"] == null) return;
        (saveFile["settings"]["colors"] as JObject)?.Remove("icon");
    }

    public void SetColorSettingOverlay(JArray value)
    {
        if (saveFile["settings"] == null) saveFile["settings"] = new JObject();
        if (saveFile["settings"]["colors"] == null) saveFile["settings"]["colors"] = new JObject();
        saveFile["settings"]["colors"]["overlay"] = value;
    }

    public JArray GetColorSettingOverlay()
    {
        if (saveFile["settings"] == null) return new JArray(255, 255, 255);
        if (saveFile["settings"]["colors"] == null) return new JArray(255, 255, 255);
        if (saveFile["settings"]["colors"]["overlay"] == null) return new JArray(255, 255, 255);
        return JArray.Parse(saveFile["settings"]["colors"]["overlay"].ToString());
    }

    public void UnsetColorSettingOverlay()
    {
        if (saveFile["settings"] == null) return;
        if (saveFile["settings"]["colors"] == null) return;
        if (saveFile["settings"]["colors"]["overlay"] == null) return;
        (saveFile["settings"]["colors"] as JObject)?.Remove("overlay");
    }

    //Account stuff

    public void SetAccountSession(string value)
    {
        if (saveFile["account"] == null) saveFile["account"] = new JObject();
        saveFile["account"]["session"] = value;
    }

    public string GetAccountSession()
    {
        if (saveFile["account"] == null) return null;
        if (saveFile["account"]["session"] == null) return null;
        return saveFile["account"]["session"].ToString();
    }

    public void UnsetAccountSession()
    {
        if (saveFile["account"] == null) return;
        (saveFile["account"] as JObject)?.Remove("session");
    }

    public void SetAccountName(string value)
    {
        if (saveFile["account"] == null) saveFile["account"] = new JObject();
        saveFile["account"]["name"] = value;
    }

    public string GetAccountName()
    {
        if (saveFile["account"] == null) return null;
        if (saveFile["account"]["name"] == null) return null;
        return saveFile["account"]["name"].ToString();
    }

    public void UnsetAccountName()
    {
        if (saveFile["account"] == null) return;
        (saveFile["account"] as JObject)?.Remove("name");
    }

    public void SetAccountID(BigInteger value)
    {
        if (saveFile["account"] == null) saveFile["account"] = new JObject();
        saveFile["account"]["id"] = value.ToString();
    }

    public BigInteger? GetAccountID()
    {
        if (saveFile["account"] == null) return null;
        if (saveFile["account"]["id"] == null) return null;
        return BigInteger.Parse(saveFile["account"]["id"].ToString());
    }

    public void UnsetAccountID()
    {
        if (saveFile["account"] == null) return;
        (saveFile["account"] as JObject)?.Remove("id");
    }

    //Game store stuff

    public void SetGameStoreHighScore(BigInteger value)
    {
        if (saveFile["gameStore"] == null) saveFile["gameStore"] = new JObject();
        saveFile["gameStore"]["highScore"] = value.ToString();
    }

    public BigInteger GetGameStoreHighScore()
    {
        if (saveFile["gameStore"] == null) return 0;
        if (saveFile["gameStore"]["highScore"] == null) return 0;
        return BigInteger.Parse(saveFile["gameStore"]["highScore"].ToString());
    }

    public void UnsetGameStoreHighScore()
    {
        if (saveFile["gameStore"] == null) return;
        (saveFile["gameStore"] as JObject)?.Remove("highScore");
    }

    public void SetGameStoreTotalAttepts(BigInteger value)
    {
        if (saveFile["gameStore"] == null) saveFile["gameStore"] = new JObject();
        saveFile["gameStore"]["totalAttempts"] = value.ToString();
    }

    public BigInteger GetGameStoreTotalAttepts()
    {
        if (saveFile["gameStore"] == null) return 0;
        if (saveFile["gameStore"]["totalAttempts"] == null) return 0;
        return BigInteger.Parse(saveFile["gameStore"]["totalAttempts"].ToString());
    }

    public void UnsetGameStoreTotalAttepts()
    {
        if (saveFile["gameStore"] == null) return;
        (saveFile["gameStore"] as JObject)?.Remove("totalAttempts");
    }

    public void SetGameStoreTotalNormalBerries(BigInteger value)
    {
        if (saveFile["gameStore"] == null) saveFile["gameStore"] = new JObject();
        saveFile["gameStore"]["totalNormalBerries"] = value.ToString();
    }

    public BigInteger GetGameStoreTotalNormalBerries()
    {
        if (saveFile["gameStore"] == null) return 0;
        if (saveFile["gameStore"]["totalNormalBerries"] == null) return 0;
        return BigInteger.Parse(saveFile["gameStore"]["totalNormalBerries"].ToString());
    }

    public void UnsetGameStoreTotalNormalBerries()
    {
        if (saveFile["gameStore"] == null) return;
        (saveFile["gameStore"] as JObject)?.Remove("totalNormalBerries");
    }

    public void SetGameStoreTotalPoisonBerries(BigInteger value)
    {
        if (saveFile["gameStore"] == null) saveFile["gameStore"] = new JObject();
        saveFile["gameStore"]["totalPoisonBerries"] = value.ToString();
    }

    public BigInteger GetGameStoreTotalPoisonBerries()
    {
        if (saveFile["gameStore"] == null) return 0;
        if (saveFile["gameStore"]["totalPoisonBerries"] == null) return 0;
        return BigInteger.Parse(saveFile["gameStore"]["totalPoisonBerries"].ToString());
    }

    public void UnsetGameStoreTotalPoisonBerries()
    {
        if (saveFile["gameStore"] == null) return;
        (saveFile["gameStore"] as JObject)?.Remove("totalPoisonBerries");
    }

    public void SetGameStoreTotalSlowBerries(BigInteger value)
    {
        if (saveFile["gameStore"] == null) saveFile["gameStore"] = new JObject();
        saveFile["gameStore"]["totalSlowBerries"] = value.ToString();
    }

    public BigInteger GetGameStoreTotalSlowBerries()
    {
        if (saveFile["gameStore"] == null) return 0;
        if (saveFile["gameStore"]["totalSlowBerries"] == null) return 0;
        return BigInteger.Parse(saveFile["gameStore"]["totalSlowBerries"].ToString());
    }

    public void UnsetGameStoreTotalSlowBerries()
    {
        if (saveFile["gameStore"] == null) return;
        (saveFile["gameStore"] as JObject)?.Remove("totalSlowBerries");
    }

    public void SetGameStoreTotalUltraBerries(BigInteger value)
    {
        if (saveFile["gameStore"] == null) saveFile["gameStore"] = new JObject();
        saveFile["gameStore"]["totalUltraBerries"] = value.ToString();
    }

    public BigInteger GetGameStoreTotalUltraBerries()
    {
        if (saveFile["gameStore"] == null) return 0;
        if (saveFile["gameStore"]["totalUltraBerries"] == null) return 0;
        return BigInteger.Parse(saveFile["gameStore"]["totalUltraBerries"].ToString());
    }

    public void UnsetGameStoreTotalUltraBerries()
    {
        if (saveFile["gameStore"] == null) return;
        (saveFile["gameStore"] as JObject)?.Remove("totalUltraBerries");
    }

    public void SetGameStoreTotalSpeedyBerries(BigInteger value)
    {
        if (saveFile["gameStore"] == null) saveFile["gameStore"] = new JObject();
        saveFile["gameStore"]["totalSpeedyBerries"] = value.ToString();
    }

    public BigInteger GetGameStoreTotalSpeedyBerries()
    {
        if (saveFile["gameStore"] == null) return 0;
        if (saveFile["gameStore"]["totalSpeedyBerries"] == null) return 0;
        return BigInteger.Parse(saveFile["gameStore"]["totalSpeedyBerries"].ToString());
    }

    public void UnsetGameStoreTotalSpeedyBerries()
    {
        if (saveFile["gameStore"] == null) return;
        (saveFile["gameStore"] as JObject)?.Remove("totalSpeedyBerries");
    }

    public void SetGameStoreTotalCoinBerries(BigInteger value)
    {
        if (saveFile["gameStore"] == null) saveFile["gameStore"] = new JObject();
        saveFile["gameStore"]["totalCoinBerries"] = value.ToString();
    }

    public BigInteger GetGameStoreTotalCoinBerries()
    {
        if (saveFile["gameStore"] == null) return 0;
        if (saveFile["gameStore"]["totalCoinBerries"] == null) return 0;
        return BigInteger.Parse(saveFile["gameStore"]["totalCoinBerries"].ToString());
    }

    public void UnsetGameStoreTotalCoinBerries()
    {
        if (saveFile["gameStore"] == null) return;
        (saveFile["gameStore"] as JObject)?.Remove("totalCoinBerries");
    }
}
