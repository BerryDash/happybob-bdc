using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonToScene : MonoBehaviour
{
    public string sceneName;

    void Awake()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(async () =>
        {
            await SceneManager.LoadSceneAsync(sceneName);
        });
    }
}