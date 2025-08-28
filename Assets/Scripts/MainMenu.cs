using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public Button exitButton;

    void Awake()
    {
        if (Application.isMobilePlatform || Application.isEditor)
        {
            exitButton.gameObject.SetActive(false);
        }
        else
        {
            exitButton.onClick.AddListener(() =>
            {
                Application.Quit();
            });
        }
    }
}