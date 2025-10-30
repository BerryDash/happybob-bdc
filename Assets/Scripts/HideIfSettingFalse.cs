using UnityEngine;

public class HideIfSettingFalse : MonoBehaviour
{
    public SettingTypes setting;
    public bool reverse;

    void Start()
    {
        bool value = GetSettingValue(setting);
        gameObject.SetActive(value == !reverse);
    }

    bool GetSettingValue(SettingTypes s)
    {
        var b = BazookaManager.Instance;
        return s switch
        {
            SettingTypes.FullScreen => b.GetSettingFullScreen(),
            SettingTypes.ShowFPS => b.GetSettingShowFPS(),
            SettingTypes.Vsync => b.GetSettingVsync(),
            _ => false
        };
    }
}