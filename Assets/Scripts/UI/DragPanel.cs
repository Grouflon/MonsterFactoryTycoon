using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DragPanel : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public RectTransform panelRectTransform;

	void Start()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            m_canvasRectTransform = canvas.transform as RectTransform;
        }
    }

    public void OnPointerDown(PointerEventData _data)
    {
        panelRectTransform.SetAsLastSibling();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform, _data.position, _data.pressEventCamera, out m_pointerOffset);
    }

    public void OnDrag(PointerEventData _data)
    {
        if (panelRectTransform == null)
            return;

        Vector2 pointerPosition = ClampToWindow(_data);
        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_canvasRectTransform, pointerPosition, _data.pressEventCamera, out localPointerPosition))
        {
            panelRectTransform.localPosition = localPointerPosition - m_pointerOffset;
        }
    }

    Vector2 ClampToWindow(PointerEventData _data)
    {
        Vector3[] canvasCorners = new Vector3[4];
        m_canvasRectTransform.GetWorldCorners(canvasCorners);

        float clampedX = Mathf.Clamp(_data.position.x, canvasCorners[0].x, canvasCorners[2].x);
        float clampedY = Mathf.Clamp(_data.position.y, canvasCorners[0].y, canvasCorners[2].y);

        return new Vector2(clampedX, clampedY);
    }



    private Vector2 m_pointerOffset;
    private RectTransform m_canvasRectTransform;
}
