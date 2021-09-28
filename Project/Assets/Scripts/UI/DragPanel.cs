using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DragPanel : MonoBehaviour, IPointerDownHandler
{
    public string direction;
    private bool dragging;
    public Vector3 maxVector, minVector;
    private Vector2 originalLocalPointerPosition;
    private Vector3 originalPanelLocalPosition;
    private RectTransform panelRectTransform;
    private RectTransform parentRectTransform;

    private void Awake()
    {
        panelRectTransform = transform.parent as RectTransform;
        parentRectTransform = panelRectTransform.parent as RectTransform;
    }

    public void Update()
    {
        if (panelRectTransform && parentRectTransform)
        {
            if (dragging)
            {
                transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    }
}