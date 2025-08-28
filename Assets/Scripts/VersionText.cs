using TMPro;
using UnityEngine;

public class VersionText : MonoBehaviour
{
    private TMP_Text text;
    void Awake()
    {
        text = gameObject.GetComponent<TMP_Text>();
        text.text = "Current: v" + Application.version;
    }
}