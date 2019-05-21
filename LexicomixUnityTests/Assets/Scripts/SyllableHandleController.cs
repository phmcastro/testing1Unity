using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SyllableHandleController : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.parent.GetComponent<SyllableSlotController>().HandleClicked();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Image>().color = Color.grey;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Image>().color = transform.parent.GetComponent<SyllableSlotController>().originalColor;
    }
}


