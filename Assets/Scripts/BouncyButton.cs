using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BouncyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool isHovered = false;
    bool isHeld = false;
    bool didBounce = false;
    Vector3 originalScale;
    Vector3 finalScale;

    void Start()
    {
        originalScale = transform.localScale;
        finalScale = originalScale * 1.1f;
    }

    public void OnPointerEnter(PointerEventData eventData) => isHovered = true;
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        ResetScale();
    }
    void OnDisable()
    {
        isHovered = false;
        ResetScale();
    }

    void Update()
    {
        if (Application.isMobilePlatform)
        {
            isHeld = Touchscreen.current != null && Touchscreen.current.press.isPressed;
        }
        else
        {
            isHeld = Mouse.current != null && Mouse.current.leftButton.isPressed;
        }

        if (isHovered && isHeld && !didBounce)
        {
            StartCoroutine(BounceOnce());
            didBounce = true;
        }

        if ((!isHovered || !isHeld) && didBounce)
        {
            ResetScale();
        }
    }

    void ResetScale()
    {
        StopAllCoroutines();
        transform.localScale = originalScale;
        didBounce = false;
    }

    System.Collections.IEnumerator BounceOnce()
    {
        float t = 0.125f;
        Vector3 s1 = originalScale * 1.12f;
        Vector3 s2 = originalScale * 1.06f;
        Vector3 s3 = finalScale;

        yield return LerpScale(originalScale, s1, t);
        yield return LerpScale(s1, s2, t * 0.5f);
        yield return LerpScale(s2, s3, t * 0.5f);

        transform.localScale = s3;
    }

    System.Collections.IEnumerator LerpScale(Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float p = elapsed / duration;
            transform.localScale = Vector3.Lerp(from, to, p);
            yield return null;
        }
        transform.localScale = to;
    }
}