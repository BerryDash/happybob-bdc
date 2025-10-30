using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class GlobalFPSText : MonoBehaviour
{
    private TMP_Text text;
    private float timer;
    private int frames;
    private float fps;

    void Awake()
    {
        if (FindObjectsByType<GlobalFPSText>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject.transform.parent.gameObject);
            return;
        }
        DontDestroyOnLoad(transform.parent.gameObject);
        text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        var enabled = BazookaManager.Instance.GetSettingShowFPS();
        if (!enabled && text.text != "")
        {
            text.text = "";
            return;
        }
        else if (!enabled) return;
        frames++;
        timer += Time.unscaledDeltaTime;
        if (timer >= 0.25f)
        {
            fps = frames / timer;
            text.text = "FPS: " + Mathf.RoundToInt(fps);
            frames = 0;
            timer = 0;
        }
    }
}
