using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class DraggableUI : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    public static DraggableUI Instance;
    private Vector2 offset;
    public string uiName;
    public bool canDrag = false;

    private readonly List<GameObject> outlineSides = new();

    void CreateOutline()
    {
        var borderColor = Color.cyan;
        float thickness = 2f;

        CreateSide("Top", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, thickness), borderColor);
        CreateSide("Bottom", new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, thickness), borderColor);
        CreateSide("Left", new Vector2(0, 0), new Vector2(0, 1), new Vector2(thickness, 0), borderColor);
        CreateSide("Right", new Vector2(1, 0), new Vector2(1, 1), new Vector2(thickness, 0), borderColor);
    }

    void CreateSide(string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 size, Color color)
    {
        var go = new GameObject(name, typeof(Image));
        go.transform.SetParent(transform, false);
        var rt = go.GetComponent<RectTransform>();
        var img = go.GetComponent<Image>();
        img.color = color;
        img.raycastTarget = false;

        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.sizeDelta = size;
        rt.anchoredPosition = Vector2.zero;

        outlineSides.Add(go);
    }

    public void Awake()
    {
        Instance = this;
        CreateOutline();
    }

    public void OnEnable()
    {
        string key = "DraggedUI" + uiName;
        if (PlayerPrefs.HasKey(key) && TryParseVector3(PlayerPrefs.GetString(key), out Vector3 savedPos)) transform.localPosition = savedPos;
    }

    public void Update()
    {
        foreach (var side in outlineSides)
            side.SetActive(canDrag);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)transform.parent,
            eventData.position,
            eventData.pressEventCamera,
            out offset
        );
        offset = (Vector2)transform.localPosition - offset;
        PlayerPrefs.SetString("DraggedUI" + uiName, transform.localPosition.ToString());
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)transform.parent,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        )) transform.localPosition = localPoint + offset;
        PlayerPrefs.SetString("DraggedUI" + uiName, transform.localPosition.ToString());
    }

    private bool TryParseVector3(string s, out Vector3 result)
    {
        s = s.Trim('(', ')');
        string[] parts = s.Split(',');
        if (parts.Length == 3 &&
            float.TryParse(parts[0], out float x) &&
            float.TryParse(parts[1], out float y) &&
            float.TryParse(parts[2], out float z))
        {
            result = new Vector3(x, y, z);
            return true;
        }
        result = Vector3.zero;
        return false;
    }
}