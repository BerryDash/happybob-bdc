using UnityEngine;

public class IgnoreRaycast : MonoBehaviour, ICanvasRaycastFilter
{
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera) => GetComponent<DraggableUI>() != null && GetComponent<DraggableUI>().canDrag;
}