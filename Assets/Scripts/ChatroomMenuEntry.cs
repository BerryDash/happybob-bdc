using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChatroomMenuEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Image bgImg;
    private Button optionsButton;
    private static ChatroomMenuEntry activeEntry;

    public void Init(Image bgImgArg, Button optionsButtonArg)
    {
        bgImg = bgImgArg;
        optionsButton = optionsButtonArg;
        bgImg.color = new Color(50f / 255f, 50f / 255f, 50f / 255f);
        optionsButton.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData e)
    {
        if (!Application.isMobilePlatform)
        {
            if (activeEntry != null && activeEntry != this)
            {
                activeEntry.Deactivate();
            }
            if (activeEntry != this)
            {
                Activate();
            }
        }
    }

    public void OnPointerExit(PointerEventData e)
    {
        if (!Application.isMobilePlatform)
        {
            if (activeEntry != null && activeEntry != this)
            {
                activeEntry.Deactivate();
            }
            if (activeEntry == this)
            {
                Deactivate();
            }
        }
    }

    public void OnPointerClick(PointerEventData e)
    {
        if (Application.isMobilePlatform)
        {
            if (activeEntry != null && activeEntry != this)
            {
                activeEntry.Deactivate();
            }
            if (activeEntry == this)
            {
                Deactivate();
            }
            else
            {
                Activate();
            }
        }
        else if (e.button == PointerEventData.InputButton.Right)
        {
            optionsButton.onClick?.Invoke();
        }
    }

    private void Activate()
    {
        activeEntry = this;
        bgImg.color = new Color(60f / 255f, 60f / 255f, 60f / 255f);
        optionsButton.gameObject.SetActive(true);
    }

    private void Deactivate()
    {
        bgImg.color = new Color(50f / 255f, 50f / 255f, 50f / 255f);
        optionsButton.gameObject.SetActive(false);
        if (activeEntry == this) activeEntry = null;
    }
}
