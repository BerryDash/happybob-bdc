using System.IO;
using System.Numerics;
using System.Text;
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
        string path = Path.Join(Application.persistentDataPath, "save.json");
        if (!File.Exists(path))
        {
            File.Create(path).Dispose();
        }
        else
        {
            try
            {
                saveFile = JObject.Parse(File.ReadAllText(path));
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
    }

    public void Save()
    {
#if UNITY_EDITOR
        return;
#else
        string path = Path.Join(Application.persistentDataPath, "save.json");
        var encoded = Encoding.UTF8.GetBytes(saveFile.ToString());
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

    public void SetSettingRandomMusic(bool value)
    {
        if (saveFile["settings"] == null) saveFile["settings"] = new JObject();
        saveFile["settings"]["randomMusic"] = value;
    }

    public bool GetSettingRandomMusic()
    {
        if (saveFile["settings"] == null) return true;
        if (saveFile["settings"]["randomMusic"] == null) return true;
        return bool.Parse(saveFile["settings"]["randomMusic"].ToString());
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

    public void SetColorSettingMenuBackground(JArray value)
    {
        if (saveFile["settings"] == null) saveFile["settings"] = new JObject();
        if (saveFile["settings"]["colors"] == null) saveFile["settings"]["colors"] = new JObject();
        saveFile["settings"]["colors"]["menuBackground"] = value;
    }

    public JArray GetColorSettingMenuBackground()
    {
        if (saveFile["settings"] == null) return new JArray(24, 24, 24);
        if (saveFile["settings"]["colors"] == null) return new JArray(24, 24, 24);
        if (saveFile["settings"]["colors"]["menuBackground"] == null) return new JArray(24, 24, 24);
        return JArray.Parse(saveFile["settings"]["colors"]["menuBackground"].ToString());
    }

    public void SetColorSettingButton(JArray value)
    {
        if (saveFile["settings"] == null) saveFile["settings"] = new JObject();
        if (saveFile["settings"]["colors"] == null) saveFile["settings"]["colors"] = new JObject();
        saveFile["settings"]["colors"]["button"] = value;
    }

    public JArray GetColorSettingButton()
    {
        if (saveFile["settings"] == null) return new JArray(255, 255, 255);
        if (saveFile["settings"]["colors"] == null) return new JArray(255, 255, 255);
        if (saveFile["settings"]["colors"]["button"] == null) return new JArray(255, 255, 255);
        return JArray.Parse(saveFile["settings"]["colors"]["button"].ToString());
    }

    public void SetColorSettingText(JArray value)
    {
        if (saveFile["settings"] == null) saveFile["settings"] = new JObject();
        if (saveFile["settings"]["colors"] == null) saveFile["settings"]["colors"] = new JObject();
        saveFile["settings"]["colors"]["text"] = value;
    }

    public JArray GetColorSettingText()
    {
        if (saveFile["settings"] == null) return new JArray(255, 255, 255);
        if (saveFile["settings"]["colors"] == null) return new JArray(255, 255, 255);
        if (saveFile["settings"]["colors"]["text"] == null) return new JArray(255, 255, 255);
        return JArray.Parse(saveFile["settings"]["colors"]["text"].ToString());
    }

    public void SetColorSettingButtonContent(JArray value)
    {
        if (saveFile["settings"] == null) saveFile["settings"] = new JObject();
        if (saveFile["settings"]["colors"] == null) saveFile["settings"]["colors"] = new JObject();
        saveFile["settings"]["colors"]["buttonColor"] = value;
    }

    public JArray GetColorSettingButtonContent()
    {
        if (saveFile["settings"] == null) return new JArray(0, 0, 0);
        if (saveFile["settings"]["colors"] == null) return new JArray(0, 0, 0);
        if (saveFile["settings"]["colors"]["buttonColor"] == null) return new JArray(0, 0, 0);
        return JArray.Parse(saveFile["settings"]["colors"]["buttonColor"].ToString());
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

    public void SetGameStoreTotalRandomBerries(BigInteger value)
    {
        if (saveFile["gameStore"] == null) saveFile["gameStore"] = new JObject();
        saveFile["gameStore"]["totalRandomBerries"] = value.ToString();
    }

    public BigInteger GetGameStoreTotalRandomBerries()
    {
        if (saveFile["gameStore"] == null) return 0;
        if (saveFile["gameStore"]["totalRandomBerries"] == null) return 0;
        return BigInteger.Parse(saveFile["gameStore"]["totalRandomBerries"].ToString());
    }

    public void UnsetGameStoreTotalRandomBerries()
    {
        if (saveFile["gameStore"] == null) return;
        (saveFile["gameStore"] as JObject)?.Remove("totalRandomBerries");
    }

    public void SetGameStoreTotalAntiBerries(BigInteger value)
    {
        if (saveFile["gameStore"] == null) saveFile["gameStore"] = new JObject();
        saveFile["gameStore"]["totalAntiBerries"] = value.ToString();
    }

    public BigInteger GetGameStoreTotalAntiBerries()
    {
        if (saveFile["gameStore"] == null) return 0;
        if (saveFile["gameStore"]["totalAntiBerries"] == null) return 0;
        return BigInteger.Parse(saveFile["gameStore"]["totalAntiBerries"].ToString());
    }

    public void UnsetGameStoreTotalAntiBerries()
    {
        if (saveFile["gameStore"] == null) return;
        (saveFile["gameStore"] as JObject)?.Remove("totalAntiBerries");
    }

    public void SetGameStoreTotalTimeSlowBerries(BigInteger value)
    {
        if (saveFile["gameStore"] == null) saveFile["gameStore"] = new JObject();
        saveFile["gameStore"]["totalTimeSlowBerries"] = value.ToString();
    }

    public BigInteger GetGameStoreTotalTimeSlowBerries()
    {
        if (saveFile["gameStore"] == null) return 0;
        if (saveFile["gameStore"]["totalTimeSlowBerries"] == null) return 0;
        return BigInteger.Parse(saveFile["gameStore"]["totalTimeSlowBerries"].ToString());
    }

    public void UnsetGameStoreTotalTimeSlowBerries()
    {
        if (saveFile["gameStore"] == null) return;
        (saveFile["gameStore"] as JObject)?.Remove("totalTimeSlowBerries");
    }

    public void SetGameStoreTotalPurpleBerries(BigInteger value)
    {
        if (saveFile["gameStore"] == null) saveFile["gameStore"] = new JObject();
        saveFile["gameStore"]["totalPurpleBerries"] = value.ToString();
    }

    public BigInteger GetGameStoreTotalPurpleBerries()
    {
        if (saveFile["gameStore"] == null) return 0;
        if (saveFile["gameStore"]["totalPurpleBerries"] == null) return 0;
        return BigInteger.Parse(saveFile["gameStore"]["totalPurpleBerries"].ToString());
    }

    public void UnsetGameStoreTotalPurpleBerries()
    {
        if (saveFile["gameStore"] == null) return;
        (saveFile["gameStore"] as JObject)?.Remove("totalPurpleBerries");
    }
    public void SetGameStoreTotalTimeFreezeBerries(BigInteger value)
    {
        if (saveFile["gameStore"] == null) saveFile["gameStore"] = new JObject();
        saveFile["gameStore"]["totalTimeFreezeBerries"] = value.ToString();
    }

    public BigInteger GetGameStoreTotalTimeFreezeBerries()
    {
        if (saveFile["gameStore"] == null) return 0;
        if (saveFile["gameStore"]["totalTimeFreezeBerries"] == null) return 0;
        return BigInteger.Parse(saveFile["gameStore"]["totalTimeFreezeBerries"].ToString());
    }

    public void UnsetGameStoreTotalTimeFreezeBerries()
    {
        if (saveFile["gameStore"] == null) return;
        (saveFile["gameStore"] as JObject)?.Remove("totalTimeFreezeBerries");
    }

    public void SetGameStoreTotalEvilBerries(BigInteger value)
    {
        if (saveFile["gameStore"] == null) saveFile["gameStore"] = new JObject();
        saveFile["gameStore"]["totalEvilBerries"] = value.ToString();
    }

    public BigInteger GetGameStoreTotalEvilBerries()
    {
        if (saveFile["gameStore"] == null) return 0;
        if (saveFile["gameStore"]["totalEvilBerries"] == null) return 0;
        return BigInteger.Parse(saveFile["gameStore"]["totalEvilBerries"].ToString());
    }

    public void UnsetGameStoreTotalEvilBerries()
    {
        if (saveFile["gameStore"] == null) return;
        (saveFile["gameStore"] as JObject)?.Remove("totalEvilBerries");
    }
}
