using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public static GameObject objectBeingDragged;
    private Vector3 startPos;
    private Transform startParent;

    public void OnBeginDrag(PointerEventData eventData)
    {
        objectBeingDragged = gameObject;
        startPos = transform.position;
        startParent = transform.parent;
        GetComponent<CanvasGroup>().blocksRaycasts = false;

    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        objectBeingDragged = null;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        if (transform.parent != startParent)
        {
            transform.position = startPos;
        }

    }
}
