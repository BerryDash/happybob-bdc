using UnityEngine;
using UnityEngine.UI;

public class ButtonToURL : MonoBehaviour
{
    public string url;

    void Awake()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            Application.OpenURL(url);
        });
    }
}